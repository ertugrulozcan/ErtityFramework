using System;
using System.Collections.Generic;
using ErtityFramework.Entities;

namespace ErtityFramework.Tables
{
    public interface ITable
    {
        string TableName { get; }

        EntityBase Select(int id);
        List<EntityBase> Select();
        EntityBase Insert(EntityBase entity);
        bool Update(EntityBase entity);
        bool Delete(int id);
    }

    internal interface ITable<T> : ITable where T : EntityBase
    {
        new T Select(int id);
        new List<T> Select();
        T Insert(T entity);
        bool Update(T entity);
        new bool Delete(int id);
    }
}
