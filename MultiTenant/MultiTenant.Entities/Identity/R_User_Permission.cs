using System;
using System.Collections.Generic;
using System.Text;

namespace MultiTenant.Entities.Identity
{
    public class R_User_Permission:BaseEntity
    {
        public Guid Id4UserId { get; set; }
        public Guid Id4PermissionId { get; set; }
        public virtual Id4User Id4User { get; set; }
        public virtual Id4Permission Id4Permission { get; set; }
    }
}
