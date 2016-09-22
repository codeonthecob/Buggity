
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Buggity.Models.CodeFirst
{
    public class Project
    {
        public Project()
        {
            this.Tickets = new HashSet<Ticket>();
            
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Display(Name = "Created Date")]
        public DateTimeOffset Created { get; set; }
        [Display(Name = "Updated Date")]
        public DateTimeOffset? Updated { get; set; }
        [Display(Name ="Project Name")]
        public string Title { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public string Owner { get; set; }
        [AllowHtml]
        [Display(Name = "Description")]
        public string Body { get; set; }
        public virtual ICollection<Ticket> Tickets { get; set; }
       
        public string UserId { get; set; }
        public virtual ICollection<ApplicationUser> ApplicationUsers { get; set; }


    }
}