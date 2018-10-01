using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using ErtityFramework.Entities;
using System.Globalization;
using ErtityFramework.Scheme;
using System.Data.Common;
using ErtityFramework.Data;
using ErtityFramework.Helpers;
using ErtityFramework.Database;
using System.Data.SqlClient;
using System.Data;
using ErtityFramework.Database.MsSql;

namespace ErtityFramework.Tables.MsSql
{
    public class Table<T> : TableBase<T> where T : EntityBase
    {
        #region Properties

        public override string TableName
        {
            get
            {
                TableInfo tableInfo = (TableInfo)Attribute.GetCustomAttribute(typeof(T), typeof(TableInfo));
                if (tableInfo != null)
                    return tableInfo.TableName;

                string typeName = typeof(T).Name;
                return typeName;

                /*
                if (typeName.LastOrDefault() == 'y')
                    return typeName.Substring(0, typeName.Length - 1) + "ies";
                
                return typeName + "s";
                */
            }
        }

        #endregion

        #region Constructors

        public Table(IDatabase<SqlConnection, MsSqlConnectionString> db) : base(db)
        {

        }

        #endregion

        #region Query Methods

        protected override T ExecuteSelect(int id, SqlConnection connection)
        {
            try
            {
                string query = string.Format("SELECT * FROM {0} WHERE Id={1};", this.TableName, id);
                SqlCommand command = new SqlCommand(query, connection);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var annotations = this.GetColumnInfoList();

                    if (reader.Read())
                    {
                        var parameters = new Dictionary<string, object>();

                        foreach (var columnInfo in annotations)
                        {
                            var cellString = $"{reader[columnInfo.ColumnName]}";
                            var cellValue = GenericParser.Parse(cellString, ConvertToPrimitiveType(columnInfo.DbType));

                            parameters.Add(columnInfo.PropertyName, cellValue);
                        }

                        T item = ReflectionHelper.CreateInstance(typeof(T), new object[] { id }) as T;
                        ReflectionHelper.SetProperties(item, parameters);
                        return item;
                    }
                }
            }
            catch (SqlException mysqlEx)
            {
                System.Diagnostics.Debug.WriteLine("Database'e bağlanılamıyor! \n\r" + mysqlEx.StackTrace);
                return null;
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                return null;
            }

            return null;
        }

        protected override List<T> ExecuteSelect(SqlConnection connection)
        {
            List<T> entityList = new List<T>();

            try
            {
                string query = string.Format("SELECT * FROM {0};", this.TableName);
                SqlCommand command = new SqlCommand(query, connection);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var annotations = this.GetColumnInfoList();

                    while (reader.Read())
                    {
                        long id = Int64.Parse($"{reader["Id"]}");
                        var parameters = new Dictionary<string, object>();

                        foreach (var columnInfo in annotations)
                        {
                            try
                            {
                                var cellString = $"{reader[columnInfo.ColumnName]}";

                                var cellValue = GenericParser.Parse(cellString, ConvertToPrimitiveType(columnInfo.DbType));

                                parameters.Add(columnInfo.PropertyName, cellValue);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                        }

                        T item = ReflectionHelper.CreateInstance(typeof(T), new object[] { id }) as T;
                        ReflectionHelper.SetProperties(item, parameters);
                        entityList.Add(item);
                    }
                }
            }
            catch (SqlException mysqlEx)
            {
                System.Diagnostics.Debug.WriteLine("Database'e bağlanılamıyor! \n\r" + mysqlEx.StackTrace);
                return null;
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                return null;
            }

            return entityList;
        }

