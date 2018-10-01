using ErtityFramework.Entities;
using ErtityFramework.Mapping;
using ErtityFramework.Mapping.MySql;
using ErtityFramework.Tables;
using ErtityFramework.Tables.MySql;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbConnector = ErtityFramework.Database.IDatabaseConnector<MySql.Data.MySqlClient.MySqlConnection, ErtityFramework.Database.MySql.MySqlConnectionString>;

namespace ErtityFramework.Database.MySql
{
    public class MySqlDatabase : IDatabase<MySqlConnection, MySqlConnectionString>
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

        public MySqlDatabase(DbConnector databaseConnector)
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
