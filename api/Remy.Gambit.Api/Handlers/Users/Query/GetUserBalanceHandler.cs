using FluentValidation;
using Remy.Gambit.Api.Handlers.Users.Query.Dto;
using Remy.Gambit.Core.Cqs;
using Remy.Gambit.Services;

namespace Remy.Gambit.Api.Handlers.Users.Query;

public class GetUserBalanceHandler (IPartnerService partnerService, IValidator<GetUserBalanceRequest> getUserBalanceRequestValidator) : IQueryHandler<GetUserBalanceRequest, GetUserBalanceResult>
{
    private readonly IPartnerService _partnerService = partnerService;
    private readonly IValidator<GetUserBalanceRequest> _getUserBalanceRequestValidator = getUserBalanceRequestValidator;

    public async ValueTask<GetUserBalanceResult> HandleAsync(GetUserBalanceRequest request, CancellationToken token = default)
    {
        var validationResult = await _getUserBalanceRequestValidator.ValidateAsync(request, token);

        if (!validationResult.IsValid)
        {
            return new GetUserBalanceResult
            {
                IsSuccessful = false,
                ValidationResults = validationResult.Errors.Select(x => x.ErrorMessage)
            };
        }

        // TODO: Add validation for user name and user token

        var balance = await _partnerService.GetBalanceAsync(request.UserName!, request.UserToken!, token);

        return new GetUserBalanceResult
        {
            IsSuccessful = true,
            Result = balance
        };  
    }
}
