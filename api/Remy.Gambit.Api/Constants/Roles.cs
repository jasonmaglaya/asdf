namespace Remy.Gambit.Api.Constants
{
    public static class Roles
    {
        public const string Player = "player";
        public const string Agent = "ag";
        public const string MasterAgent = "ma";
        public const string SuperMasterAgent = "sma";
        public const string Incorporator = "inco";
        public const string Admin = "admin";
        public const string SuperAdmin = "sa";
        public static Dictionary<string, string> DownLines = new() {
            { "ma", "ag" },
            { "sma", "ma" },
            { "inco", "sma" }
        };
    }
}
