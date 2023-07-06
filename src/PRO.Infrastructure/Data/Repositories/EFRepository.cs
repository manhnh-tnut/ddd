using PRO.Domain.Base;
using PRO.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace PRO.Infrastructure.Data.Repositories
{
    public class EFRepository<T> : IEFRepository<T> where T : BaseEntity
    {
        private readonly EFContext _context;

        public IUnitOfWork UnitOfWork => _context;

        public EFRepository(EFContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<T> AddAsync(T entity)
        {
            return (await _context.AddAsync(entity)).Entity;
        }

        public Task<bool> UpdateAsync(T entity)
        {
            entity.Updated = DateTime.Now;
            return Task.FromResult(_context.Update(entity).Entity != null);
        }

        public Task<bool> DeleteAsync(T entity, bool keep = true)
        {
            if (keep)
            {
                entity.Used = false;
                entity.Deleted = true;
                return UpdateAsync(entity);
            }
            return Task.FromResult(_context.Remove(entity).Entity != null);
        }

        public Task<T> GetAsync(Expression<Func<T, bool>> expression)
        {
            return _context.Set<T>().FirstOrDefaultAsync(expression);
        }

        public Task<List<T>> ListAsync(Expression<Func<T, bool>> expression)
        {
            return _context.Set<T>().Where(expression).ToListAsync();
        }
    }
}