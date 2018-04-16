using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using ErtityFramework.Entities;
using MySql.Data.MySqlClient;
using System.Globalization;
using ErtityFramework.Scheme;
using System.Data.Common;
using ErtityFramework.Data;
using ErtityFramework.Helpers;

namespace ErtityFramework.Tables.MySql
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

        public Table(string connectionString) : base(connectionString)
        {

        }

        #endregion

        #region Query Methods

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

        protected override T ExecuteSelect(int id, MySqlConnection connection)
        {
            try
            {
                string query = string.Format("SELECT * FROM {0} WHERE Id={1};", this.TableName, id);
                MySqlCommand command = new MySqlCommand(query, connection);

                using (MySqlDataReader reader = command.ExecuteReader())
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
            catch (MySqlException mysqlEx)
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

        protected override List<T> ExecuteSelect(MySqlConnection connection)
        {
            List<T> entityList = new List<T>();

            try
            {
                string query = string.Format("SELECT * FROM {0};", this.TableName);
                MySqlCommand command = new MySqlCommand(query, connection);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    var annotations = this.GetColumnInfoList();

                    while (reader.Read())
                    {
                        long id = Int64.Parse($"{reader["Id"]}");
                        var parameters = new Dictionary<string, object>();

                        foreach (var columnInfo in annotations)
                        {
                            var cellString = $"{reader[columnInfo.ColumnName]}";

                            var cellValue = GenericParser.Parse(cellString, ConvertToPrimitiveType(columnInfo.DbType));

                            parameters.Add(columnInfo.PropertyName, cellValue);
                        }

                        T item = ReflectionHelper.CreateInstance(typeof(T), new object[] { id }) as T;
                        ReflectionHelper.SetProperties(item, parameters);
                        entityList.Add(item);
                    }
                }
            }
            catch (MySqlException mysqlEx)
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

        protected override T ExecuteInsert(T entity, MySqlConnection connection)
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
                
                MySqlCommand command = new MySqlCommand
                {
                    Connection = connection,
                    CommandText = query
                };

                foreach (var pair in columnDict)
                {
                    var cInfo = pair.Key;
                    var parameter = command.Parameters.Add(string.Format("?{0}", cInfo.ColumnName), ConvertToMySqlDbType(cInfo.DbType));
                    parameter.Value = pair.Value;
                }

                command.ExecuteNonQuery();

                inserted = ReflectionHelper.CreateInstance<T>(new object[] { (int)command.LastInsertedId });
                ReflectionHelper.SetProperties(inserted, columnDict.ToDictionary(x => x.Key.PropertyName, x => x.Value));
            }
            catch (MySqlException mysqlEx)
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

        protected override bool ExecuteUpdate(T entity, MySqlConnection connection)
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

                MySqlCommand command = new MySqlCommand
                {
                    Connection = connection,
                    CommandText = query
                };

                // UserID
                command.Parameters.Add("?id", MySqlDbType.Int32).Value = entity.Id;

                foreach (var pair in columnDict)
                {
                    if (pair.Value != null)
                        command.Parameters.Add(string.Format("?{0}", pair.Key.ColumnName), ConvertToMySqlDbType(pair.Key.DbType)).Value = pair.Value;
                }

                command.ExecuteNonQuery();

                return true;
            }
            catch (MySqlException mysqlEx)
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

        protected override bool ExecuteDelete(int id, MySqlConnection connection)
        {
            try
            {
                string query = string.Format(@"DELETE FROM {0} " + "WHERE Id=?id", this.TableName);

                MySqlCommand command = new MySqlCommand
                {
                    Connection = connection,
                    CommandText = query
                };

                command.Parameters.Add("?id", MySqlDbType.Int32).Value = id;
                command.ExecuteNonQuery();

                return true;
            }
            catch (MySqlException mysqlEx)
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

        private static MySqlDbType ConvertToMySqlDbType(DatabaseType dbType)
        {
            switch (dbType)
            {
                case DatabaseType.UNSET:
                    return MySqlDbType.VarChar;
                case DatabaseType.TINYINT:
                    return MySqlDbType.Int16;
                case DatabaseType.SMALLINT:
                    return MySqlDbType.Int16;
                case DatabaseType.MEDIUMINT:
                    return MySqlDbType.Int24;
                case DatabaseType.INT:
                    return MySqlDbType.Int32;
                case DatabaseType.BIGINT:
                    return MySqlDbType.Int64;
                case DatabaseType.DOUBLE:
                    return MySqlDbType.Double;
                case DatabaseType.DOUBLEPRECISION:
                    return MySqlDbType.Double;
                case DatabaseType.REAL:
                    return MySqlDbType.Double;
                case DatabaseType.DECIMAL:
                    return MySqlDbType.Decimal;
                case DatabaseType.BIT:
                    return MySqlDbType.Bit;
                case DatabaseType.SERIAL:
                    return MySqlDbType.VarChar;
                case DatabaseType.BOOL:
                    return MySqlDbType.Bit;
                case DatabaseType.BOOLEAN:
                    return MySqlDbType.Bit;
                case DatabaseType.DEC:
                    return MySqlDbType.Decimal;
                case DatabaseType.FIXED:
                    return MySqlDbType.VarChar;
                case DatabaseType.NUMERIC:
                    return MySqlDbType.NewDecimal;
                case DatabaseType.CHAR:
                    return MySqlDbType.VarChar;
                case DatabaseType.VARCHAR:
                    return MySqlDbType.VarChar;
                case DatabaseType.TINYTEXT:
                    return MySqlDbType.TinyText;
                case DatabaseType.TEXT:
                    return MySqlDbType.Text;
                case DatabaseType.MEDIUMTEXT:
                    return MySqlDbType.MediumText;
                case DatabaseType.LONGTEXT:
                    return MySqlDbType.LongText;
                case DatabaseType.TINYBLOB:
                    return MySqlDbType.TinyBlob;
                case DatabaseType.MEDIUMBLOB:
                    return MySqlDbType.MediumBlob;
                case DatabaseType.BLOB:
                    return MySqlDbType.Blob;
                case DatabaseType.LONGBLOB:
                    return MySqlDbType.LongBlob;
                case DatabaseType.BINARY:
                    return MySqlDbType.Binary;
                case DatabaseType.VARBINARY:
                    return MySqlDbType.VarBinary;
                case DatabaseType.ENUM:
                    return MySqlDbType.Enum;
                case DatabaseType.SET:
                    return MySqlDbType.Set;
                case DatabaseType.DATE:
                    return MySqlDbType.Date;
                case DatabaseType.DATETIME:
                    return MySqlDbType.DateTime;
                case DatabaseType.TIMESTAMP:
                    return MySqlDbType.Timestamp;
                case DatabaseType.TIME:
                    return MySqlDbType.Time;
                case DatabaseType.YEAR:
                    return MySqlDbType.Year;
                case DatabaseType.GEOMETRY:
                    return MySqlDbType.Geometry;
                case DatabaseType.POINT:
                    return MySqlDbType.VarChar;
                case DatabaseType.LINESTRING:
                    return MySqlDbType.VarChar;
                case DatabaseType.POLYGON:
                    return MySqlDbType.VarChar;
                case DatabaseType.MULTIPOINT:
                    return MySqlDbType.VarChar;
                case DatabaseType.MULTILINESTRING:
                    return MySqlDbType.VarChar;
                case DatabaseType.MULTIPOLYGON:
                    return MySqlDbType.VarChar;
                case DatabaseType.GEOMETRYCOLLECTION:
                    return MySqlDbType.Geometry;
                default:
                    return MySqlDbType.VarChar;
            }
        }

        #endregion
    }
}
