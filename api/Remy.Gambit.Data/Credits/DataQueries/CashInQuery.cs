﻿using Remy.Gambit.Core.Data;
using Remy.Gambit.Models;

namespace Remy.Gambit.Data.Credits.DataQueries;

public class CashInQuery : DataQuery
{
    private readonly string _query = @"
INSERT INTO Credits
(
	Id, UserId, Amount, TransactionDate, TransactionType, TransactedBy, Notes, IpAddress
)
VALUES
(
	NEWID(), @UserId, @Amount, GETUTCDATE(), 'Loading', @UserId, @Notes, @IpAddress
)
";

    public CashInQuery(Credit credit, string notes)
    {
        Parameters.Add("@UserId", credit.UserId);
        Parameters.Add("@Amount", credit.Amount);
        Parameters.Add("@Notes", notes);
        Parameters.Add("@IpAddress", credit.IpAddress);

        CmdText = _query;
    }
}
