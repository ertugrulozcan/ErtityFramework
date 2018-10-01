using ErtityFramework.Entities;
using ErtityFramework.Mapping;
using ErtityFramework.Tables;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErtityFramework.Database
{
    public interface IDatabase
    {
        IMappingManager Mapping { get; }

        ReadOnlyCollection<ITable> Tables { get; }

        ITable<T> GetTable<T>() where T : EntityBase;
    }

    public interface IDatabase<ConnectionType, ConnectionStringType> : IDatabase
        where ConnectionType : DbConnection 
        where ConnectionStringType : IConnectionString
    {
        IDatabaseConnector<ConnectionType, ConnectionStringType> Connector { get; } 
    }
}
