using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FestivalFlatform.Data.Repository;

namespace FestivalFlatform.Data.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        public IGenericRepository<T> Repository<T>()
          where T : class;

        int Commit();

        Task<int> CommitAsync();

        FestivalFlatformDbContext GetDbContext();
    }

}
