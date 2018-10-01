using ErtityFramework.Entities;
using ErtityFramework.Mapping;
using ErtityFramework.Mapping.MsSql;
using ErtityFramework.Tables;
using ErtityFramework.Tables.MsSql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbConnector = ErtityFramework.Database.IDatabaseConnector<System.Data.SqlClient.SqlConnection, ErtityFramework.Database.MsSql.MsSqlConnectionString>;

namespace ErtityFramework.Database.MsSql
{
    public class MsSqlDatabase : IDatabase<SqlConnection, MsSqlConnectionString>
    {
        #region Fields

        private ReadOnlyCollection<ITable> tablesReadonlyCollection;

        #endregion

        #region Properties

        public DbConnector Connector { get; private set; }

        public IMappingManager Mapping { get; private set; }

        public ReadOnlyCollection<ITable> Tables
        {
            get
            {
                if (tablesReadonlyCollection == null)
                    tablesReadonlyCollection = new ReadOnlyCollection<ITable>(this.Mapping.Tables);

                return tablesReadonlyCollection;
            }
        }

        #endregion

        #region Constructors

        public MsSqlDatabase(DbConnector databaseConnector)
        {
            this.Connector = databaseConnector;
            this.Mapping = new MappingManager(this);
        }

        #endregion

        #region Methods

        public ITable<T> GetTable<T>() where T : EntityBase
        {
            if (this.Mapping.TableDictionary.ContainsKey(typeof(T)))
                return this.Mapping.TableDictionary[typeof(T)] as Table<T>;
            else
                return null;
        }

        #endregion
    }
}
