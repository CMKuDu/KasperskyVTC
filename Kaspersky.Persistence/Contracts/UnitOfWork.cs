using Kaspersky.Domain.Common.Contracts;
using Kaspersky.Persistence.Data;
using Kaspersky.Persistence.Repository;

namespace Kaspersky.Persistence.Contracts
{
    public interface IUnitOfWork : IDisposable
    {
        IRefreshTokenRipository RefreshToken { get; }
        int Compele();
    }
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApiDbContext _context;
        public IRefreshTokenRipository RefreshToken { get; private set; }
        public UnitOfWork(ApiDbContext context,
            IRefreshTokenRipository refreshToken)
        {
            RefreshToken = refreshToken;
            _context = context;
        }
        public int Compele()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool dispose)
        {
            if (dispose)
            {
                _context.Dispose();
            }
        }
    }
}
