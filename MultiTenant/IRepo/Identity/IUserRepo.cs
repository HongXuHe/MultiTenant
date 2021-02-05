using MultiTenant.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MultiTenant.IRepo.Identity
{
    public interface IUserRepo : IRepoBase<Id4User>
    {
        Id4User FindUserByEmail(string emailAddress);
        Task<Id4User> FindUserByEmailAsync(string emailAddress);
    }
}
