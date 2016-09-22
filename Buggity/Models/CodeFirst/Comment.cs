﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Buggity.Models.CodeFirst
{
    public class Comment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Display(Name = "Comments")]
        public string CommentText { get; set; }
        public virtual Ticket tickets { get; set; }
        public Guid TicketId { get; set; }
    }
}