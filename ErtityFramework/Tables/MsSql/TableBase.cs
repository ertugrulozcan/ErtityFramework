using System;
using System.Collections.Generic;
using System.Linq;
using ErtityFramework.Entities;
using ErtityFramework.Database;
using System.Data.SqlClient;
using DbAbstraction = ErtityFramework.Database.IDatabase<System.Data.SqlClient.SqlConnection, ErtityFramework.Database.MsSql.MsSqlConnectionString>;

namespace ErtityFramework.Tables.MsSql
{
    public abstract class TableBase<T> : TableBase, ITable<T> where T : EntityBase
    {
        #region Properties

        private DbAbstraction Database { get; set; }

        public abstract string TableName { get; }

        #endregion

        #region Constructors

        protected TableBase(DbAbstraction database)
        {
            Database = database;
        }

        #endregion

        #region Abstract Methods

        protected abstract T ExecuteSelect(int id, SqlConnection connection);

        protected abstract List<T> ExecuteSelect(SqlConnection connection);

        protected abstract T ExecuteInsert(T entity, SqlConnection connection);

        protected abstract bool ExecuteUpdate(T entity, SqlConnection connection);

        protected abstract bool ExecuteDelete(int id, SqlConnection connection);

        #endregion

        #region Methods

        private R ExecuteQuery<R>(ref SqlConnection connection, Func<R> function)
        {
            R result = default(R);

            try
            {
                using (connection = this.Database.Connector.GetConnection())
                {
                    connection.Open();
                    result = function();
                    connection.Close();
                }
            }
            catch (SqlException mysqlEx)
            {
                System.Diagnostics.Debug.WriteLine("Database'e bağlanılamıyor! \n\r" + mysqlEx.StackTrace);
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }

            return result;
        }

        public T Select(int id)
        {
            SqlConnection connection = null;
            return this.ExecuteQuery<T>(ref connection, () => { return this.ExecuteSelect(id, connection); });
        }

        public List<T> Select()
        {
            SqlConnection connection = null;
            return this.ExecuteQuery<List<T>>(ref connection, () => { return this.ExecuteSelect(connection); });
        }

        public T Insert(T entity)
        {
            SqlConnection connection = null;
            return this.ExecuteQuery<T>(ref connection, () => { return this.ExecuteInsert(entity, connection); });
        }

        public bool Update(T entity)
        {
            SqlConnection connection = null;
            return this.ExecuteQuery<bool>(ref connection, () => { return this.ExecuteUpdate(entity, connection); });
        }

        public bool Delete(int id)
        {
            SqlConnection connection = null;
            return this.ExecuteQuery<bool>(ref connection, () => { return this.ExecuteDelete(id, connection); });
        }

        EntityBase ITable.Select(int id)
        {
            return this.Select(id);
        }

        List<EntityBase> ITable.Select()
        {
            var rows = this.Select();
            if (rows == null)
                return null;

            return rows.Cast<EntityBase>().ToList();
        }

        public EntityBase Insert(EntityBase entity)
        {
            return this.Insert(entity);
        }

        public bool Update(EntityBase entity)
        {
            return this.Update(entity);
        }

        #endregion
    }
}
