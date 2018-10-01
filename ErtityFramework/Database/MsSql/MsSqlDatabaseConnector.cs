using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ErtityFramework.Entities;
using ErtityFramework.Tables;
using ErtityFramework.Tables.MsSql;
using System.Data.SqlClient;

namespace ErtityFramework.Database.MsSql
{
    public class MsSqlDatabaseConnector : IDatabaseConnector<SqlConnection, MsSqlConnectionString>
    {
        #region Fields

        private MsSqlConnectionString connectionString;

        #endregion

        #region Properties

        public MsSqlConnectionString ConnectionString
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

        public MsSqlDatabaseConnector(string connectionString)
        {
            this.ConnectionString = new MsSqlConnectionString(connectionString);
        }

        public MsSqlDatabaseConnector(MsSqlConnectionString connStringModel)
        {
            this.ConnectionString = connStringModel;
        }

        #endregion

        #region Methods

        public SqlConnection GetConnection()
        {
            return new SqlConnection(this.ConnectionString.ToString());
        }

        #endregion
    }
}
