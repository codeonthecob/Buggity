using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Buggity.Models;
using Microsoft.AspNet.Identity;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using Buggity.Models.CodeFirst;
using System.Xml.Linq;

namespace Buggity.Controllers
{

    public class TicketsController : Controller
    {



        private ApplicationDbContext db;
        private Helpers.UserRolesHelper urhelper;
        private Helpers.UserTicketsHelper uthelper; 


        public TicketsController()
        {
            db = new ApplicationDbContext();
            urhelper = new Helpers.UserRolesHelper(db);
            uthelper = new Helpers.UserTicketsHelper();

        }




        // GET: Tickets

        [Authorize]
        public ActionResult Index()
        {
            // var ticket = db.Ticket.Include(t => t.Project);


            //join user in db.Users
            //     on userRoles.UserId equals user.Id
            //     select user;

            var allusers = db.Users.ToList();


                var ticket = from e in db.Ticket
                         join a in db.TicketStatuses on e.TicketStatusId equals a.Id
                         join c in db.TicketTypes on e.TicketTypeId equals c.Id
                        
                         select new TicketViewModel()
                         {
                             Id = e.Id,
                             Title = e.Title,
                             ProjectName = e.Project.Title,
                             Description = e.Description,
                             Created = e.Created,
                             Updated = e.Updated,
                             TicketPriority = e.TicketPriorities.Name,
                             TicketStatus = a.Name,
                             TicketType = c.Name,
                             CreatedBy = e.CreatedBy,
                             AssigneeId = e.AssigneeId

                         };




            string UserId = User.Identity.GetUserId();
            Guid user = new Guid(UserId);
            ApplicationUser ap = db.Users.Find(UserId);
            bool usrIsAdmiNOrPM = urhelper.IsUserInAnyRole(UserId, "Admin", "PM");
            bool usrIsDev = urhelper.IsUserInRole(UserId, "Developer");

            if (usrIsAdmiNOrPM)
            {



                //tickets of projects whose project id exits in tickets tbl
                //var usrprojtik = (from tik in ticket
                //                  join proj in ap.Projects on new { tik.Project.Id } equals new { proj.Id }
                //                  select tik).ToList();
                //return View(usrprojtik);

                return View(ticket.OrderBy(x => x.Id).ToList());
            }
            else
            {
                if (usrIsDev)
                {
                    var devassgnedtik = (from tik in ticket
                                         where tik.AssigneeId == ap.Email
                                         select tik).OrderBy(x => x.Id).ToList();

                    return View(devassgnedtik);
                }
                else
                {
                    var submittertik = (from tik in ticket
                                        where tik.CreatedBy == ap.Email
                                        select tik).OrderBy(x => x.Id).ToList();

                    return View(submittertik);
                }
            }


        }

       
        // GET: Tickets/Details/5
        public ActionResult Details(Guid? id, bool? usrIsDev)
        {
            if (id == null)
            {

                id = new Guid(TempData["ticketid"].ToString());

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

            }
            Ticket ticket = db.Ticket.Find(id);
            List<ApplicationUser> lstAppUsrs = uthelper.GetAssgedUsrs(ticket.AssigneeId);
            List<SelectListItem> lstSLAppUsrs = lstAppUsrs
                    .Select(
                        appusr => new SelectListItem
                        {
                            Value = appusr.Id,
                            Text = (string.IsNullOrEmpty(appusr.FirstName) ? "" : appusr.FirstName) + " " + (string.IsNullOrEmpty(appusr.LastName) ? "" : appusr.LastName)
                        }).ToList();

            ViewData["TicketAssgedUsrs"] = lstSLAppUsrs;


            //This breaks as of now due to weird EntittyFramework error checking for wrong table name on database
            /*ViewData["TicketAssgedUsrs"] = ticket.ApplicationUsers.ToList()
                    .Select(rr => new SelectListItem { Value = rr.Id, Text = rr.FirstName + ' ' +rr.LastName })
                    .ToList();*/


            if (ticket == null)
            {
                return HttpNotFound();
            }


            var ticketattachment = db.TicketAttachments.Where(x => x.TicketId == ticket.Id).ToList();



            if (ticketattachment == null)
            {

            }

            var ticketcomments = db.Comments.Where(x => x.TicketId == ticket.Id).ToList();



            if (ticketcomments == null)
            {

            }

            var tickethistory = db.Histories.Where(x => x.TicketId == ticket.Id).ToList();

            if (tickethistory == null)
            {
            }


            var tuple = new Tuple<Ticket, IEnumerable<TicketAttachment>, Project,IEnumerable<Comment>>(ticket, ticketattachment, new Project(),ticketcomments);

            return View("Details", tuple);
        }



        public ActionResult _tickethistory(Guid id)
        {

            var list = db.Histories.Where(x => x.TicketId == id).ToList();
            return View(list);

        }

