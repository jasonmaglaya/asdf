namespace Remy.Gambit.Api.Dto
{
    public class Match
    {
        public Guid Id { get; set; }
        public int? Number { get; set; }
        public string? Status { get; set; }
        public IEnumerable<Team>? Teams { get; set; }
        public IEnumerable<string>? Winners { get; set; }
    }
}
