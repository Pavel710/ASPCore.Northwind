using System;
using System.Collections.Generic;
using Epam.ASPCore.Northwind.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Epam.ASPCore.Northwind.Domain.Repositories
{
    public class NorthwindRepository<TEntity> : INorthwindRepository<TEntity> where TEntity : class
    {
        private readonly NorthwindContext _context;
        private DbSet<TEntity> _entities;

        public NorthwindRepository(NorthwindContext context)
        {
            _context = context;
        }

        public IEnumerable<TEntity> Get()
        {
            return Entities;
        }

        public TEntity GetByID(int entityId)
        {
            return Entities.Find(entityId);
        }

        public void Insert(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            Entities.Add(entity);
            _context.SaveChanges();
        }

        public void Delete(int entityId)
        {
            var entity = GetByID(entityId);
            if (entity == null)
                throw new ArgumentNullException("entity");

            Entities.Remove(entity);
            _context.SaveChanges();
        }

        public void Update(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            Entities.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();
        }

        protected DbSet<TEntity> Entities
        {
            get { return _entities ?? (_entities = _context.Set<TEntity>()); }
        }
    }
}
