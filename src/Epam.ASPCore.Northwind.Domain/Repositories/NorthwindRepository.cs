using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        public TEntity GetByID(int firstId, int secondId)
        {
          return Entities.Find(firstId, secondId);
        }

        public void Insert(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            Entities.Add(entity);
            _context.SaveChanges();
        }

        public async Task Delete(int entityId)
        {
            var entity = GetByID(entityId);
            if (entity == null)
                throw new ArgumentNullException("entity");

            Entities.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int firstId, int secondId)
        {
          var entity = GetByID(firstId, secondId);
          if (entity == null)
            throw new ArgumentNullException("entity");

          Entities.Remove(entity);
          await _context.SaveChangesAsync();
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
