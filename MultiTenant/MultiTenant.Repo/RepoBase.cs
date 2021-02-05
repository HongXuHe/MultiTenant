using Microsoft.Extensions.Logging;
using MultiTenant.Entities;
using MultiTenant.IRepo.Identity;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MultiTenant.Repo
{
    public abstract class RepoBase<T> : IRepoBase<T> where T :class, IBaseEntity
    {
        private readonly MultiTenantContext _context;
        private readonly ILogger<RepoBase<T>> _logger;

        public RepoBase(MultiTenantContext context, ILogger<RepoBase<T>> logger)
        {
            _context = context;
            _logger = logger;
            _logger.LogInformation("RepoBase ctor Entered");
        }
        public void Create(T entity)
        {
            _context.Set<T>().Add(entity);
        }

        public async Task CreateAsync(T entity)
        {
           await _context.Set<T>().AddAsync(entity);
        }

        public void Delete(T entity)
        {
            entity.SoftDelete = true;
        }

        public void Delete(Guid id)
        {
            var deleteEntity = _context.Set<T>().Find(id);
            if(deleteEntity != null)
            {
                deleteEntity.SoftDelete = true;
            }
        }

        public async Task DeleteAsync(T entity)
        {
            entity.SoftDelete = true;
        }

        public async Task DeleteAsync(Guid id)
        {
            var deleteEntity = await _context.Set<T>().FindAsync(id);
            if (deleteEntity != null)
            {
                deleteEntity.SoftDelete = true;
            }
        }

        public IQueryable<T> FindAll()
        {
            return _context.Set<T>().Where(x => !x.SoftDelete);
        }

        public IQueryable<T> FindAll(Expression<Func<T, bool>> whereLambda)
        {
            return FindAll().Where(whereLambda);
        }

        public IQueryable<T> FindAll<S>(Expression<Func<T, bool>> whereLambda, Expression<Func<T, S>> orderByAsync, bool IsAsc = true)
        {
            if (IsAsc)
            {
                return FindAll().Where(whereLambda).OrderBy(orderByAsync);
            }
            return FindAll().Where(whereLambda).OrderByDescending(orderByAsync);

        }

        public IQueryable<T> FindPagedData<S>(Expression<Func<T, bool>> whereLambda, Expression<Func<T, S>> orderByAsync, int pageIndex, int pageSize, bool IsAsc = true)
        {
           var list = FindAll().Where(whereLambda).OrderBy(orderByAsync);
            if (!IsAsc)
            {
                list = FindAll().Where(whereLambda).OrderByDescending(orderByAsync);
            }
            return list.Skip((pageIndex - 1) * pageSize).Take(pageSize);
        }

        public void Update()
        {
        }

        public async Task UpdateAsync()
        {
        }
    }
}
