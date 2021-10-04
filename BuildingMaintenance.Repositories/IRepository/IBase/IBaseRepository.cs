using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BuildingMaintenance.Repositories.IRepository.IBase
{
    public interface IBaseRepository<T> where T : class
    {
        DbSet<T> DbSet { get; }
        void Add(T entity);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(int id);
        List<T> GetWithoutPredicate();
        List<T> GetWithPredicate(Expression<Func<T, bool>> filter);
        IQueryable<T> GetWithoutPredicateWithIncludes(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includes);
        T GetByID(int id);
    }
}
