using ErtityFramework.Entities;
using ErtityFramework.Tables;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErtityFramework.Mapping
{
    public interface IMappingManager
    {
        ReadOnlyCollection<ITable> Tables { get; }

        ReadOnlyDictionary<Type, TableBase> TableDictionary { get; }

        void RegisterTableModel<T>() where T : EntityBase;
    }
}
