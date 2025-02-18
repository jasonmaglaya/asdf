using Remy.Gambit.Core.Data;

namespace Remy.Gambit.Data.Users.DataQueries;

public class SignUpQuery : DataQuery
{
    private readonly string _query = @"
DECLARE @Id UNIQUEIDENTIFIER = NEWID()

INSERT INTO
Users (Id, Username, Password, ContactNumber, Role, IsActive)
VALUES (@Id, @Username, @Password, @ContactNumber, @Role, @IsActive)

IF @Upline IS NOT NULL
BEGIN
    INSERT INTO Agency (UserId, AgentCode, Upline)
    VALUES (@Id, @AgentCode, @Upline)
END
";

    public SignUpQuery(string username, string? password, string? contactNumber, string role, Guid? upline, string? agentCode, bool isActive)
    {
        CmdText = _query;

        Parameters.Add("Username", username);
        Parameters.Add("Password", password);
        Parameters.Add("ContactNumber", contactNumber);
        Parameters.Add("Role", role);
        Parameters.Add("Upline", upline);
        Parameters.Add("AgentCode", agentCode);
        Parameters.Add("IsActive", isActive);
    }
}
