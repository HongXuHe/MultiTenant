using System;
using System.Collections.Generic;
using System.Text;

namespace MultiTenant.Entities
{
    public class BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTimeOffset CreateTime { get; set; } = DateTimeOffset.UtcNow;
        public string CreateorId { get; set; }
        public DateTimeOffset? ModifyTime { get; set; }
        public string ModifierId { get; set; }
        public bool SoftDelete { get; set; } = false;

    }
}
