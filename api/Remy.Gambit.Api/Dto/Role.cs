namespace Remy.Gambit.Api.Dto
{
    public class Role
    {
        public required string Code { get; set; }
        public required string Name { get; set; }
        public bool IsAgent { get; set; }
    }
}
