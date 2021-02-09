using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiTenant.IdentityServerFour.Models.Id4Permission
{
    public class EditPermissionVM
    {
        public string PermissionId { get; set; }
        public string PermissionName { get; set; }
        public string PermissionValue { get; set; }
    }
}
