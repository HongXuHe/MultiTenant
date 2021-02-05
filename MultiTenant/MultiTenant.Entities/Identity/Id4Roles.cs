using Microsoft.AspNetCore.Identity;
using MultiTenant.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MultiTenant.Entities
{
    public class Id4Role:IdentityRole<Guid>,IBaseEntity
    {
        public DateTimeOffset CreateTime { get; set; } = DateTimeOffset.UtcNow;
        public string CreateorId { get; set; }
        public DateTimeOffset? ModifyTime { get; set; } = DateTimeOffset.UtcNow;
        public string ModifierId { get; set; }
        public bool SoftDelete { get; set; } = false;
        public List<Id4Role_Id4Permission> Id4Permissions { get; set; } = new List<Id4Role_Id4Permission>();
    }
}
