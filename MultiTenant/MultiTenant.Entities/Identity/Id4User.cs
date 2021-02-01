using MultiTenant.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MultiTenant.Entities
{
    public class Id4User:BaseEntity
    {
        public string UserName { get; set; }
        public string UserPasswordHash { get; set; }
        public string UserEmail { get; set; }
        public string UserPhoneNumber { get; set; }
        public string  UserDepartment { get; set; }
        public List<R_User_Role> Id4Roles { get; set; } = new List<R_User_Role>();
        public List<R_User_Permission> Id4Permissions { get; set; } = new List<R_User_Permission>();
    }
}
