namespace Remy.Gambit.Api.Constants
{
    public static class AgentCodes
    {
        public const string Agent = "AG";
        public const string MasterAgent = "MA";
        public const string SuperMasterAgent = "SMA";
        public const string Incorporator = "INCO";
        public static Dictionary<string, string> DownLines = new() {
            { "MA", "AG" },
            { "SMA", "MA" },
            { "INCO", "SMA" }
        };
    }
}
