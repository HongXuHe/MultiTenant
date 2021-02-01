using System;
using System.Collections.Generic;
using System.Text;

namespace MultiTenant.Entities.Identity
{
    public class R_Role_Permission:BaseEntity
    {
        public Guid Id4PermissionId { get; set; }
        public Guid Id4RoleId { get; set; }
        public virtual Id4Permission Id4Permission { get; set; }
        public virtual Id4Role Id4Role { get; set; }
    }
}
