using FluentValidation;
using Remy.Gambit.Api.Handlers.Credits.Dto;
using Remy.Gambit.Core.Concurrency;
using Remy.Gambit.Core.Cqs;
using Remy.Gambit.Data.Credits;
using Remy.Gambit.Data.Users;
using Remy.Gambit.Models;
using Remy.Gambit.Services;

namespace Remy.Gambit.Api.Handlers.Credits
{
    public class CashInHandler(IUsersRepository usersRepository, ICreditsRepository creditsRepository, IValidator<CashInRequest> validator,
        IPartnerService partnerService, IUserLockService userLockService) : ICommandHandler<CashInRequest, CashInResult>
    {
        private readonly IUsersRepository _usersRepository = usersRepository;
        private readonly ICreditsRepository _creditsRepository = creditsRepository;
        private readonly IValidator<CashInRequest> _validator = validator;
        private readonly IPartnerService _partnerService = partnerService;
        private readonly IUserLockService _userLockService = userLockService;

        public async ValueTask<CashInResult> HandleAsync(CashInRequest command, CancellationToken token = default)
        {
            var validationResult = await _validator.ValidateAsync(command, token);
            if (!validationResult.IsValid)
            {
                return new CashInResult { IsSuccessful = false, ValidationResults = validationResult.Errors.Select(x => x.ErrorMessage) };
            }

            try
            {
                var lockAcquired = await _userLockService.AcquireLockAsync(command.UserId);
                if (!lockAcquired)
                {
                    return new CashInResult { IsSuccessful = false, ValidationResults = ["Failed to cash in."] };
                }

                var user = await _usersRepository.GetUserByIdAsync(command.UserId, token);
                if (user is null)
                {
                    return new CashInResult { IsSuccessful = false, ValidationResults = ["Failed to cash in."] };
                }

                if (!user.IsActive)
                {
                    return new CashInResult { IsSuccessful = false, ValidationResults = ["Failed to cash in."] };
                }

                // Get Current Balance and check if the user has enough balance
                var balance = await _partnerService.GetBalanceAsync(new Services.Dto.GetBalanceRequest { Token = command.PartnerToken }, token);

                if(balance.Amount < command.Amount)
                {
                    return new CashInResult { IsSuccessful = false, ValidationResults = ["Insufficient balance."] };
                }

                // Deduct the amount from the partner
                var transactionId = Guid.NewGuid();
                var tableId = "Infiniti1";
                var round = "Infiniti1-1";

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

                var cashInResult = await _partnerService.CashInAsync(cashInRequest, token);
                if (!cashInResult.IsSuccessful)
                {
                    return new CashInResult { IsSuccessful = false, Errors = cashInResult.Errors! };
                }

                // Add the amount to the user
                var credit = new Credit
                {
                    Id = transactionId,
                    UserId = user.Id,
                    Amount = command.Amount
                };

                var notes = $"CASH IN - TableId: {tableId}, Round: {round}";

                var addCreditResult =  await _creditsRepository.CashInAsync(credit, notes, token);

                if(!addCreditResult)
                {
                    //Rollback the cash in
                    transactionId = Guid.NewGuid();

                    var cashOutRequest = new Services.Dto.CashOutRequest
                    {
                        TransactionId = transactionId.ToString(),
                        Token = command.PartnerToken,
                        UserName = user.Username,
                        Amount = command.Amount,
                        Currency = command.Currency,
                        TableId = tableId,
                        Round = round
                    };

                    var res = await _partnerService.CashOutAsync(cashOutRequest, token);

                    return new CashInResult { IsSuccessful = false, Errors = res.Errors! };
                }

                return new CashInResult
                {
                    IsSuccessful = true,
                    NewBalance = cashInResult.NewBalance + user.Credits,
                    Currency = cashInResult.Currency
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
