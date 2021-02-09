using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MultiTenant.Entities;
using MultiTenant.Entities.Identity;
using MultiTenant.IdentityServerFour.Models.ApiResource;
using MultiTenant.IdentityServerFour.Models.Clients;
using MultiTenant.IdentityServerFour.Models.Id4Permission;
using MultiTenant.IdentityServerFour.Models.Id4Role;
using MultiTenant.IdentityServerFour.Models.Id4User;
using MultiTenant.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiTenant.IdentityServerFour.Controllers
{
    [Authorize(policy: "AdminRole")]
    public class HomeController : Id4BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly RoleManager<Id4Role> _roleManager;
        private readonly UserManager<Id4User> _userManager;
        private readonly MultiTenantContext _context;
        private readonly ConfigurationDbContext _configurationDb;

        public HomeController(ILogger<HomeController> logger,
                                RoleManager<Id4Role> roleManager,
                                UserManager<Id4User> userManager,
                                MultiTenantContext context,
                               ConfigurationDbContext configurationDb)
        {
            _logger = logger;
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
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
            if (client.AllowedGrantTypes == MultiTenantGrantTypes.Implicit
                || client.AllowedGrantTypes == MultiTenantGrantTypes.Code)
            {
                if (string.IsNullOrEmpty(client.ClientHost.Trim()))
                {
                    _logger.LogInformation("User{0} try to create client {1}, but failed not fill in clienthost", User?.Identity.Name, client.ClientName);
                    ModelState.AddModelError("ClientHost Required", "Please fill in ClientHost");
                    return View();
                }
            }
            if (client.ClientHost != null && client.ClientHost.EndsWith('/'))
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
            _logger.LogInformation($"client {clientToAdd.ClientName} add successfully");
            return RedirectToAction(nameof(ClientList));
        }

        public async Task<IActionResult> ClientDetails(string clientId)
        {
            var client = await _configurationDb.Clients.FirstOrDefaultAsync(c => c.ClientId == clientId);
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

        #region Id4 Users
        public async Task<IActionResult> UserList()
        {

            var users = await _userManager.Users.Select(x => new UserListVM { Id = x.Id.ToString(), Name = x.UserName }).ToListAsync();

            return View(users);
        }

        public async Task<IActionResult> UserDetails(string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);
            var vmUser = new UserViewModel()
            {
                Id = user.Id,
                UserName = user.UserName,
                UserEmail = user.Email,
            };
            vmUser.Roles = (await _userManager.GetRolesAsync(user)).ToList();
            var permissionIds = await _context.Id4User_Id4Permissions.Where(x => x.Id4UserId == user.Id).Select(x => x.Id4PermissionId).ToListAsync();
            foreach (var pId in permissionIds)
            {
                var permission = await _context.Id4Permissions.SingleOrDefaultAsync(x => x.Id == pId);
                vmUser.Permissions.Add(permission.PermissionName);
            }
            return View(vmUser);
        }

        public async Task<IActionResult> EditUser(string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);
            var vmUser = new UserEditVM()
            {
                Id = user.Id,
                UserName = user.UserName,
                UserEmail = user.Email,
            };
            vmUser.Roles = (await _userManager.GetRolesAsync(user)).ToList();
            var permissionIds = await _context.Id4User_Id4Permissions.Where(x => x.Id4UserId == user.Id).Select(x => x.Id4PermissionId).ToListAsync();
            foreach (var pId in permissionIds)
            {
                var permission = await _context.Id4Permissions.SingleOrDefaultAsync(x => x.Id == pId);
                vmUser.Permissions.Add(permission.PermissionName);
            }
            vmUser.AllRoles = await _roleManager.Roles.Select(x => x.Name).ToListAsync();
            vmUser.AllPermissions = await _context.Id4Permissions.Select(x => x.PermissionName).ToListAsync();
            return View(vmUser);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(UserEditVM editVM)
        {
            var userFromDb = await _userManager.Users.Include(x => x.Id4Permissions).FirstOrDefaultAsync(x => x.Id == editVM.Id);
            if (userFromDb != null)
            {
                userFromDb.UserName = editVM.UserName.Trim();
                var existEmailUser = await _userManager.FindByEmailAsync(editVM.UserEmail.Trim());
                var existNameUser = await _userManager.FindByNameAsync(editVM.UserName.Trim());
                if ((existEmailUser != null && existEmailUser.Id != userFromDb.Id) ||
                    ((existNameUser != null && existNameUser.Id != userFromDb.Id))
                    )
                {
                    ModelState.AddModelError("Exist user", "User Name/email already exists");
                    return View(editVM);
                }
                userFromDb.Email = editVM.UserEmail.Trim();
                var permList = new List<Id4User_Id4Permission>();
                foreach (var perm in editVM.Permissions)
                {
                    var permission = await _context.Id4Permissions.Where(x => x.PermissionName == perm).Where(x => !x.SoftDelete).FirstOrDefaultAsync();
                    if (permission != null)
                    {
                        permList.Add(new Id4User_Id4Permission
                        {
                            //  Id4Permission = permission,
                            Id4PermissionId = permission.Id,
                            //  Id4User = userFromDb,
                            Id4UserId = userFromDb.Id
                        });
                    }
                }
                var oldUserRoles = await _userManager.GetRolesAsync(userFromDb);
                var result = await _userManager.RemoveFromRolesAsync(userFromDb, oldUserRoles);
                if (result.Succeeded)
                {
                    await _userManager.AddToRolesAsync(userFromDb, editVM.Roles);
                }
                userFromDb.Id4Permissions = permList;
                if (await _context.SaveChangesAsync() >= 0)
                {
                    return RedirectToAction(nameof(UserList));
                }
            }
            return View();
        }
        #endregion

        #region Id4 Roles

        public async Task<IActionResult> RoleList()
        {
            var roleList = await _roleManager.Roles.Select(x => new RoleListVM { RoleId = x.Id.ToString(), RoleName = x.Name }).ToListAsync();
            return View(roleList);
        }

        public async Task<IActionResult> EditRole(string roleId)
        {
            var roleFromDb = await _roleManager.FindByIdAsync(roleId);
            var vmRole = new EditRoleVM();
            if(roleFromDb == null)
            {
                ModelState.AddModelError("None Exist Role", "None Exist Role");
                return View(roleFromDb);
            }

             vmRole = new EditRoleVM()
            {
                RoleId = roleFromDb.Id.ToString(),
                RoleName = roleFromDb.Name
            };
            var permissionIds = await _context.Id4Role_Id4Permissions.Where(x => x.Id4RoleId == roleFromDb.Id).Select(x => x.Id4PermissionId).ToListAsync();
            foreach (var pId in permissionIds)
            {
                var permission = await _context.Id4Permissions.SingleOrDefaultAsync(x => x.Id == pId);
                vmRole.Permissions.Add(permission.PermissionName);
            }
            vmRole.AllPermissions = await _context.Id4Permissions.Select(x => x.PermissionName).ToListAsync();
            return View(vmRole);
        }
        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleVM editVM)
        {
            var roleFromDb = await _roleManager.Roles.Include(x => x.Id4Permissions).FirstOrDefaultAsync(x => x.Id.ToString() == editVM.RoleId);
            if (roleFromDb != null)
            {
                roleFromDb.Name = editVM.RoleName.Trim();
                var existRole = await _roleManager.FindByNameAsync(editVM.RoleName.Trim());
                if (existRole != null && existRole.Id != roleFromDb.Id)
                {
                    ModelState.AddModelError("Exist Role", "Role already exists");
                    return View(editVM);
                }
                var permList = new List<Id4Role_Id4Permission>();
                foreach (var perm in editVM.Permissions)
                {
                    var permission = await _context.Id4Permissions.Where(x => x.PermissionName == perm).Where(x => !x.SoftDelete).FirstOrDefaultAsync();
                    if (permission != null)
                    {
                        permList.Add(new Id4Role_Id4Permission
                        {
                            Id4PermissionId = permission.Id,
                            Id4RoleId = roleFromDb.Id
                        });
                    }
                }
                roleFromDb.Id4Permissions = permList;
                if (await _context.SaveChangesAsync() >= 0)
                {
                    return RedirectToAction(nameof(RoleList));
                }
            }
            return View();
        }
        public async Task<IActionResult> CreateRole()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateRole(AddRoleVM roleVM)
        {
            var roleExist = await _roleManager.FindByNameAsync(roleVM.RoleName.Trim());
            if (roleExist != null)
            {
                ModelState.AddModelError("RoleExist", "Role already exists");
                return View();
            }
            var res = await _roleManager.CreateAsync(new Id4Role
            {
                Name = roleVM.RoleName.Trim()
            });
            if (res.Succeeded)
            {
                return RedirectToAction(nameof(RoleList));
            }
            ModelState.AddModelError("Create Role Failed", "Create Role Failed");
            return View();

        }

        #endregion

        #region Id4 Permissions

        public async Task<IActionResult> PermissionList()
        {
            var permissionList = await _context.Id4Permissions.Select(x => new PermissionListVM { PermissionId = x.Id.ToString(), PermissionName = x.PermissionName }).ToListAsync();
            return View(permissionList);
        }

        public async Task<IActionResult> EditPermission(string permissionId)
        {
            var permissionFromDb = await _context.Id4Permissions.SingleOrDefaultAsync(x => x.Id.ToString() == permissionId);
            var editModel = new EditPermissionVM();
            if (permissionFromDb != null)
            {
                editModel = new EditPermissionVM
                {
                    PermissionId = permissionFromDb.Id.ToString(),
                    PermissionName = permissionFromDb.PermissionName,
                    PermissionValue = permissionFromDb.PermissionValue
                };
                return View(editModel);
            }
            ModelState.AddModelError("Non Exist Permission", "None Exist Permission");
            return View(editModel);
        }
        [HttpPost]
        public async Task<IActionResult> EditPermission(EditPermissionVM editVM)
        {
            var permissionFromDb = await _context.Id4Permissions.FirstOrDefaultAsync(x => x.Id.ToString() == editVM.PermissionId);
            if (permissionFromDb != null)
            {
                permissionFromDb.PermissionName = editVM.PermissionName.Trim();
                permissionFromDb.PermissionValue = editVM.PermissionValue.Trim();
                var existPermissionByName = await _context.Id4Permissions.SingleOrDefaultAsync(x => x.PermissionName == editVM.PermissionName.Trim());
                var existPermissionByValue = await _context.Id4Permissions.SingleOrDefaultAsync(x => x.PermissionValue == editVM.PermissionValue.Trim());
                if ((existPermissionByName != null && existPermissionByName.Id != permissionFromDb.Id) ||
                    (existPermissionByValue != null && existPermissionByValue.Id != permissionFromDb.Id))
                {
                    ModelState.AddModelError("Exist Permission", "Permission already exists");
                    return View(editVM);
                }
                if (await _context.SaveChangesAsync() >= 0)
                {
                    return RedirectToAction(nameof(PermissionList));
                }
            }
            return View();
        }
        public async Task<IActionResult> CreatePermission()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreatePermission(AddPermissionVM addModel)
        {
            var perNameExist =await _context.Id4Permissions.SingleOrDefaultAsync(x => x.PermissionName == addModel.PermissionName.Trim());
            var perValueExist =await _context.Id4Permissions.SingleOrDefaultAsync(x => x.PermissionValue == addModel.PermissionValue.Trim());
            if (perNameExist != null || perValueExist !=null)
            {
                ModelState.AddModelError("Permission Exist", "Permission already exists");
                return View(addModel);
            }
            _context.Id4Permissions.Add(new Id4Permission
            {
                PermissionName = addModel.PermissionName.Trim(),
                PermissionValue = addModel.PermissionValue.Trim()
            });
            if (await _context.SaveChangesAsync() >=0)
            {
                return RedirectToAction(nameof(PermissionList));
            }
            ModelState.AddModelError("Create Permission Failed", "Create Permission Failed");
            return View(addModel);

        }
        #endregion
    }
}
