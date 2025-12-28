using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;

namespace WhiteLagoon.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDBContext _db;
        private readonly DbSet<T> _dbSet;

        public Repository(ApplicationDBContext db)
        {
            _db = db;
            _dbSet = _db.Set<T>();
        }

        private IQueryable<T> GetQuery(Expression<Func<T, bool>> filter, string[] includeProperties)
        {
            IQueryable<T> query = _dbSet;
            if (filter is not null)
            {
                query = query.Where(filter);
            }
            if (includeProperties is not null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            return query;
        }

        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public T Get(Expression<Func<T, bool>> filter = null, string[] includedProperties = null)
        {
            IQueryable<T> query = GetQuery(filter, includedProperties);
            return query.FirstOrDefault();
        }

        public bool Any(Expression<Func<T, bool>> filter)
        {
            return _dbSet.Any(filter);
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>> filter = null, string[] includeProperties = null)
        {
            IQueryable<T> query = GetQuery(filter, includeProperties);
            return query.ToList();
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void SaveChanges()
        {
            _db.SaveChanges();
        }
    }
}
