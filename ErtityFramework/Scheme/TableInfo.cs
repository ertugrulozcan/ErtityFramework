using System;

namespace ErtityFramework.Scheme
{
    public class TableInfo : System.Attribute
    {
        public string TableName { get; set; }

        public TableInfo(string tableName)
        {
            this.TableName = tableName;
        }
    }
}
