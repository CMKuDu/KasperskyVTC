using Kaspersky.Application.Persistence.Contracts;
using Kaspersky.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Kaspersky.Persistence.Contracts
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly ApiDbContext _context;
        public GenericRepository(ApiDbContext context)
        {
            _context = context;
        }
        public async Task Add(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
        }
        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }
        public async Task<IEnumerable<T>> GetAll()
        {
            var result = await _context.Set<T>().ToListAsync();
            return result;
        }
        public async Task<T> GetById(Guid id)
        {
            var result = await _context.Set<T>().FindAsync(id);
            return result!;
        }
        public async Task<T> GetByStringId(string id)
        {
            var result = await _context.Set<T>().FirstOrDefaultAsync();
            return result!;
        }

        public void Update(T entity)
        {
            _context.Set<T>().Update(entity);
        }
    }
}
