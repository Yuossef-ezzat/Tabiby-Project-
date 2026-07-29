using DAL.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public interface IUnitOfWork
    {
        IGenaricRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity ,new();
        Task<int> SaveChangesAsync();
    }
}