        protected override T ExecuteInsert(T entity, SqlConnection connection)
        {
            T inserted = null;

            try
            {
                var columnDict = GetColumnInfoDictionary(entity);
                var columnInfos = columnDict.Select(x => x.Key).ToList();

                string query = string.Format(@"INSERT INTO {0} ({1}) VALUES({2});",
                                             this.TableName,
                                             string.Join(", ", columnInfos.Select(x => x.ColumnName)),
                                             string.Join(", ", columnInfos.Select(x => string.Format("?{0}", x.ColumnName))));

                SqlCommand command = new SqlCommand
                {
                    Connection = connection,
                    CommandText = query
                };

                foreach (var pair in columnDict)
                {
                    var cInfo = pair.Key;
                    var parameter = command.Parameters.Add(string.Format("?{0}", cInfo.ColumnName), ConvertToSqlDbType(cInfo.DbType));
                    parameter.Value = pair.Value;
                }

                var lastInsertedIdScalar = command.ExecuteScalar();
                //command.ExecuteNonQuery();

                inserted = ReflectionHelper.CreateInstance<T>(new object[] { Convert.ToInt32(lastInsertedIdScalar) });
                ReflectionHelper.SetProperties(inserted, columnDict.ToDictionary(x => x.Key.PropertyName, x => x.Value));
            }
            catch (SqlException mysqlEx)
            {
                System.Diagnostics.Debug.WriteLine("Database'e bağlanılamıyor! \n\r" + mysqlEx.StackTrace);
                return null;
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                return null;
            }

            return inserted;
        }

        protected override bool ExecuteUpdate(T entity, SqlConnection connection)
        {
            try
            {
                var columnDict = GetColumnInfoDictionary(entity);

                List<string> queryPartList = new List<string>();

                string sqlQuery = @"UPDATE {0} SET ";

                foreach (var pair in columnDict)
                {
                    if (pair.Value != null)
                        queryPartList.Add(string.Format("{0}=?{0}", pair.Key.ColumnName));
                }

                sqlQuery += string.Join(", ", queryPartList);
                sqlQuery += " WHERE Id=?id";

                string query = string.Format(sqlQuery, this.TableName);

                SqlCommand command = new SqlCommand
                {
                    Connection = connection,
                    CommandText = query
                };

                // UserID
                command.Parameters.Add("?id", SqlDbType.Int).Value = entity.Id;

                foreach (var pair in columnDict)
                {
                    if (pair.Value != null)
                        command.Parameters.Add(string.Format("?{0}", pair.Key.ColumnName), ConvertToSqlDbType(pair.Key.DbType)).Value = pair.Value;
                }

                command.ExecuteNonQuery();

                return true;
            }
            catch (SqlException mysqlEx)
            {
                System.Diagnostics.Debug.WriteLine("Database'e bağlanılamıyor! \n\r" + mysqlEx.StackTrace);
                return false;
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                return false;
            }
        }

        protected override bool ExecuteDelete(int id, SqlConnection connection)
        {
            try
            {
                string query = string.Format(@"DELETE FROM {0} " + "WHERE Id=?id", this.TableName);

                SqlCommand command = new SqlCommand
                {
                    Connection = connection,
                    CommandText = query
                };

                command.Parameters.Add("?id", SqlDbType.Int).Value = id;
                command.ExecuteNonQuery();

                return true;
            }
            catch (SqlException mysqlEx)
            {
                System.Diagnostics.Debug.WriteLine("Database'e bağlanılamıyor! \n\r" + mysqlEx.StackTrace);
                return false;
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                return false;
            }
        }

        #endregion

        #region Other Methods

        private List<ColumnInfo> GetColumnInfoList()
        {
            var annotations = new List<ColumnInfo>();

            PropertyInfo[] properties = typeof(T).GetProperties();
            foreach (var prop in properties)
            {
                var customAttributes = prop.GetCustomAttributes();
                var attribute = customAttributes.FirstOrDefault(x => x.GetType().Equals(typeof(ColumnInfo)));
                if (attribute != null)
                {
                    ColumnInfo columnInfoAttr = attribute as ColumnInfo;
                    if (string.IsNullOrEmpty(columnInfoAttr.PropertyName))
                        columnInfoAttr.PropertyName = prop.Name;

                    annotations.Add(columnInfoAttr);
                }
            }

            return annotations;
        }

