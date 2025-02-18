using Remy.Gambit.Core.Data;

namespace Remy.Gambit.Data.Users.DataQueries;

public class GetDownLinesQuery : DataQuery
{
    private readonly string _query = @"
SELECT U.*, dbo.fx_GetUserCredits(U.Id) Credits
FROM Users U
    JOIN Agency A
        ON U.Id = A.UserId
WHERE A.Upline = @AgentUserId
    AND U.IsDeleted = 0
";

    public GetDownLinesQuery(Guid agentUserId)
    {
        CmdText = _query;

        Parameters.Add("AgentUserId", agentUserId);
    }
}
