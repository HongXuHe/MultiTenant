using MultiTenant.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MultiTenant.Entities
{
    public class Id4Permission:BaseEntity
    {
        public string PermissionName { get; set; }
        public string PermissionValue { get; set; }

        public List<R_User_Permission> Id4Users { get; set; } = new List<R_User_Permission>();
        public List<R_Role_Permission> Id4Roles { get; set; } = new List<R_Role_Permission>();
    }
}
