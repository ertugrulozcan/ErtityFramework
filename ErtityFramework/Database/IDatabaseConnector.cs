using System;
using System.Collections.ObjectModel;
using ErtityFramework.Entities;
using ErtityFramework.Tables;
using ErtityFramework.Tables.MySql;
using ErtityFramework.Database.MySql;
using MySql.Data.MySqlClient;
using System.Data.Common;

namespace ErtityFramework.Database
{
    public interface IDatabaseConnector<ConnectionType, ConnectionStringType> where ConnectionType : DbConnection where ConnectionStringType : IConnectionString
    {
        ConnectionStringType ConnectionString { get; }

        ConnectionType GetConnection();
    }
}
