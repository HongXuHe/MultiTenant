using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MultiTenant.Entities.Identity
{
    public class Id4Role_Id4Permission
    {
        public Guid Id4RoleId { get; set; }
        public Guid Id4PermissionId { get; set; }
        public virtual Id4Role Id4Role { get; set; }
        public virtual Id4Permission Id4Permission { get; set; }
    }
}
