using Remy.Gambit.Core.Data;
using Remy.Gambit.Models;

namespace Remy.Gambit.Data.Credits.DataQueries;

public class CashInQuery : DataQuery
{
    private readonly string _query = @"
INSERT INTO Credits
(
	Id, UserId, Amount, TransactionDate, TransactionType, TransactedBy
)
VALUES
(
	@Id, @UserId, @Amount, GETUTCDATE(), @TransactionType, @TransactedBy
)
";

    public CashInQuery(Credit credit)
    {
        Parameters.Add("@Id", credit.Id);
        Parameters.Add("@UserId", credit.UserId);
        Parameters.Add("@Amount", credit.Amount);
        Parameters.Add("@TransactionType", "CASH IN");
        Parameters.Add("@TransactedBy", credit.UserId);
        
        CmdText = _query;
    }
}
