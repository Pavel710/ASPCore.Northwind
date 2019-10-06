using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Epam.ASPCore.Northwind.Domain.Repositories
{
    public interface INorthwindRepository<TEntity> where TEntity : class
    {
        IEnumerable<TEntity> Get();
        TEntity GetByID(int entityId);
        void Insert(TEntity entity);
        void Delete(int entityId);
        void Update(TEntity entity);
    }
}
