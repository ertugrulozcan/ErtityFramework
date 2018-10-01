using ErtityFramework.Database;
using ErtityFramework.Database.MySql;
using ErtityFramework.Entities;
using ErtityFramework.Tables;
using ErtityFramework.Tables.MySql;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErtityFramework.Mapping.MySql
{
    public class MappingManager : IMappingManager
    {
        #region Fields

        private List<ITable> tables = new List<ITable>();
        private Dictionary<Type, TableBase> tableDictionary = new Dictionary<Type, TableBase>();

        #endregion

        #region Properties

        private IDatabase<MySqlConnection, MySqlConnectionString> Database { get; set; }

        public ReadOnlyCollection<ITable> Tables { get; private set; }

        public ReadOnlyDictionary<Type, TableBase> TableDictionary { get; private set; }

        #endregion

        #region Constructors

        public MappingManager(IDatabase<MySqlConnection, MySqlConnectionString> db)
        {
            this.Database = db;
            this.Tables = new ReadOnlyCollection<ITable>(this.tables);
            this.TableDictionary = new ReadOnlyDictionary<Type, TableBase>(this.tableDictionary);
        }

        #endregion

        #region Methods

        public void RegisterTableModel<T>() where T : EntityBase
        {
            if (!this.TableDictionary.ContainsKey(typeof(T)))
            {
                var table = new Table<T>(this.Database);
                this.tableDictionary.Add(typeof(T), table);
                this.tables.Add(table);
            }
        }

        #endregion
    }
}
