using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MultiTenant.IdentityServerFour.Models.ApiResource;
using MultiTenant.IdentityServerFour.Models.Clients;
using MultiTenant.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiTenant.IdentityServerFour.Controllers
{
    // [Authorize(policy: "AdminRole")]
    public class HomeController : Id4BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ConfigurationDbContext _configurationDb;

        public HomeController(ILogger<HomeController> logger,
                               ConfigurationDbContext configurationDb)
        {
            _logger = logger;
            _configurationDb = configurationDb;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region ApiResources
        public async Task<IActionResult> ApiResouceList()
        {
            _logger.LogInformation("User{0} viewed resource list", User?.Identity.Name);
            var apiResourceList = await _configurationDb.ApiResources.Select(x => new ApiResourceVM { Name = x.Name, DisplayName = x.DisplayName }).ToListAsync();
            return View((object)apiResourceList);
        }

        [HttpGet]
        public IActionResult CreateApiResouce()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateApiResouce(CreateAPIResource model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogInformation("User{0} try to create resource{1}, but failed", User?.Identity.Name, model.Name);
                ModelState.AddModelError("Fields Required", "Please fill in all the required fields");
                return View();
            }
            var existsApiResource = await _configurationDb.ApiResources.SingleOrDefaultAsync(x => x.Name == model.Name.Trim());
            if (existsApiResource != null)
            {
                _logger.LogInformation("User{0} try to create resource{1}, but failed", User?.Identity.Name, model.Name);
                ModelState.AddModelError("ApiResource Exists", "Api Resource already exists");
                return View();
            }
            var apiResource = new ApiResource()
            {
                Name = model.Name.Trim(),
                DisplayName = model.DisplayName.Trim(),
                Scopes = { "scope1" }
            };
            _configurationDb.ApiResources.Add(apiResource.ToEntity());
            await _configurationDb.SaveChangesAsync();
            _logger.LogInformation("User{0}  created resource{1} success", User?.Identity.Name, model.Name);
            return RedirectToAction(nameof(ApiResouceList));
        }
        #endregion

        #region Clients

        public async Task<IActionResult> ClientList()
        {
            _logger.LogInformation("User{0} viewed client list", User?.Identity.Name);
            var clientList = await _configurationDb.Clients.Select(x => new ClientVM { ClientId = x.ClientId, ClientName = x.ClientName, LogoUri = x.LogoUri }).ToListAsync();
            return View((object)clientList);
        }

        public IActionResult CreateClient()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateClient(NewClient client)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogInformation("User{0} try to create client {1}, but failed", User?.Identity.Name, client.ClientName);
                ModelState.AddModelError("Fields Required", "Please fill in all the required fields");
                return View();
            }
            if(client.AllowedGrantTypes == MultiTenantGrantTypes.Implicit
                || client.AllowedGrantTypes == MultiTenantGrantTypes.Code)
            {
                if (string.IsNullOrEmpty(client.ClientHost.Trim())){
                    ModelState.AddModelError("ClientHost Required", "Please fill in ClientHost");
                    return View();
                }
            }
            if (client.ClientHost !=null &&client.ClientHost.EndsWith('/'))
            {
                ModelState.AddModelError("ClientHost Cannot end with \'/\'", "ClientHost Cannot end with \'/\'");
                return View();
            }
            var existsclient = await _configurationDb.Clients.SingleOrDefaultAsync(x => x.ClientName == client.ClientName.Trim());
            if (existsclient != null)
            {
                _logger.LogInformation("User{0} try to create client {1}, but failed", User?.Identity.Name, client.ClientName.Trim());
                ModelState.AddModelError("client Exists", "Api client already exists");
                return View();
            }
            var clientToAdd = new Client()
            {
                ClientId = Guid.NewGuid().ToString(),
                ClientName = client.ClientName,
                ClientUri = client.ClientHost,
            };
            var secretList = new List<Secret>();
            secretList.Add(new Secret(client.ClientSecrets.Sha256()));
            clientToAdd.ClientSecrets = secretList;
            clientToAdd.LogoUri = client.LogoUri;
            switch (client.AllowedGrantTypes)
            {
                case MultiTenantGrantTypes.ClientCredentials:
                    clientToAdd.AllowedGrantTypes = GrantTypes.ClientCredentials;
                    clientToAdd.AllowedScopes = new List<string> { "scope1" };
                    break;
                case MultiTenantGrantTypes.ResourceOwnerPassword:
                    clientToAdd.AllowedGrantTypes = GrantTypes.ResourceOwnerPassword;
                    clientToAdd.AllowedScopes = new List<string> { "scope1" };
                    break;
                case MultiTenantGrantTypes.Implicit:
                    clientToAdd.AllowAccessTokensViaBrowser = true;
                    clientToAdd.AllowedGrantTypes = GrantTypes.Implicit;
                    clientToAdd.AllowedScopes = new List<string> { "scope1",
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile};
                    clientToAdd.RedirectUris = new List<string> { $"{client.ClientHost}/signin-oidc" };
                    clientToAdd.PostLogoutRedirectUris = new List<string>
                    {
                        $"{client.ClientHost}/signout-callback-oidc"
                    };
                    break;
                case MultiTenantGrantTypes.Code:
                    clientToAdd.AlwaysSendClientClaims = true;
                    clientToAdd.AllowAccessTokensViaBrowser = true;
                    clientToAdd.AllowedGrantTypes = GrantTypes.Code;
                    clientToAdd.AllowedScopes = new List<string> { "scope1",
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile};
                    clientToAdd.RedirectUris = new List<string> { $"{client.ClientHost}/signin-oidc" };
                    clientToAdd.PostLogoutRedirectUris = new List<string>
                    {
                        $"{client.ClientHost}/signout-callback-oidc"
                    };
                    break;
                default:
                    clientToAdd.AllowedGrantTypes = GrantTypes.ClientCredentials;
                    break;
            }
            _configurationDb.Clients.Add(clientToAdd.ToEntity());
            await _configurationDb.SaveChangesAsync();
            return RedirectToAction(nameof(ClientList));
        }

        public async Task<IActionResult> ClientDetails(string clientId)
        {
            var client =await _configurationDb.Clients.FirstOrDefaultAsync(c => c.ClientId == clientId);
            if (client == null)
            {
                RedirectToAction(nameof(ClientList));
            }
            var clientVm = new ClientVM
            {
                ClientId = client.ClientId,
                ClientName = client.ClientName,
                LogoUri = client.LogoUri
            };
            return View(clientVm);
        }

        #endregion
    }
}