        // GET: Tickets/Create
        [Authorize(Roles = "Submitter")]
        public ActionResult Create()
        {
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name");
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name");
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name");


            List<Project> projectslist = new List<Project>();

            string userId = User.Identity.GetUserId();
           
          //  bool usrIsDev = urhelper.IsUserInRole(userId, "Submitter");


            //foreach (var item in db.Project.ToList())
            //{
            //    ApplicationUser usr = db.Users.Find(item.UserId);

            //    projectslist.Add(new Project
            //    {
            //        Id = item.Id,
            //        Title = item.Title,
            //        Body = item.Body,
            //        Owner = item.Owner,
            //        Created = item.Created,
            //        Updated = item.Updated,
            //        Priority = item.Priority,
            //        Status = item.Status,
            //        UserId = usr.Email,
            //    });

             
         
            //}

            ApplicationUser idusr = db.Users.Find(userId);

           

            //foreach (var item in db.Project.ToList())
            //{
            //    Project proj = db.Project.Find(item.Id);

            //    proj.ApplicationUsers.Where(x => x.Id == userId);

               


            //    projectslist.Add(proj);

               
            //}
           
           


            ViewBag.ProjectId = new SelectList(idusr.Projects, "Id", "Title");
            ViewData["userName"] = User.Identity.IsAuthenticated ? db.Users.Find(User.Identity.GetUserId()).FirstName : "";
            Ticket tik = new Ticket();

            return View(tik);
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Submitter")]
       
        public ActionResult Create(Ticket ticket, HttpPostedFileBase fileUpload)
        {
            History history = new History();
            string pathurl = null;
            if (ModelState.IsValid)
            {


                if (fileUpload != null && fileUpload.ContentLength > 0)
                {
                    try
                    {
                        pathurl = DateTime.Now.Millisecond.ToString() + fileUpload.FileName;

                        string path = Path.Combine(Server.MapPath("~/TicketImages"),
                                                   Path.GetFileName(pathurl));

                        fileUpload.SaveAs(path);
                        ViewBag.ImageURL = path;

                        ViewBag.Message = "File uploaded successfully";

                        ticket.PictureUrl = pathurl;
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Message = "ERROR:" + ex.Message.ToString();
                    }
                }


                ticket.Id = Guid.NewGuid();
                //ticket.AssigneeId = db.Users.Find(User.Identity.GetUserId()).FirstName;
                ticket.Created = DateTimeOffset.Now;
                ticket.CreatedBy = db.Users.Find(User.Identity.GetUserId()).Email;
                history.TicketId = ticket.Id;
                db.Histories.Add(history);
                db.Ticket.Add(ticket);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProjectId = new SelectList(db.Project, "Id", "Title", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name");


            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name");
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name");
            return View(ticket);
        }

        // GET: Tickets/Edit/5
       // [Authorize(Roles = "Admin, PM, Developer")]
        public ActionResult Edit(Guid? id)
        {


            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


            string UserId = User.Identity.GetUserId();
            Guid user = new Guid(UserId);
            ApplicationUser ap = db.Users.Find(UserId);
           // bool usrIsAdmiNOrPM = urhelper.IsUserInAnyRole(UserId, "Admin", "PM");
            bool usrIsDev = urhelper.IsUserInRole(UserId, "Submitter");




            Ticket ticket = db.Ticket.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProjectId = new SelectList(db.Project, "Id", "Title", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name");

            ViewBag.ProjectId = new SelectList(db.Project, "Id", "Title", ticket.ProjectId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name");

            ViewBag.ProjectId = new SelectList(db.Project, "Id", "Title", ticket.ProjectId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name");

            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
       
        [HttpPost]
        [ValidateAntiForgeryToken]
      //  [Authorize(Roles = "Admin, PM, Developer,Submitter")]

        public async Task<ActionResult> Edit(Ticket ticket)
        {
            //var ticketHistories = db.TicketHistories.Where(t => t.TicketId == ticket.Id).FirstOrDefault();
            StringBuilder sb = new StringBuilder();
            var oldTicket = db.Ticket.AsNoTracking().FirstOrDefault(t => t.Id == ticket.Id);


            if (ModelState.IsValid)
            {


                try
                {
                   // ticket.CreatedBy = db.Users.Find(User.Identity.GetUserId()).Email;
                    ticket.Created = oldTicket.Created;
                    ticket.Updated = DateTimeOffset.Now;
                    db.Entry(ticket).State = EntityState.Modified;
                    db.SaveChanges();
                    var newticket = db.Ticket.Find(ticket.Id);



                    if (oldTicket != ticket)
                    {
                        sb.AppendLine("Changed on " + DateTimeOffset.Now);
                        //sb.Append("<br />");

                        if (oldTicket.TicketStatusId != newticket.TicketStatusId)
                        {
                            var newTicketStatus = db.TicketStatuses.Where(s => s.Id == newticket.TicketStatusId).Select(n => n.Name).FirstOrDefault();
                            sb.AppendLine("This ticket status changed from " + oldTicket.TicketStatuses.Name + " to " + (newTicketStatus));
                           /* sb.Append("<br />")*/;
                        }

                        if (oldTicket.TicketTypeId != ticket.TicketTypeId)
                        {
                            var newTicketType = db.TicketTypes.Where(t => t.Id == newticket.TicketTypeId).Select(n => n.Name).FirstOrDefault();
                            sb.AppendLine("This ticket type changed from " + oldTicket.TicketTypes.Name + " to " + (newTicketType));
                            //sb.Append("<br />");
                        }

                        if (oldTicket.TicketPriorityId != ticket.TicketPriorityId)
                        {
                            var newTicketPriority = db.TicketPriorities.Where(p => p.Id == newticket.TicketPriorityId).Select(n => n.Name).FirstOrDefault();
                            sb.AppendLine("This ticket priority changed from " + oldTicket.TicketPriorities.Name + " to " + (newTicketPriority));
                            //sb.Append("<br />");
                        }

                        if (oldTicket.AssigneeId != ticket.AssigneeId)
                        {
                            var newTicketAssignee = db.Users.Where(u => u.Id == newticket.AssigneeId).FirstOrDefault();

                            sb.AppendLine("This ticket status changed from " + oldTicket.TicketStatusId + " to " + (newTicketAssignee));
                        }
                    }

                    var history = new History();
                    
                    history.Id = Guid.NewGuid();
                    history.TicketId = ticket.Id;
                    history.Body = sb.ToString();

                    history.Updated = DateTimeOffset.Now;
                    db.Histories.Add(history);
                    db.SaveChanges();


                    if (ticket.AssigneeId != null)

                    {
                        ApplicationUser user = db.Users.Where(u => u.UserName.Equals(ticket.AssigneeId, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                        await UserManager.SendEmailAsync(user.Id, "You have a ticket needing attention!", "Please log in to dashboard to see if you have a new ticket or the ticket you are working on has been updated!");

                    }

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {

                    throw;
                }
             






              
            }

            return View(ticket);
        }


        private ApplicationUserManager _userManager;



        public TicketsController(ApplicationUserManager userManager)
        {
            UserManager = userManager;

        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }






        // GET: Tickets/Delete/5
        [Authorize(Roles = "Admin")]

        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Ticket.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Delete/5
        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Ticket ticket = db.Ticket.Find(id);
            db.Ticket.Remove(ticket);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public virtual void ReleaseController(IController controller)
        {
            IDisposable disposable = controller as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }

        [HttpPost]
        public ActionResult RemoveDevAssgedToTicket(string AssgedDevUsrId, string ticketId)
        {

            Guid tickGuid = new Guid(ticketId);
            Ticket tik = db.Ticket.Find(tickGuid);
            if (!string.IsNullOrEmpty(AssgedDevUsrId))
            {
                Guid userGuid = new Guid(AssgedDevUsrId);


                ApplicationUser usr = db.Users.Find(AssgedDevUsrId);

                if (usr != null)
                {
                 //  string newAssgedTo = tik.AssigneeId.Replace(AssgedDevUsrId, "");
                    tik.AssigneeId = string.Empty;
                    db.SaveChanges();
                }
            }
            //Details(tickGuid, false);
            return Details(tickGuid, false);

        }

        [Authorize(Roles = "Admin, PM")]
        public ActionResult AssignDevsToTicket(string ticketId, bool usrIsDev)
        {
            Guid tickGuid = new Guid(ticketId);
            Ticket tik = db.Ticket.Find(tickGuid);

            List<ApplicationUser> developers = new List<ApplicationUser>();
            foreach (var user in tik.Project.ApplicationUsers)
            {
                if (!uthelper.UserHasTicketsAssigned(tik.Project, user))
                {
                    if (urhelper.IsUserInRole(user.Id, "Developer"))
                    {
                        developers.Add(user);
                    }
                }
            }


            
            var list = developers.Select(rr => new SelectListItem { Value = rr.Email.ToString(), Text = rr.FirstName + " " + rr.LastName }).ToList();
            ViewData["developers"] = list;
            return View(tik);
        }


        public ActionResult AssignTicketToDev(string ticketId, string DevelopersUsrId, bool usrIsDev)
        {

            Guid tickGuid = new Guid(ticketId);
            Ticket tik = db.Ticket.Find(tickGuid);

            if (tik != null)
            {
                tik.AssigneeId = tik.AssigneeId + "*" + DevelopersUsrId;
                tik.AssigneeId =  DevelopersUsrId;
                db.SaveChanges();
            }

            return Details(tickGuid, usrIsDev);

        }


        [Authorize(Roles = "Submitter")]
        public ActionResult SubmitterDashboard()
        {
            return View();
        }

    }



}