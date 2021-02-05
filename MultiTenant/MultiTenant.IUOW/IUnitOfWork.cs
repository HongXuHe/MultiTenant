using MultiTenant.Entities;
using MultiTenant.IRepo.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiTenant.IUOW
{
    public interface IUnitOfWork
    {
        IUserRepo UserRepo {get; }
        void Commit();
        Task CommitAsync();
        void BeginTransaction();
        Task BeginTransactionAsync(CancellationToken cancellationToken = default(CancellationToken));
        void Rollback();
        Task RollbackAsync();
        void UseTransaction(Action action);
        Task UseTransactionAysn(Func<Task> func);
    }
}
