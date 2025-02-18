using Remy.Gambit.Core.Data;

namespace Remy.Gambit.Data.Users.DataQueries;

public class UpdateAgencyQuery : DataQuery
{
    private readonly string _query = @"
IF EXISTS (SELECT * FROM Agency WHERE UserId = @UserId) 
BEGIN
    UPDATE Agency SET
        AgentCode = @AgentCode,
        Commission = @Commission
    WHERE UserId = @UserId
END
ELSE
BEGIN
    INSERT INTO Agency (UserId, AgentCode, Commission)
    VALUES (@UserId, @AgentCode, @Commission)
END

UPDATE Users SET
    Role = @Role
WHERE Id = @UserId
";

    public UpdateAgencyQuery(Guid userId, string agentCode, double commission, string role)
    {
        CmdText = _query;

        Parameters.Add("UserId", userId);
        Parameters.Add("AgentCode", agentCode);
        Parameters.Add("Commission", commission);
        Parameters.Add("Role", role);
    }
}
