using BuildingMaintenance.Repositories.IRepository.IBase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BuildingMaintenance.Repositories.Repository.Base
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly DbContext _context;
        public BaseRepository(DbContext dbContext)
        {
            _context = dbContext;
        }
        public DbSet<T> DbSet
        {
            get
            {
                return _context.Set<T>();
            }
        }
        public void Add(T entity)
        {
            try
            {
                _context.Add(entity);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void Delete(int id)
        {
            try
            {
                _context.Remove(id);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<T> GetWithoutPredicate()
        {
            try
            {
                var entity = _context.Set<T>();
                return entity.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IQueryable<T> GetWithoutPredicateWithIncludes(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includes)
        {
            try
            {
                IQueryable<T> entity = _context.Set<T>();
                if (includes != null)
                {
                    entity = includes.Aggregate(entity, (current, include) => current.Include(include));
                }
                if (filter != null)
                {
                    return entity.Where(filter);
                }
                return entity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<T> GetWithPredicate(Expression<Func<T, bool>> filter)
        {
            try
            {
                var entity = _context.Set<T>();
                return (List<T>)entity.Where(filter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public T GetByID(int id)
        {
            try
            {
                var entity = _context.Set<T>();
                return entity.Find(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void Update(T entity)
        {
            try
            {
                DbSet.Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task AddAsync(T entity)
        {
            try
            {
                _context.Add(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }       
    }

}
