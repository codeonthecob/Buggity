using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Buggity.Models.CodeFirst
{
    public class TicketViewModel
    {
       
        public Guid Id { get; set; }    


        public string Title { get; set; }
        public string Description { get; set; }

        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }

        [Display(Name = "Ticket Attachment")]
        public string PictureUrl { get; set; }
        [Display(Name = "Project")]

        public string ProjectName { get; set; }
        [Display(Name = "Priority")]

        public string TicketPriority { get; set; }
        [Display(Name = "Type")]

        public string TicketType { get; set; }
        [Display(Name = "Status")]

        public string TicketStatus { get; set; }       


        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }
        

        [Display(Name = "Assigned To")]
        public string AssigneeId { get; set; }

       
    }
}