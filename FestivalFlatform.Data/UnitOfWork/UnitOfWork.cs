using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FestivalFlatform.Data.Repository;

namespace FestivalFlatform.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly FestivalFlatformDbContext _context;

        public UnitOfWork(FestivalFlatformDbContext context)
        {
            _context = context;
        }

        private readonly Dictionary<Type, object> reposotories = new Dictionary<Type, object>();

        public IGenericRepository<T> Repository<T>()
            where T : class
        {
            Type type = typeof(T);
            if (!reposotories.TryGetValue(type, out object value))
            {
                var genericRepos = new GenericRepository<T>(_context);
                reposotories.Add(type, genericRepos);
                return genericRepos;
            }
            return value as GenericRepository<T>;
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public int Commit()
        {
            return _context.SaveChanges();
        }

        public Task<int> CommitAsync() => _context.SaveChangesAsync();

        public FestivalFlatformDbContext GetDbContext()
        {
            return _context;
        }
    }
}
