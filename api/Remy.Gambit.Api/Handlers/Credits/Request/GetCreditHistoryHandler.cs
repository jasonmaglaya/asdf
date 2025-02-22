using AutoMapper;
using FluentValidation;
using Remy.Gambit.Api.Dto;
using Remy.Gambit.Api.Handlers.Credits.Request.Dto;
using Remy.Gambit.Core.Cqs;
using Remy.Gambit.Core.Generics;
using Remy.Gambit.Data.Credits;
using Remy.Gambit.Models;

namespace Remy.Gambit.Api.Handlers.Credits.Request;

public class GetCreditHistoryHandler(
    ICreditsRepository creditsRepository,
    IValidator<GetCreditHistoryRequest> getCreditHistoryRequestValidator, 
    IMapper mapper) : IQueryHandler<GetCreditHistoryRequest, GetCreditHistoryResult>
{
    private readonly ICreditsRepository _creditsRepository = creditsRepository;
    private readonly IValidator<GetCreditHistoryRequest> _getCreditHistoryRequestValidator = getCreditHistoryRequestValidator;
    private readonly IMapper _mapper = mapper;

    public async ValueTask<GetCreditHistoryResult> HandleAsync(GetCreditHistoryRequest request, CancellationToken token = default)
    {
        var validationResult = await _getCreditHistoryRequestValidator.ValidateAsync(request, token);

        if (!validationResult.IsValid)
        {
            return new GetCreditHistoryResult
            {
                IsSuccessful = false,
                ValidationResults = validationResult.Errors.Select(x => x.ErrorMessage)
            };
        }

        var result = await _creditsRepository.GetHistoryAsync(request.UserId, request.PageNumber, request.PageSize, token);

        return new GetCreditHistoryResult
        {
            IsSuccessful = true,
            Result = _mapper.Map<PaginatedList<Credit>, PaginatedList<CreditHistoryItem>>(result)
        };
    }
}
