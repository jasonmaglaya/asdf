using FluentValidation;
using Remy.Gambit.Api.Handlers.Credits.Dto;
using Remy.Gambit.Api.Helpers;
using Remy.Gambit.Core.Concurrency;
using Remy.Gambit.Core.Cqs;
using Remy.Gambit.Data.Credits;
using Remy.Gambit.Data.Users;
using Remy.Gambit.Models;
using Remy.Gambit.Services;

namespace Remy.Gambit.Api.Handlers.Users.Command
{
    public class CashOutHandler(IUsersRepository usersRepository, ICreditsRepository creditsRepository, IValidator<CashOutRequest> validator,
        IPartnerService partnerService, IUserLockService userLockService) : ICommandHandler<CashOutRequest, CashOutResult>
    {
        private readonly IUsersRepository _usersRepository = usersRepository;
        private readonly ICreditsRepository _creditsRepository = creditsRepository;
        private readonly IValidator<CashOutRequest> _validator = validator;
        private readonly IPartnerService _partnerService = partnerService;
        private readonly IUserLockService _userLockService = userLockService;

        public async ValueTask<CashOutResult> HandleAsync(CashOutRequest command, CancellationToken token = default)
        {
            var validationResult = await _validator.ValidateAsync(command, token);
            if (!validationResult.IsValid)
            {
                return new CashOutResult { IsSuccessful = false, ValidationResults = validationResult.Errors.Select(x => x.ErrorMessage) };
            }

            try
            {
                var lockAcquired = await _userLockService.AcquireLockAsync(command.UserId);
                if (!lockAcquired)
                {
                    return new CashOutResult { IsSuccessful = false, ValidationResults = ["Failed to cash out."] };
                }

                var user = await _usersRepository.GetUserByIdAsync(command.UserId, token);
                if (user is null)
                {
                    return new CashOutResult { IsSuccessful = false, ValidationResults = ["Failed to cash out."] };
                }

                if (!user.IsActive)
                {
                    return new CashOutResult { IsSuccessful = false, ValidationResults = ["Failed to cash out."] };
                }

                // Check Current Balance if it is greater than the amount requested
                if (user.Credits < command.Amount)
                {
                    return new CashOutResult { IsSuccessful = false, ValidationResults = ["Amount requested is greater than the current balance."] };
                }

                // Credit the amount to the partner
                var transactionId = $"{DateTime.UtcNow.ToString("yyyyMMddHHmmss")}{TokenHelper.GenerateToken(10)}";
                var tableId = "Infiniti1";
                var round = "Infiniti1-1";

                var cashOutRequest = new Services.Dto.CashOutRequest
                {
                    TransactionId = transactionId,
                    Token = command.PartnerToken,
                    UserName = user.Username,
                    Amount = command.Amount,
                    Currency = command.Currency,
                    TableId = tableId,
                    Round = round
                };

                var cashOutResult = await _partnerService.CashOutAsync(cashOutRequest, token);
                if (!cashOutResult.IsSuccessful)
                {
                    return new CashOutResult { IsSuccessful = false, Errors = cashOutResult.Errors! };
                }

                var amount = command.Amount * -1;

                // Deduct the amount from the user
                var deduction = new Credit
                {
                    UserId = user.Id,
                    Amount = amount
                };

                var notes = $"CASH IN - TableId: {tableId}, Round: {round}";

                var deductCreditResult = await _creditsRepository.CashOutAsync(deduction, notes, token);

                if (!deductCreditResult)
                {
                    // Rollback the cash out
                    transactionId = $"{DateTime.UtcNow.ToString("yyyyMMddHHmmss")}{TokenHelper.GenerateToken(10)}";

                    var cashInRequest = new Services.Dto.CashInRequest
                    {
                        TransactionId = transactionId.ToString(),
                        Token = command.PartnerToken,
                        UserName = user.Username,
                        Amount = command.Amount,
                        Currency = command.Currency,
                        TableId = tableId,
                        Round = round
                    };

                    var res = await _partnerService.CashInAsync(cashInRequest, token);

                    return new CashOutResult { IsSuccessful = false, Errors = res.Errors! };
                }

                return new CashOutResult
                {
                    IsSuccessful = true,
                    NewBalance = cashOutResult.NewBalance + user.Credits,
                    Currency = cashOutResult.Currency
                };
            }
            catch
            {
                throw;
            }
            finally
            {
                await _userLockService.ReleaseLockAsync(command.UserId);
            }
        }
    }
}
