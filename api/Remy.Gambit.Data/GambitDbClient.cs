using Remy.Gambit.Core.Data;
using System.Data;

namespace Remy.Gambit.Data;

public class GambitDbClient : DataClient, IGambitDbClient
{
    public GambitDbClient(IDbConnection dbConnection) : base(dbConnection)
    {

    }
}
