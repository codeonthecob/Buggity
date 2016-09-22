using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Buggity.Models;
using Buggity.Models.CodeFirst;
using Buggity.Models;

namespace Buggity.Helpers
{
    

    public class UserRolesHelper
    {

        private ApplicationDbContext db = new ApplicationDbContext();
        private UserManager<ApplicationUser> userManager;
        private RoleManager<IdentityRole> roleManager;

        public UserRolesHelper(ApplicationDbContext ctx)
        {
            this.userManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(ctx));

            this.roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(ctx));

            this.db = ctx;
        }
 
        public bool IsUserInRole(string userId, string roleName)
        {            
            return userManager.IsInRole(userId, roleName);
        }

        public bool IsUserInAllRoles(string userId, params string[] rolesNames)
        {
            foreach(string roleName in rolesNames)
            {
                if (!userManager.IsInRole(userId, roleName))
                    return false;
            }

            return true;

        }

        public bool IsUserInAnyRole(string userId, params string[] rolesNames)
        {
            foreach (string roleName in rolesNames)
            {
                if (userManager.IsInRole(userId, roleName))
                    return true;
            }

            return false;

        }

        public IList<string> ListUserRoles(string userId)
        {
            return userManager.GetRoles(userId);
            
        }

        public List<ApplicationUser> ListRoleUsers(string roleName)
        {
            List<ApplicationUser> lstRoleUsers = new List<ApplicationUser>();
            IdentityRole role = db.Roles.Where(r => r.Name.Contains(roleName)).FirstOrDefault();
            
            foreach (var user in role.Users)
            {
                ApplicationUser appusr = db.Users.Find(user.UserId);
                lstRoleUsers.Add(appusr);
            }
            
            return lstRoleUsers;
        }

  

        


    }
}