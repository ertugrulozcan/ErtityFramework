using System;
using MySql.Data.MySqlClient;

namespace ErtityFramework.Data
{
    public static class MySqlTypeConverter
    {
        public static MySqlDbType Convert(DatabaseType dbType)
        {
            switch (dbType)
            {
                case DatabaseType.BIGINT:
                    return MySqlDbType.Int64;
                case DatabaseType.BINARY:
                    return MySqlDbType.Binary;
                case DatabaseType.BIT:
                    return MySqlDbType.Bit;
                case DatabaseType.BLOB:
                    return MySqlDbType.Blob;
                case DatabaseType.BOOL:
                case DatabaseType.BOOLEAN:
                    return MySqlDbType.Bit;
                case DatabaseType.CHAR:
                    return MySqlDbType.VarChar;
                case DatabaseType.DATE:
                    return MySqlDbType.Date;
                case DatabaseType.DATETIME:
                    return MySqlDbType.DateTime;
                case DatabaseType.DEC:
                case DatabaseType.DECIMAL:
                    return MySqlDbType.Decimal;
                case DatabaseType.DOUBLE:
                case DatabaseType.DOUBLEPRECISION:
                    return MySqlDbType.Double;
                case DatabaseType.ENUM:
                    return MySqlDbType.Enum;
                case DatabaseType.FIXED:
                    return MySqlDbType.VarChar;
                case DatabaseType.GEOMETRY:
                    return MySqlDbType.Geometry;
                case DatabaseType.GEOMETRYCOLLECTION:
                    return MySqlDbType.VarChar;
                case DatabaseType.INT:
                    return MySqlDbType.Int32;
                case DatabaseType.LINESTRING:
                    return MySqlDbType.VarChar;
                case DatabaseType.LONGBLOB:
                    return MySqlDbType.LongBlob;
                case DatabaseType.LONGTEXT:
                    return MySqlDbType.LongText;
                case DatabaseType.MEDIUMBLOB:
                    return MySqlDbType.MediumBlob;
                case DatabaseType.MEDIUMINT:
                    return MySqlDbType.Int24;
                case DatabaseType.MEDIUMTEXT:
                    return MySqlDbType.MediumText;
                case DatabaseType.MULTILINESTRING:
                    return MySqlDbType.VarString;
                case DatabaseType.MULTIPOINT:
                    return MySqlDbType.VarChar;
                case DatabaseType.MULTIPOLYGON:
                    return MySqlDbType.VarChar;
                case DatabaseType.NUMERIC:
                    return MySqlDbType.Decimal;
                case DatabaseType.POINT:
                    return MySqlDbType.VarChar;
                case DatabaseType.POLYGON:
                    return MySqlDbType.VarChar;
                case DatabaseType.REAL:
                    return MySqlDbType.Float;
                case DatabaseType.SERIAL:
                    return MySqlDbType.VarChar;
                case DatabaseType.SET:
                    return MySqlDbType.Set;
                case DatabaseType.SMALLINT:
                    return MySqlDbType.Int16;
                case DatabaseType.TEXT:
                    return MySqlDbType.Text;
                case DatabaseType.TIME:
                    return MySqlDbType.Time;
                case DatabaseType.TIMESTAMP:
                    return MySqlDbType.Timestamp;
                case DatabaseType.TINYBLOB:
                    return MySqlDbType.TinyBlob;
                case DatabaseType.TINYINT:
                    return MySqlDbType.Int16;
                case DatabaseType.TINYTEXT:
                    return MySqlDbType.TinyText;
                case DatabaseType.VARBINARY:
                    return MySqlDbType.VarBinary;
                case DatabaseType.VARCHAR:
                    return MySqlDbType.VarChar;
                case DatabaseType.YEAR:
                    return MySqlDbType.Year;
                default:
                    return MySqlDbType.VarChar;
            }
        }
    }
}
