using Remy.Gambit.Core.Data;
using Remy.Gambit.Models;

namespace Remy.Gambit.Data.Credits.DataQueries;

public class CashOutQuery : DataQuery
{
    private readonly string _query = @"
INSERT INTO Credits
(
	Id, UserId, Amount, TransactionDate, TransactionType, TransactedBy, IpAddress
)
VALUES
(
	NEWID(), @UserId, @Amount, GETUTCDATE(), @TransactionType, @UserId, IpAddress
)
";

    public CashOutQuery(Credit credit)
    {
        Parameters.Add("@UserId", credit.UserId);
        Parameters.Add("@Amount", credit.Amount);
        Parameters.Add("@TransactionType", "CASH OUT");
        Parameters.Add("@IpAddress", credit.IpAddress);

        CmdText = _query;
        CmdText = _query;
    }
}
