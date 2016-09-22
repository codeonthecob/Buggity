
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Buggity.Models.CodeFirst
{
    public class History
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Display(Name = "Ticket")]
        public Guid TicketId { get; set; }
        [AllowHtml]
        [Display(Name = "Description")]
        public string Body { get; set; }
        public DateTimeOffset Updated { get; set; }

        public virtual Ticket Ticket { get; set; }
    }
}