using IdentityServer4.AspNetIdentity;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MultiTenant.Entities;
using MultiTenant.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MultiTenant.IdentityServerFour.Profiles
{
    public class Id4Profile : IProfileService
    {
        private readonly UserManager<Id4User> _userManager;
        private readonly RoleManager<Id4Role> _roleManager;
        private readonly MultiTenantContext _context;

        public Id4Profile(UserManager<Id4User> userManager, RoleManager<Id4Role> roleManager, MultiTenantContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var user = await _userManager.GetUserAsync(context.Subject);
            if (user != null)
            {
                var claims = new List<Claim>();
                var userRoles = await _userManager.GetRolesAsync(user);
                var roleIds = await _context.Roles.Where(x => userRoles.Contains(x.Name)).Select(x => x.Id).ToListAsync();
                var permissionIds = await _context.Id4Role_Id4Permissions.Where(x => roleIds.Contains(x.Id4RoleId)).Select(x => x.Id4PermissionId).ToListAsync();

                foreach (var p in permissionIds)
                {
                    var permission = await _context.Id4Permissions.SingleOrDefaultAsync(x => x.Id == p);
                    if (!claims.Contains(new Claim(permission.PermissionName, permission.PermissionValue)))
                    {
                        claims.Add(new Claim(permission.PermissionName, permission.PermissionValue));
                    }
                }
                var userPermissions = await _context.Id4User_Id4Permissions
                 .Where(x => x.Id4UserId == user.Id).ToListAsync();
                foreach (var per in userPermissions)
                {
                    var permission = await _context.Id4Permissions.SingleOrDefaultAsync(x => x.Id == per.Id4PermissionId);
                    if (!claims.Contains(new Claim(permission.PermissionName, permission.PermissionValue)))
                    {
                        claims.Add(new Claim(permission.PermissionName, permission.PermissionValue));
                    }
                }
                context.IssuedClaims.AddRange(claims);
            }


        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var user = await _userManager.GetUserAsync(context.Subject);
            context.IsActive = (user != null) && !user.SoftDelete;
        }
    }
}
