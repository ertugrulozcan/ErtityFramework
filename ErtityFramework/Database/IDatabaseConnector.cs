using System;
using System.Collections.ObjectModel;
using ErtityFramework.Entities;
using ErtityFramework.Tables;
using ErtityFramework.Tables.MySql;

namespace ErtityFramework.Database
{
    public interface IDatabaseConnector
    {
        ReadOnlyCollection<ITable> Tables { get; }

        string Host { get; }

        int Port { get; }

        string Username { get; }

        string Password { get; }

        string DbName { get; }

        bool PersistSecurityInfo { get; set; }

        string Charset { get; set; }

        string ConnectionString { get; }

        bool Connect(string host, int port, string username, string password);

        void RegisterTableModel<T>() where T : EntityBase;

        Table<T> GetTable<T>() where T : EntityBase;
    }
}