        private Dictionary<ColumnInfo, object> GetColumnInfoDictionary(object obj)
        {
            var dict = new Dictionary<ColumnInfo, object>();

            PropertyInfo[] properties = typeof(T).GetProperties();
            foreach (var prop in properties)
            {
                var customAttributes = prop.GetCustomAttributes();
                var attribute = customAttributes.FirstOrDefault(x => x.GetType().Equals(typeof(ColumnInfo)));
                if (attribute != null)
                {
                    ColumnInfo columnInfoAttr = attribute as ColumnInfo;
                    if (string.IsNullOrEmpty(columnInfoAttr.PropertyName))
                        columnInfoAttr.PropertyName = prop.Name;

                    dict.Add(columnInfoAttr, prop.GetValue(obj));
                }
            }

            return dict;
        }

        private static Type ConvertToPrimitiveType(DatabaseType dbType)
        {
            switch (dbType)
            {
                case DatabaseType.UNSET:
                    return typeof(string);
                case DatabaseType.TINYINT:
                    return typeof(byte?);
                case DatabaseType.SMALLINT:
                    return typeof(int?);
                case DatabaseType.MEDIUMINT:
                    return typeof(int?);
                case DatabaseType.INT:
                    return typeof(int?);
                case DatabaseType.BIGINT:
                    return typeof(int?);
                case DatabaseType.DOUBLE:
                    return typeof(double?);
                case DatabaseType.DOUBLEPRECISION:
                    return typeof(float?);
                case DatabaseType.REAL:
                    return typeof(float?);
                case DatabaseType.DECIMAL:
                    return typeof(decimal?);
                case DatabaseType.BIT:
                    return typeof(bool?);
                case DatabaseType.SERIAL:
                    return typeof(string);
                case DatabaseType.BOOL:
                    return typeof(bool?);
                case DatabaseType.BOOLEAN:
                    return typeof(bool?);
                case DatabaseType.DEC:
                    return typeof(decimal?);
                case DatabaseType.FIXED:
                    return typeof(int?);
                case DatabaseType.NUMERIC:
                    return typeof(int?);
                case DatabaseType.CHAR:
                    return typeof(char?);
                case DatabaseType.VARCHAR:
                    return typeof(string);
                case DatabaseType.TINYTEXT:
                    return typeof(string);
                case DatabaseType.TEXT:
                    return typeof(string);
                case DatabaseType.MEDIUMTEXT:
                    return typeof(string);
                case DatabaseType.LONGTEXT:
                    return typeof(string);
                case DatabaseType.TINYBLOB:
                    return typeof(string);
                case DatabaseType.MEDIUMBLOB:
                    return typeof(string);
                case DatabaseType.BLOB:
                    return typeof(string);
                case DatabaseType.LONGBLOB:
                    return typeof(string);
                case DatabaseType.BINARY:
                    return typeof(string);
                case DatabaseType.VARBINARY:
                    return typeof(string);
                case DatabaseType.ENUM:
                    return typeof(int?);
                case DatabaseType.SET:
                    return typeof(string);
                case DatabaseType.DATE:
                    return typeof(DateTime?);
                case DatabaseType.DATETIME:
                    return typeof(DateTime?);
                case DatabaseType.TIMESTAMP:
                    return typeof(DateTime?);
                case DatabaseType.TIME:
                    return typeof(DateTime?);
                case DatabaseType.YEAR:
                    return typeof(int?);
                case DatabaseType.GEOMETRY:
                    return typeof(string);
                case DatabaseType.POINT:
                    return typeof(string);
                case DatabaseType.LINESTRING:
                    return typeof(string);
                case DatabaseType.POLYGON:
                    return typeof(string);
                case DatabaseType.MULTIPOINT:
                    return typeof(string);
                case DatabaseType.MULTILINESTRING:
                    return typeof(string);
                case DatabaseType.MULTIPOLYGON:
                    return typeof(string);
                case DatabaseType.GEOMETRYCOLLECTION:
                    return typeof(string);
                default:
                    return typeof(string);
            }
        }

