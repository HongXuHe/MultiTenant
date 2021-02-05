using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiTenant.IdentityServerFour.Models.Clients
{
    public class ClientVM
    {
        public string ClientId { get; set; }

        public string ClientName { get; set; }

        public string LogoUri { get; set; }
    }
}
