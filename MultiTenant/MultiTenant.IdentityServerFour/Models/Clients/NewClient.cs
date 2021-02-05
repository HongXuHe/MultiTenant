using IdentityServer4.Models;
using MultiTenant.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MultiTenant.IdentityServerFour.Models.Clients
{
    public class NewClient
    {
        [Required]
        public string ClientName { get; set; }

        public string LogoUri { get; set; }

        [Required]
        public string ClientSecrets { get; set; } 

        [Required]
        public MultiTenantGrantTypes AllowedGrantTypes { get; set; } 

        public string ClientHost { get; set; }
    }
}
