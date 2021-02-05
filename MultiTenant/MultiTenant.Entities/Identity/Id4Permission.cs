using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MultiTenant.Entities.Identity
{
    public class Id4Permission:IBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string PermissionName { get; set; }
        public string PermissionValue { get; set; }

        public DateTimeOffset CreateTime { get; set; } = DateTimeOffset.UtcNow;
        public string CreateorId { get; set; }
        public DateTimeOffset? ModifyTime { get; set; } = DateTimeOffset.UtcNow;
        public string ModifierId { get; set; }
        public bool SoftDelete { get; set; } = false;

        public List<Id4User_Id4Permission> Id4Users { get; set; } = new List<Id4User_Id4Permission>();
        public List<Id4Role_Id4Permission> Id4Roles { get; set; } = new List<Id4Role_Id4Permission>();
    }
}
