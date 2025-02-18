using Remy.Gambit.Core.Data;

namespace Remy.Gambit.Data.Users.DataQueries;

public class TransferCreditsQuery : DataQuery
{
    private readonly string _query = @"
BEGIN TRANSACTION;

BEGIN TRY
	INSERT INTO Credits(UserId, Amount, TransactionDate, TransactionType, CreditsFrom, TransactedBy, Notes)
    VALUES (@To, @Amount, GETUTCDATE(), 'Loading', COALESCE(@From, @TransactedBy), @TransactedBy, @Notes)

    IF @From IS NOT NULL
    BEGIN
        INSERT INTO Credits(UserId, Amount, TransactionDate, TransactionType, CreditsTo, TransactedBy, Notes)
        VALUES (@From, @Amount * -1, GETUTCDATE(), 'Loading', @To, @TransactedBy, @Notes)
    END
END TRY
BEGIN CATCH    
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;
END CATCH;

IF @@TRANCOUNT > 0
    COMMIT TRANSACTION;
";

    public TransferCreditsQuery(Guid? from, Guid to, decimal amount, Guid transactedBy, string? notes)
    {
        CmdText = _query;

        Parameters.Add("From", from);
        Parameters.Add("To", to);
        Parameters.Add("Amount", amount);
        Parameters.Add("TransactedBy", transactedBy);
        Parameters.Add("Notes", notes);
    }
}
