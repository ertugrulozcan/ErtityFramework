using System;
using ErtityFramework.Data;

namespace ErtityFramework.Scheme
{
    public class ColumnInfo : System.Attribute
    {
        private string propertyName;

        public string ColumnName { get; set; }

        public string PropertyName 
        { 
            get
            {
                if (string.IsNullOrEmpty(propertyName))
                    return this.ColumnName;
                
                return propertyName;
            }

            set
            {
                propertyName = value;
            }
        }

        public DatabaseType DbType { get; set; }

        public ColumnInfo(string columnName)
        {
            this.ColumnName = columnName;
        }
    }
}
