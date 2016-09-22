using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Buggity.Models.CodeFirst;

namespace Buggity.Models.CodeFirst
{
    public class Ticket
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Ticket()
        {
            this.Histories = new HashSet<History>();
            
            this.TicketAttachments = new HashSet<TicketAttachment>();

            this.Comments = new HashSet<Comment>();
        }



        
        public string Title { get; set; }


        public string Description { get; set; }



        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }


      
       
       
        [Display(Name = "Ticket Attachment")]
        public string PictureUrl { get; set; }


        public virtual ICollection<TicketAttachment> TicketAttachments { get; set; }


        [Display(Name = "Ticket Comment")]
        public string TicketComment { get; set; }


        public virtual ICollection<Comment> Comments { get; set; }





        [Display(Name = "Project")]
        public Guid ProjectId { get; set; }
        [Display(Name = "Project")]
        public virtual Project Project { get; set; }
      
        public virtual TicketPriority TicketPriorities { get; set; }
        [Display(Name = "Priority")]
        public Guid TicketPriorityId { get; set; }

        public virtual TicketType TicketTypes { get; set; }
        [Display(Name = "Type")]
        public Guid TicketTypeId { get; set; }

        public virtual TicketStatus TicketStatuses { get; set; }
        [Display(Name = "Status")]
        public Guid TicketStatusId { get; set; }


        public virtual ICollection<History> Histories { get; set; }



        public virtual ApplicationUser ApplicationUsers { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }
        
      
        //public virtual ICollection<ApplicationUser> CreatedByUser { get; set; }


        [Display(Name = "Assigned To")]
        public string AssigneeId { get; set; }

       // public virtual ApplicationUser AssignedToUser { get; set; }

      //  public virtual ICollection<ApplicationUser> AssignedToUser { get; set; }






    }
}