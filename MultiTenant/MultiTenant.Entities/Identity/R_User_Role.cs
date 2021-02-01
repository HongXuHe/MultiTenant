using System;
using System.Collections.Generic;
using System.Text;

namespace MultiTenant.Entities.Identity
{
    public class R_User_Role:BaseEntity
    {
        public Guid Id4UserId { get; set; }
        public Guid Id4RoleId { get; set; }
        public virtual Id4User Id4User { get; set; }
        public virtual Id4Role Id4Role { get; set; }
    }
}
