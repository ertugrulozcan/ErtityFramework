using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ErtityFramework.Entities;
using ErtityFramework.Tables;
using ErtityFramework.Tables.MySql;

namespace ErtityFramework.Database.MySql
{
    public class MySqlDatabaseConnector : IDatabaseConnector
    {
        #region Constants



        #endregion

        #region Fields

        private string connectionString;
        private List<ITable> tables = new List<ITable>();
        private ReadOnlyCollection<ITable> tablesReadonlyCollection;

        #endregion

        #region Properties

        private Dictionary<Type, TableBase> TableDictionary { get; set; }

        public ReadOnlyCollection<ITable> Tables
        {
            get
            {
                if (tablesReadonlyCollection == null)
                    tablesReadonlyCollection = new ReadOnlyCollection<ITable>(tables);

                return tablesReadonlyCollection;
            }
        }

        public string Host { get; private set; }

        public int Port { get; private set; }

        public string Username { get; private set; }

        public string Password { get; private set; }

        public string DbName { get; private set; }

        public bool PersistSecurityInfo { get; set; }

        public string Charset { get; set; }

        public string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(this.connectionString))
                    return string.Format("server={0};user id={1};password={2};persistsecurityinfo={3};port={4};database={5};charset={6};",
                                     this.Host, this.Username, this.Password, this.PersistSecurityInfo, this.Port, this.DbName, this.Charset);

                return this.connectionString;
            }

            private set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    connectionString = value;

                    this.SetConnnectionFields(connectionString);
                }
            }
        }

        #endregion

        #region Constructors

        public MySqlDatabaseConnector(string connectionString)
        {
            this.TableDictionary = new Dictionary<Type, TableBase>();
            this.ConnectionString = connectionString;
        }

        public MySqlDatabaseConnector(MySqlConnectionString connStringModel)
        {
            this.TableDictionary = new Dictionary<Type, TableBase>();
            this.ConnectionString = connStringModel.ToString();
        }

        /*
        public MySqlDatabaseConnector(IOptions<MySqlConnectionString> dbConfig)
        {
            this.TableDictionary = new Dictionary<Type, TableBase>();
            this.ConnectionString = dbConfig.Value.ToString();
        }
        */

        #endregion

        #region Methods

        public bool Connect(string host, int port, string username, string password)
        {
            this.Host = host;
            this.Port = port;
            this.Username = username;
            this.Password = password;

            return true;
        }

        public void RegisterTableModel<T>() where T : EntityBase
        {
            if (!this.TableDictionary.ContainsKey(typeof(T)))
            {
                var table = new Table<T>(this.ConnectionString);
                this.TableDictionary.Add(typeof(T), table);
                this.tables.Add(table);
            }
        }

        public Table<T> GetTable<T>() where T : EntityBase
        {
            if (this.TableDictionary.ContainsKey(typeof(T)))
                return this.TableDictionary[typeof(T)] as Table<T>;
            else
                return null;
        }

        private void SetConnnectionFields(string connString)
        {
            var connStringParts = connString.Split(';');

            string serverPart = connStringParts.FirstOrDefault(x => x.StartsWith("server=", StringComparison.InvariantCulture));
            this.Host = MySqlConnectionString.ExtractValue(serverPart);

            string userPart = connStringParts.FirstOrDefault(x => x.StartsWith("user id=", StringComparison.InvariantCulture));
            this.Username = MySqlConnectionString.ExtractValue(userPart);

            string passwordPart = connStringParts.FirstOrDefault(x => x.StartsWith("password=", StringComparison.InvariantCulture));
            this.Password = MySqlConnectionString.ExtractValue(passwordPart);

            string psiPart = connStringParts.FirstOrDefault(x => x.StartsWith("persistsecurityinfo=", StringComparison.InvariantCulture));
            this.PersistSecurityInfo = Convert.ToBoolean(MySqlConnectionString.ExtractValue(psiPart).ToLower());

            string portPart = connStringParts.FirstOrDefault(x => x.StartsWith("port=", StringComparison.InvariantCulture));
            this.Port = Convert.ToInt32(MySqlConnectionString.ExtractValue(portPart));

            string databasePart = connStringParts.FirstOrDefault(x => x.StartsWith("database=", StringComparison.InvariantCulture));
            this.DbName = MySqlConnectionString.ExtractValue(databasePart);

            string charsetPart = connStringParts.FirstOrDefault(x => x.StartsWith("charset=", StringComparison.InvariantCulture));
            this.Charset = MySqlConnectionString.ExtractValue(charsetPart);
        }

        #endregion
    }
}
