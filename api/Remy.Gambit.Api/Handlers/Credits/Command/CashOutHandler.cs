using FluentValidation;
using Remy.Gambit.Api.Handlers.Credits.Command.Dto;
using Remy.Gambit.Api.Helpers;
using Remy.Gambit.Core.Concurrency;
using Remy.Gambit.Core.Cqs;
using Remy.Gambit.Data.Credits;
using Remy.Gambit.Data.Features;
using Remy.Gambit.Data.Users;
using Remy.Gambit.Models;
using Remy.Gambit.Services;

namespace Remy.Gambit.Api.Handlers.Credits.Command;

public class CashOutHandler(IUsersRepository usersRepository, ICreditsRepository creditsRepository, IAppSettingsRepository appSettingsRepository,
    IValidator<CashOutRequest> validator, IPartnerService partnerService, IUserLockService userLockService) : ICommandHandler<CashOutRequest, CashOutResult>
{
    public async ValueTask<CashOutResult> HandleAsync(CashOutRequest command, CancellationToken token = default)
    {
        var validationResult = await validator.ValidateAsync(command, token);
        if (!validationResult.IsValid)
        {
            return new CashOutResult { IsSuccessful = false, ValidationResults = validationResult.Errors.Select(x => x.ErrorMessage) };
        }

        try
        {
            var canCashOut = await creditsRepository.CanCashOutAsync(command.UserId, token);
            if (!canCashOut)
            {
                var matchesToCashOut = await appSettingsRepository.GetAppSettingValueAsync<int>(Constants.AppSettings.MatchesToCashOut, token);
                return new CashOutResult { IsSuccessful = false, ValidationResults = [$"You are not yet allowed to cash out. Please wait after {matchesToCashOut} fights."] };
            }

            var lockAcquired = await userLockService.AcquireLockAsync(command.UserId);
            if (!lockAcquired)
            {
                return new CashOutResult { IsSuccessful = false, ValidationResults = ["Failed to cash out."] };
            }

            var user = await usersRepository.GetUserByIdAsync(command.UserId, token);
            if (user is null)
            {
                return new CashOutResult { IsSuccessful = false, ValidationResults = ["Failed to cash out."] };
            }

            if (!user.IsActive)
            {
                return new CashOutResult { IsSuccessful = false, ValidationResults = ["Failed to cash out."] };
            }

            // Credit the amount to the partner
            var transactionId = $"{DateTime.UtcNow:yyyyMMddHHmmss}{TokenHelper.GenerateToken(10)}";
            var tableId = Constants.Config.MarvelGamingTableId;
            var round = await usersRepository.GetLastRoundIdAsync(user.Id, token);
            if(string.IsNullOrEmpty(round))
            {
                round = $"{DateTime.UtcNow:yyyyMMddHHmmss}{TokenHelper.GenerateToken(10)}";
            }

            var currency = await appSettingsRepository.GetAppSettingValueAsync<string>(Constants.AppSettings.Currency, token);

            var cashOutRequest = new Services.Dto.CashOutRequest
            {
                TransactionId = transactionId,
                Token = command.PartnerToken,
                UserName = user.Username,
                Amount = user.Credits.ToString("F2"),
                Currency = currency,
                TableId = tableId,
                Round = round
            };

            var cashOutResult = await partnerService.CashOutAsync(cashOutRequest, token);
            if (!cashOutResult.IsSuccessful)
            {
                return new CashOutResult { IsSuccessful = false, Errors = cashOutResult.Errors! };
            }

            var amount = user.Credits * -1;

            // Deduct the amount from the user
            var deduction = new Credit
            {
                UserId = user.Id,
                Amount = amount,
                IpAddress = command.IpAddress,
            };

            var notes = $"CASH OUT - TransactionId: {transactionId}, TableId: {tableId}, Round: {round}";

            var deductCreditResult = await creditsRepository.CashOutAsync(deduction, notes, token);

            if (!deductCreditResult)
            {
                // Rollback the cash out
                transactionId = $"{DateTime.UtcNow:yyyyMMddHHmmss}{TokenHelper.GenerateToken(10)}";
                round = $"{DateTime.UtcNow:yyyyMMddHHmmss}{TokenHelper.GenerateToken(10)}";

                var cashInRequest = new Services.Dto.CashInRequest
                {
                    TransactionId = transactionId.ToString(),
                    Token = command.PartnerToken,
                    UserName = user.Username,
                    Amount = user.Credits.ToString("F2"),
                    Currency = currency,
                    TableId = tableId,
                    Round = round
                };

                var res = await partnerService.CashInAsync(cashInRequest, token);

                await usersRepository.UpdateLastRoundIdAsync(user.Id, round, token);

                return new CashOutResult { IsSuccessful = false, Errors = res.Errors! };
            }

            return new CashOutResult
            {
                IsSuccessful = true,
                NewBalance = 0,
                Currency = cashOutResult.Currency
            };
        }
        catch
        {
            throw;
        }
        finally
        {
            await userLockService.ReleaseLockAsync(command.UserId);
        }
    }
}
