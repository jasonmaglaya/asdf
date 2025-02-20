using Remy.Gambit.Core.Data;
using Remy.Gambit.Models;

namespace Remy.Gambit.Data.Credits.DataQueries;

public class CashInQuery : DataQuery
{
    private readonly string _query = @"
INSERT INTO Credits
(
	Id, UserId, Amount, TransactionDate, TransactionType, TransactedBy, Notes
)
VALUES
(
	NEWID(), @UserId, @Amount, GETUTCDATE(), @TransactionType, @TransactedBy, @Notes
)
";

    public CashInQuery(Credit credit, string notes)
    {
        Parameters.Add("@UserId", credit.UserId);
        Parameters.Add("@Amount", credit.Amount);
        Parameters.Add("@TransactionType", "Loading");
        Parameters.Add("@TransactedBy", credit.UserId);
        Parameters.Add("@Notes", notes);

        CmdText = _query;
    }
}
