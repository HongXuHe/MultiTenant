using System.Collections.Generic;
using IdentityServer4.Models;
using IdentityServer4.Test;
using IdentityServer4;

namespace MultiTenant.IdentityServerFour
{
    public class Config
    {
        //api resources
        public static List<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("api1","API1"){
                    Scopes ={"scope1"}
                }
            };
        }

        public static List<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>{
              new IdentityResources.OpenId(),
              new IdentityResources.Profile()
            };
        }

        public static List<ApiScope> GetApiScopes()
        {
            return new List<ApiScope>
            {
                new ApiScope("scope1")
            };
        }

        public static List<Client> GetClients()
        {
            return new List<Client>
            {
                //client credentials
                new Client()
                {
                    ClientId="clientid",
                    ClientSecrets={new Secret("clientsecret".Sha256())},
                    AllowedGrantTypes=GrantTypes.ClientCredentials,
                    AllowedScopes={"scope1"}
                },
                //username and password
                new Client()
                {
                    ClientId="pwdclient",
                    ClientSecrets={new Secret("clientsecret".Sha256())},
                    AllowedGrantTypes=GrantTypes.ResourceOwnerPassword,
                    AllowedScopes={"scope1"},
                     
                },
                //code
                new Client()
                {
                    ClientId="codeClient",
                    ClientSecrets={new Secret("clientsecret".Sha256())},
                    AllowAccessTokensViaBrowser =true,
                    AllowedGrantTypes=GrantTypes.Code,
                    RedirectUris={"https://localhost:5001/signin-oidc"},
                    PostLogoutRedirectUris={"https://localhost:5001/signout-callback-oidc"},
                    AllowedScopes ={"scope1",IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile}
                }
            };
        }

        public static List<TestUser> GetTestUsers()
        {
            return new List<TestUser>{
                new TestUser()
                {
                 SubjectId="1234",
                 Username="test",
                 Password="test"
                }
            };
        }
    }
}