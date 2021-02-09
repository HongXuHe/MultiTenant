using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiTenant.IdentityServerFour.Models.Id4Role
{
    public class EditRoleVM
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public List<string> Permissions { get; set; } = new List<string>();
        public List<string> AllPermissions { get; set; } = new List<string>();
    }
}
