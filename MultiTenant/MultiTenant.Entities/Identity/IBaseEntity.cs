using System;
using System.Collections.Generic;
using System.Text;

namespace MultiTenant.Entities
{
    public interface IBaseEntity
    {
        public Guid Id { get; set; } 
        public DateTimeOffset CreateTime { get; set; }
        public string CreateorId { get; set; }
        public DateTimeOffset? ModifyTime { get; set; }
        public string ModifierId { get; set; }
        public bool SoftDelete { get; set; } 

    }
}
