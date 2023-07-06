using PRO.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PRO.Domain.Interfaces
{
    public interface IEFRepository<T> where T : BaseEntity, IAggregateRoot
    {
        IUnitOfWork UnitOfWork { get; }
        Task<T> AddAsync(T entity);

        Task<bool> UpdateAsync(T entity);

        Task<bool> DeleteAsync(T entity, bool keep = true);

        Task<T> GetAsync(Expression<Func<T, bool>> expression);

        Task<List<T>> ListAsync(Expression<Func<T, bool>> expression);
    }
}