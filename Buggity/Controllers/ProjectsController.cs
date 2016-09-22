using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Buggity.Models;
using Microsoft.AspNet.Identity;
using Buggity.Models.CodeFirst;
using System.Xml.Linq;
using PagedList;


namespace Buggity.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db;
        private Helpers.UserRolesHelper helper;

        public ProjectsController()
        {
            db = new ApplicationDbContext();
            helper = new Helpers.UserRolesHelper(db);
        }
        // GET: Projects
        [Authorize(Roles = "Admin, PM, Submitter,Developer")]
        public ActionResult Index(int? page)
        {
            List<Project> projectslist = new List<Project>();


            string userId = User.Identity.GetUserId();
            bool usrIsAdmiNOrPM = helper.IsUserInAnyRole(userId, "Admin", "PM");


            foreach (var item in db.Project.ToList())
            {
                ApplicationUser usr = db.Users.Find(item.UserId);

                projectslist.Add(new Project
                {
                    Id = item.Id,
                    Title = item.Title,
                    Body = item.Body,
                    //Owner = item.Owner,
                    Created = item.Created,
                    Updated = item.Updated,
                    //Priority =item.Priority,
                    //Status = item.Status,
                    UserId = item.UserId,

                });

            }



            if (usrIsAdmiNOrPM) // if logged in user is admin 
            {


                return View(projectslist.OrderBy(x => x.Id).ToList());
                //all projects
            }
            else
            {
                ApplicationUser usr = db.Users.Find(userId);
                if (usr != null)
                {

                    var dbproject = db.Project.Include(p => p.ApplicationUsers);
                    //var list = projectslist.Where(x => x.UserId == usr.Id)

                    //    .OrderBy(x => x.Id).ToList();

                    string userselected = null;
                    projectslist.Clear();

                    foreach (var item in dbproject)
                    {
                        var sss = item.ApplicationUsers.Where(x => x.Id == usr.Id);

                        foreach (var itemuser in sss)
                        {
                            userselected = itemuser.Id;
                        }

                        if (userselected != null)
                        {
                            projectslist.Add(new Project
                            {
                                Id = item.Id,
                                Title = item.Title,
                                Body = item.Body,
                                //Owner = item.Owner,
                                Created = item.Created,
                                Updated = item.Updated,
                                //Priority =item.Priority,
                                //Status = item.Status,
                                UserId = userselected


                            });

                        }
                    }
                    return View(projectslist);//user's projects.
                }
                else
                    return View();
            }

        }


        // GET: Projects/Details/5
        [Authorize(Roles = "Admin, PM, Developer")]
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Project.Find(id);



            project.ApplicationUsers.Where(x => x.Id == id.ToString());

            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // GET: Projects/Create

        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            // get all users 

            var userlisted = GetApplicationUsersInRole("PM");

            var list = userlisted.OrderBy(r => r.Email).ToList().Select(rr => new SelectListItem { Value = rr.Id.ToString(), Text = rr.Email }).ToList();
            ViewBag.PMUsers = list;
            return View();
        }

        // show all users with specific role
        public IEnumerable<ApplicationUser> GetApplicationUsersInRole(string roleName)
        {
            return from role in db.Roles
                   where role.Name == roleName
                   from userRoles in role.Users
                   join user in db.Users
                   on userRoles.UserId equals user.Id
                   select user;
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        //public ActionResult Create([Bind(Include = "Id,Created,Updated,Title,Status,Priority,Owner,Body,UserId")] Project project)
        public ActionResult Create([Bind(Include = "Id,Title,Status,Priority,Owner,Body,UserId")] Project project)
        {
            if (ModelState.IsValid)
            {
                project.Id = Guid.NewGuid();

                project.Created = DateTimeOffset.Now;
                project.Updated = DateTimeOffset.Now;
                db.Project.Add(project);


                db.SaveChanges();

                return RedirectToAction("Index");
            }

            var userlisted = GetApplicationUsersInRole("PM");

            var list = userlisted.OrderBy(r => r.Email).ToList().Select(rr => new SelectListItem { Value = rr.Id.ToString(), Text = rr.Email }).ToList();
            ViewBag.PMUsers = list;


            return View(project);
        }

        [Authorize(Roles = "Admin")]
        // GET: Projects/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Project.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "Id,Created,Updated,Title,Status,Priority,Owner,Body,UserId")] Project project)
        {
            if (ModelState.IsValid)
            {
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(project);
        }

        // GET: Projects/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Project.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Project project = db.Project.Find(id);
            db.Project.Remove(project);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult _userProjects(Project proj)
        {
            // TODO 
            List<SelectListItem> li = new List<SelectListItem>();


            foreach (var appusr in proj.ApplicationUsers)
            {
                li.Add(new SelectListItem { Text = appusr.FirstName + appusr.LastName, Value = appusr.Id });
            }

            ViewData["ProjAppUsers"] = li;
            return PartialView(proj);
        }

        [Authorize(Roles = "Admin, PM")]
        public ActionResult AddProjectUsers(Project project)
        {

            Project proj = db.Project.Find(project.Id);
            var lstAppUsrs = (from user in db.Users
                              where user.Projects.Count == 0
                              select user).ToList();


            var lstProjUsrs = proj.ApplicationUsers;
            var lstAdminPMRole = (from validRole in db.Roles.ToList()
                                  where validRole.Name.Contains("Admin") || validRole.Name.Contains("PM")
                                  select validRole).ToList();

            var lstAdminPmUsrs = new List<ApplicationUser>();

            foreach (var rolUsrs in lstAdminPMRole)
            {
                foreach (var roleUser in rolUsrs.Users)
                {
                    ApplicationUser roleUserAsAppusr = db.Users.Find(roleUser.UserId);
                    if (roleUserAsAppusr != null)
                    {
                        if (!lstAdminPmUsrs.Contains(roleUserAsAppusr))
                        {
                            lstAdminPmUsrs.Add(roleUserAsAppusr);
                        }
                    }
                }
            }



            var lstUsrNotPrjUsr = lstAppUsrs.Except(lstProjUsrs);
            var lstUsrNotPrjUsr_NoAdminsNorPMs = lstUsrNotPrjUsr.Except(lstAdminPmUsrs).ToList();


            List<SelectListItem> li = new List<SelectListItem>();


            foreach (var appusr in lstUsrNotPrjUsr_NoAdminsNorPMs)
            {
                li.Add(new SelectListItem { Text = appusr.FirstName + appusr.LastName, Value = appusr.Id });
            }

            ViewData["projectId"] = project.Id;
            return View(li);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, PM")]
        public ActionResult AddUserToProject(string AvailableUserId, string projectId)
        {
            Guid projGuid = new Guid(projectId);
            Project proj = db.Project.Find(projGuid);
            if (!string.IsNullOrEmpty(AvailableUserId))//Available value
            {
                Guid userGuid = new Guid(AvailableUserId);


                ApplicationUser usr = db.Users.Find(AvailableUserId);

                if (usr != null)
                {
                    proj.ApplicationUsers.Add(usr);
                    db.SaveChanges();
                }
            }
            return Edit(proj);

        }

        [HttpPost]
        [Authorize(Roles = "Admin, PM")]
        public ActionResult RemoveUserFromProject(string AvailableUserId, string projectId)
        {
            Guid projGuid = new Guid(projectId);
            Project proj = db.Project.Find(projGuid);
            if (!string.IsNullOrEmpty(AvailableUserId))//
            {
                Guid userGuid = new Guid(AvailableUserId);


                ApplicationUser usr = db.Users.Find(AvailableUserId);

                if (usr != null)
                {
                    proj.ApplicationUsers.Remove(usr);
                    db.SaveChanges();
                }
            }
            return Edit(proj);
        }
        //ProjectId = Model.Id, UserId
        public ActionResult _projectTickets(string ProjectId, string UserId)
        {

            Guid projGuid = new Guid(ProjectId);
            Project proj = db.Project.Find(projGuid);
            bool allowEdit = true, allowDetails = true, allowDelete = true, usrIsDev = true;
            if (!string.IsNullOrEmpty(UserId))
            {
                Guid userGuid = new Guid(UserId);

                usrIsDev = helper.IsUserInRole(UserId, "Developer, Submitter");
                /*srIsSub = helper.IsUserInRole(UserId, "Submitter");*/

                if (!helper.IsUserInAnyRole(UserId, "Admin", "PM"))
                {
                    if (usrIsDev)
                        allowDelete = false;
                    else
                    {
                        allowEdit = false; allowDetails = false; allowDelete = false;
                    }
                }

            }


            ViewData["ProjTickets"] = proj.Tickets.ToList();
            ViewData["allowEdit"] = allowEdit;
            ViewData["allowDetails"] = allowDetails;
            ViewData["allowDelete"] = allowDelete;
            ViewData["usrIsDev"] = usrIsDev;
            return PartialView(proj);
        }


        [Authorize(Roles = "Developer")]
        public ActionResult DeveloperDashboard()
        {
            return View();
        }


        [Authorize(Roles = "PM")]
        public ActionResult PmDashboard()
        {
            return View();
        }


        [Authorize(Roles = "Admin")]
        public ActionResult AdminDashboard()
        {
            return View();
        }


    }
}