        private static SqlDbType ConvertToSqlDbType(DatabaseType dbType)
        {
            switch (dbType)
            {
                case DatabaseType.UNSET:
                    return SqlDbType.VarChar;
                case DatabaseType.TINYINT:
                    return SqlDbType.TinyInt;
                case DatabaseType.SMALLINT:
                    return SqlDbType.SmallInt;
                case DatabaseType.MEDIUMINT:
                    return SqlDbType.Int;
                case DatabaseType.INT:
                    return SqlDbType.Int;
                case DatabaseType.BIGINT:
                    return SqlDbType.BigInt;
                case DatabaseType.DOUBLE:
                    return SqlDbType.Float;
                case DatabaseType.DOUBLEPRECISION:
                    return SqlDbType.Float;
                case DatabaseType.REAL:
                    return SqlDbType.Real;
                case DatabaseType.DECIMAL:
                    return SqlDbType.Decimal;
                case DatabaseType.BIT:
                    return SqlDbType.Bit;
                case DatabaseType.SERIAL:
                    return SqlDbType.VarChar;
                case DatabaseType.BOOL:
                    return SqlDbType.Bit;
                case DatabaseType.BOOLEAN:
                    return SqlDbType.Bit;
                case DatabaseType.DEC:
                    return SqlDbType.Decimal;
                case DatabaseType.FIXED:
                    return SqlDbType.VarChar;
                case DatabaseType.NUMERIC:
                    return SqlDbType.Decimal;
                case DatabaseType.CHAR:
                    return SqlDbType.VarChar;
                case DatabaseType.VARCHAR:
                    return SqlDbType.VarChar;
                case DatabaseType.TINYTEXT:
                    return SqlDbType.Text;
                case DatabaseType.TEXT:
                    return SqlDbType.Text;
                case DatabaseType.MEDIUMTEXT:
                    return SqlDbType.Text;
                case DatabaseType.LONGTEXT:
                    return SqlDbType.Text;
                case DatabaseType.TINYBLOB:
                    return SqlDbType.VarChar;
                case DatabaseType.MEDIUMBLOB:
                    return SqlDbType.VarChar;
                case DatabaseType.BLOB:
                    return SqlDbType.VarChar;
                case DatabaseType.LONGBLOB:
                    return SqlDbType.VarChar;
                case DatabaseType.BINARY:
                    return SqlDbType.Binary;
                case DatabaseType.VARBINARY:
                    return SqlDbType.VarBinary;
                case DatabaseType.ENUM:
                    return SqlDbType.Int;
                case DatabaseType.SET:
                    return SqlDbType.Text;
                case DatabaseType.DATE:
                    return SqlDbType.Date;
                case DatabaseType.DATETIME:
                    return SqlDbType.DateTime;
                case DatabaseType.TIMESTAMP:
                    return SqlDbType.Timestamp;
                case DatabaseType.TIME:
                    return SqlDbType.Time;
                case DatabaseType.YEAR:
                    return SqlDbType.Date;
                case DatabaseType.GEOMETRY:
                    return SqlDbType.NText;
                case DatabaseType.POINT:
                    return SqlDbType.VarChar;
                case DatabaseType.LINESTRING:
                    return SqlDbType.VarChar;
                case DatabaseType.POLYGON:
                    return SqlDbType.VarChar;
                case DatabaseType.MULTIPOINT:
                    return SqlDbType.VarChar;
                case DatabaseType.MULTILINESTRING:
                    return SqlDbType.VarChar;
                case DatabaseType.MULTIPOLYGON:
                    return SqlDbType.VarChar;
                case DatabaseType.GEOMETRYCOLLECTION:
                    return SqlDbType.NText;
                default:
                    return SqlDbType.VarChar;
            }
        }

        #endregion
    }
}
