using System.Text.Json.Serialization;

namespace Remy.Gambit.Api.Dto;

public class UpdateEventDto : SaveEventDto
{
    public Guid Id { get; set; }    
}
