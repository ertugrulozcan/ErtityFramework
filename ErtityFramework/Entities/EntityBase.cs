using System;
using ErtityFramework.Data;
using ErtityFramework.Scheme;

namespace ErtityFramework.Entities
{
    public abstract class EntityBase
    {
        [ColumnInfo("Id", PropertyName = "Id", DbType = DatabaseType.INT)]
        public long Id { get; protected set; }

        protected EntityBase(long id)
        {
            this.Id = id;
        }
    }
}
