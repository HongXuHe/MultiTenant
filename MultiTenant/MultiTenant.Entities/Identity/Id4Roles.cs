using MultiTenant.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MultiTenant.Entities
{
    public class Id4Role:BaseEntity
    {
        public string RoleName { get; set; }
        public string RoleDisplayName { get; set; }
        public List<R_User_Role> Id4Users { get; set; } = new List<R_User_Role>();
        public List<R_Role_Permission> Id4Permissions { get; set; } = new List<R_Role_Permission>();
    }
}
