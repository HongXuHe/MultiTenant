using System;
using System.Collections.Generic;
using System.Text;

namespace MultiTenant.Shared.Enums
{
    public enum MultiTenantGrantTypes
    {
        ClientCredentials,
        ResourceOwnerPassword,
        Implicit,
        Code
    }
}
