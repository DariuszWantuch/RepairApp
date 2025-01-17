﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RepairApp.Data.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        T Get(int id);
        T Get(string id);

        IEnumerable<T> GetAll(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = null
            );
        T GetFirstOrDefault(
            Expression<Func<T, bool>> filter = null,
            string includeProperties = null
            );

        string GenerateId();
        void Add(T entity);
        void Remove(int id);
        void Remove(T entity);
    }
}
