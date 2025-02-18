using Remy.Gambit.Core.Data;

namespace Remy.Gambit.Data.Users.DataQueries;

public class GetUserByReferralCodeQuery : DataQuery 
{
    private readonly string _query = @"
SELECT *
FROM Users 
WHERE
    ReferralCode = @ReferralCode
";

    public GetUserByReferralCodeQuery(string referralCode)
    {
        CmdText = _query;

        Parameters.Add("ReferralCode", referralCode);
    }
}
