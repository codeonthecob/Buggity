using System;
using System.Data.Entity;
using System.Net;
using System.Web.Mvc;
using System.Linq;
using System.Web;
using Buggity.Models;
using System.IO;
using Buggity.Models.CodeFirst;
using System.Xml.Linq;

namespace Buggity.Controllers
{
    public class TicketAttachmentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: TicketAttachments
        [Authorize]
        public ActionResult Index()
        {
            return View(db.TicketAttachments.ToList());
        }

        // GET: TicketAttachments/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketAttachment ticketAttachment = db.TicketAttachments.Find(id);
            if (ticketAttachment == null)
            {
                return HttpNotFound();
            }
            return View(ticketAttachment);
        }

        // GET: TicketAttachments/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TicketAttachments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create(TicketAttachment ticketattachment, HttpPostedFileBase image)
        {
            if (image != null && image.ContentLength > 0)
            {
                //check the file name to make sure its an image
                var ext = Path.GetExtension(image.FileName).ToLower();
                if (ext != ".png" && ext != ".jpg" && ext != ".jpeg" && ext != ".gif" && ext != ".bmp")
                    ModelState.AddModelError("image", "Invalid Format.");
            }
            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    //relative server path
                    var filePath = "/TicketImages/";
                    // path on physical drive on server
                    var absPath = Server.MapPath("~" + filePath);
                    // media url for relative path
                    ticketattachment.MediaURL = filePath + image.FileName;
                    //save image
                    image.SaveAs(Path.Combine(absPath, image.FileName));
                }

                ticketattachment.Id = Guid.NewGuid();
                db.TicketAttachments.Add(ticketattachment);

                db.SaveChanges();




                TempData["ticketid"] = ticketattachment.TicketId;
                TempData["dev"] = false;
                return RedirectToAction("Details", "Tickets");

            }

            else
            {
                return RedirectToAction("Create");
            }

        }



        // GET: TicketAttachments/Edit/5
        [Authorize(Roles = "Admin, PM, Developer")]
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketAttachment ticketAttachment = db.TicketAttachments.Find(id);
            if (ticketAttachment == null)
            {
                return HttpNotFound();
            }
            return View(ticketAttachment);
        }

        // POST: TicketAttachments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ImagePath")] TicketAttachment ticketAttachment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticketAttachment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ticketAttachment);
        }

        // GET: TicketAttachments/Delete/5
        [Authorize(Roles = "Admin, PM, Developer")]
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketAttachment ticketAttachment = db.TicketAttachments.Find(id);
            if (ticketAttachment == null)
            {
                return HttpNotFound();
            }
            return View(ticketAttachment);
        }

        // POST: TicketAttachments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, PM, Developer")]
        public ActionResult DeleteConfirmed(Guid id)
        {
            TicketAttachment ticketAttachment = db.TicketAttachments.Find(id);
            db.TicketAttachments.Remove(ticketAttachment);
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
    }
}
