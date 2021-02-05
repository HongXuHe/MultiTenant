using MultiTenant.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MultiTenant.IRepo.Identity
{
    public interface IRepoBase<T> where T:IBaseEntity
    {
        /// <summary>
        /// find all the list in db
        /// </summary>
        /// <returns></returns>
         IQueryable<T> FindAll();

        /// <summary>
        /// filter by where condition
        /// </summary>
        /// <param name="whereLambda"></param>
        /// <returns></returns>
         IQueryable<T> FindAll(Expression<Func<T, bool>> whereLambda);

         IQueryable<T> FindAll<S>(Expression<Func<T, bool>> whereLambda, Expression<Func<T, S>> orderByAsync, bool IsAsc=true);

        /// <summary>
        /// find paged data
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="whereLambda"></param>
        /// <param name="orderByAsync"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="IsAsc"></param>
        /// <returns></returns>
         IQueryable<T> FindPagedData<S>(Expression<Func<T, bool>> whereLambda, Expression<Func<T, S>> orderByAsync, int pageIndex,int pageSize, bool IsAsc = true);

        /// <summary>
        /// create entity async
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task CreateAsync(T entity);

        /// <summary>
        /// create entity
        /// </summary>
        /// <param name="entity"></param>
        void Create(T entity);

        /// <summary>
        /// update async
        /// </summary>
        /// <returns></returns>
        Task UpdateAsync();

        /// <summary>
        /// update
        /// </summary>
        void Update();

        /// <summary>
        /// delete entity
        /// </summary>
        /// <param name="entity"></param>
        void Delete(T entity);

        /// <summary>
        /// delete by id
        /// </summary>
        /// <param name="id"></param>
        void Delete(Guid id);

        /// <summary>
        /// delete entity async
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task DeleteAsync(T entity);
        /// <summary>
        /// delete by id async
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteAsync(Guid id);
    }
}
