using Buggity.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace Buggity.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {  
        ApplicationDbContext context;       
        private Helpers.UserRolesHelper urhelper;

        public RolesController()
        {
            context = new ApplicationDbContext();
            urhelper = new Helpers.UserRolesHelper(context);
        }

        // GET: Roles
        //[Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var Roles = context.Roles.ToList();
            return View(Roles);
        }



        // Create  a New role
        //[Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            var Role = new IdentityRole();
            return View(Role);
        }
        //[Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Create(IdentityRole Role)
        {

            context.Roles.Add(Role);
            context.SaveChanges();

            return RedirectToAction("Index");
        }


        //[Authorize(Roles = "Admin")]
        public ActionResult ManageUserRoles()
        {
            //roles for the view dropdown
            var list = context.Roles.OrderBy(r => r.Name).ToList().Select(rr =>

            new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
            ViewBag.Roles = list;
            return View();
        }


    

    [HttpPost]
        //[Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
    public async Task<ActionResult> RoleAddToUser(string UserName, string RoleName)
    {
        ApplicationUser user = context.Users.Where(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

        // var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
        var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

        if (user.Id != "")
        {
            await UserManager.AddToRoleAsync(user.Id, RoleName);

            ViewBag.ResultMessage = "Role created successfully !";
        }

        else
        {
            ViewBag.ResultMessage = "Error While creating Role  !";
        }

        // roles for the view dropdown
        var list = context.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
        ViewBag.Roles = list;

        return View("ManageUserRoles");
    }


    [HttpPost]
        //[Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
    public ActionResult GetRoles(string UserName)
    {
        if (!string.IsNullOrWhiteSpace(UserName))
        {
            ApplicationUser user = context.Users.Where(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

                List<SelectListItem> list = new List<SelectListItem>();
            if (user!=null)
            { 
                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));


                ViewBag.RolesForThisUser = UserManager.GetRoles(user.Id);

                // roles for the view dropdown
                list = context.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
            }
                ViewBag.Roles = list;
        }

        return View("ManageUserRoles");
    }



    [HttpPost]
        //[Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
    public ActionResult DeleteRoleForUser(string UserName, string RoleName)
    {
        var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

        ApplicationUser user = context.Users.Where(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

        if (urhelper.IsUserInRole(user.Id, RoleName))
        {
            UserManager.RemoveFromRole(user.Id, RoleName);
            ViewBag.ResultMessage = "Role removed from this user successfully!";
        }
        else
        {
            ViewBag.ResultMessage = "This user doesn't belong to selected role!";
        }
        // roles for the view dropdown
        var list = context.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
        ViewBag.Roles = list;

        return View("ManageUserRoles");
    }
  }
}