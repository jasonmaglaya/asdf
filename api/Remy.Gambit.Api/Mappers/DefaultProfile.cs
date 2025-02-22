using AutoMapper;
using Remy.Gambit.Core.Generics;

namespace Remy.Gambit.Api.Mappers
{
    public class DefaultProfile : Profile
    {
        public DefaultProfile()
        {
            CreateMap<Models.User, Dto.User>();
            CreateMap<PaginatedList<Models.User>, PaginatedList<Dto.User>>();
            CreateMap<PaginatedList<Models.Event>, PaginatedList<Dto.EventListItem>>();
            CreateMap<Models.Event, Dto.EventListItem>();
            CreateMap<Models.Event, Dto.Event>();
            CreateMap<Models.TotalBet, Dto.TotalBet>();
            CreateMap<Models.Team, Dto.Team>();
            CreateMap<Dto.Team, Models.Team>();
            CreateMap<Models.Match, Dto.Match>();
            CreateMap<Models.Role, Dto.Role>();
            CreateMap<Dto.AddEventDto, Models.Event>();
            CreateMap<Dto.UpdateEventDto, Models.Event>();
            CreateMap<Models.Winner, Dto.Winner>();
            CreateMap<Models.Match, Dto.MatchListItem>();
            CreateMap<PaginatedList<Models.Match>, PaginatedList<Dto.MatchListItem>>();
            CreateMap<Models.Balance, Dto.Balance>();
            CreateMap<Models.Credit, Dto.CreditHistoryItem>();
            CreateMap<PaginatedList<Models.Credit>, PaginatedList<Dto.CreditHistoryItem>>();
        }
    }
}
