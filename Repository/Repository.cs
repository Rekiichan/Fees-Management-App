﻿using FeeCollectorApplication.Repository.IRepository;
using FeeCollectorApplication.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FeeCollectorApplication.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        public Repository(ApplicationDbContext db)
        {
            _db = db;
            DbSet = _db.Set<T>();
        }
        internal DbSet<T> DbSet;
        public async Task Add(T entity)
        {
            await DbSet.AddAsync(entity);
        }
            
        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> filter = null)
        {
            IQueryable<T> query = DbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<T>> GetLimitAsync(Expression<Func<T, bool>>? filter = null, int numberLimit = 10)
        {
            if (DbSet.Count() == 0)
            {
                return Enumerable.Empty<T>();
            }
            IQueryable<T> query = DbSet;
            
            if (filter != null)
            {
                query = query.Where(filter).Take(numberLimit);
            }
            return await query.ToListAsync();
        }

        public async Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter, bool tracked = true)
        {

            IQueryable<T> query;
            if (tracked)
            {
                query = DbSet;
            }
            else
            {
                query = DbSet.AsNoTracking();
            }
            return await query.Where(filter).FirstOrDefaultAsync();
        }

        public void Remove(T entity)
        {
            DbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entity)
        {
            DbSet.RemoveRange(entity);
        }
    }
}
