using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiTenant.IdentityServerFour.Models.Id4User
{
    public class UserEditVM
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public List<string> Permissions { get; set; } = new List<string>();
        public List<string> AllRoles { get; set; } = new List<string>();
        public List<string> AllPermissions { get; set; } = new List<string>();
    }
}
