using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ErtityFramework.Entities;
using ErtityFramework.Tables;
using ErtityFramework.Tables.MySql;
using MySql.Data.MySqlClient;

namespace ErtityFramework.Database.MySql
{
    public class MySqlDatabaseConnector : IDatabaseConnector<MySqlConnection, MySqlConnectionString>
    {
        #region Fields

        private MySqlConnectionString connectionString;
        
        #endregion

        #region Properties

        public MySqlConnectionString ConnectionString
        {
            get
            {
                return connectionString;
            }

            private set
            {
                this.connectionString = value;
            }
        }

        #endregion

        #region Constructors

        public MySqlDatabaseConnector(string connectionString)
        {
            this.ConnectionString = new MySqlConnectionString(connectionString);
        }

        public MySqlDatabaseConnector(MySqlConnectionString connStringModel)
        {
            this.ConnectionString = connStringModel;
        }

        #endregion

        #region Methods

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(this.ConnectionString.ToString());
        }

        #endregion
    }
}
