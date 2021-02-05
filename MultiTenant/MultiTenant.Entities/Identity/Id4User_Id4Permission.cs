using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MultiTenant.Entities.Identity
{
    public class Id4User_Id4Permission
    {
        public Guid Id4UserId { get; set; }
        public Guid Id4PermissionId { get; set; }

        [ForeignKey("Id4UserId")]
        public virtual Id4User Id4User { get; set; }

        [ForeignKey("Id4PermissionId")]
        public virtual Id4Permission Id4Permission { get; set; }
    }
}
