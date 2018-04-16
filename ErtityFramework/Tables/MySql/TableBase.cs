using System;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;
using ErtityFramework.Entities;

namespace ErtityFramework.Tables.MySql
{
    public abstract class TableBase
    {

    }

    public abstract class TableBase<T> : TableBase, ITable<T> where T : EntityBase
    {
        #region Constants

        protected readonly string ConnectionString;

        #endregion

        #region Properties

        public abstract string TableName { get; }

        #endregion

        #region Constructors

        protected TableBase(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        #endregion

        #region Abstract Methods

        protected abstract T ExecuteSelect(int id, MySqlConnection connection);
        protected abstract List<T> ExecuteSelect(MySqlConnection connection);
        protected abstract T ExecuteInsert(T entity, MySqlConnection connection);
        protected abstract bool ExecuteUpdate(T entity, MySqlConnection connection);
        protected abstract bool ExecuteDelete(int id, MySqlConnection connection);

        #endregion

        #region Methods

        private R ExecuteQuery<R>(ref MySqlConnection connection, Func<R> function)
        {
            R result = default(R);

            try
            {
                using (connection = new MySqlConnection(this.ConnectionString))
                {
                    connection.Open();
                    result = function();
                    connection.Close();
                }
            }
            catch (MySqlException mysqlEx)
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
            MySqlConnection connection = null;
            return this.ExecuteQuery<T>(ref connection, () => { return this.ExecuteSelect(id, connection); });
        }

        public List<T> Select()
        {
            MySqlConnection connection = null;
            return this.ExecuteQuery<List<T>>(ref connection, () => { return this.ExecuteSelect(connection); });
        }

        public T Insert(T entity)
        {
            MySqlConnection connection = null;
            return this.ExecuteQuery<T>(ref connection, () => { return this.ExecuteInsert(entity, connection); });
        }

        public bool Update(T entity)
        {
            MySqlConnection connection = null;
            return this.ExecuteQuery<bool>(ref connection, () => { return this.ExecuteUpdate(entity, connection); });
        }

        public bool Delete(int id)
        {
            MySqlConnection connection = null;
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
