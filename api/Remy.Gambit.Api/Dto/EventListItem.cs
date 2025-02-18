namespace Remy.Gambit.Api.Dto
{
    public class EventListItem
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? SubTitle { get; set; }
        public DateTime? EventDate { get; set; }
        public string? Status { get; set; }
    }
}
