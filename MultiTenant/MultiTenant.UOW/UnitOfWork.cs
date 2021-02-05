using Microsoft.Extensions.Logging;
using MultiTenant.Entities;
using MultiTenant.IRepo.Identity;
using MultiTenant.IUOW;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiTenant.UOW
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MultiTenantContext _context;
        private readonly IUserRepo _userRepo;
        private readonly ILogger<UnitOfWork> _logger;

        public UnitOfWork(MultiTenantContext context, IUserRepo userRepo, ILogger<UnitOfWork> logger)
        {
            _context = context;
            _userRepo = userRepo;
            _logger = logger;
            _logger.LogInformation("enter ctor of UnitOfWork ");
        }
        public IUserRepo UserRepo
        {
            get
            {
                return _userRepo;
            }
        }

        public void BeginTransaction()
        {
            throw new NotImplementedException();
        }

        public Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void Commit()
        {
            _logger.LogInformation("database commit");
            _context.SaveChanges();
        }

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Rollback()
        {
            
        }

        public Task RollbackAsync()
        {
            throw new NotImplementedException();
        }

        public void UseTransaction(Action action)
        {
            throw new NotImplementedException();
        }

        public Task UseTransactionAysn(Func<Task> func)
        {
            throw new NotImplementedException();
        }
    }
}
