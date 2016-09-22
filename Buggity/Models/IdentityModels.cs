using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using Buggity.Models.CodeFirst;

namespace Buggity.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            this.Projects = new HashSet<Project>();
            this.Tickets = new HashSet<Ticket>();
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public virtual ICollection<Project> Projects { get; set; }
        public virtual ICollection<Ticket> Tickets { get; set; }

        
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }







        public DbSet<Project> Project { get; set; }
        public DbSet<Ticket> Ticket { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<TicketAttachment> TicketAttachments { get; set; }

        public DbSet<History> Histories { get; set; }

        public DbSet<TicketStatus> TicketStatuses { get; set; }

        public DbSet<TicketType> TicketTypes { get; set; }

        public DbSet<TicketPriority> TicketPriorities { get; set; }

       /// public DbSet<Buggity.Models.CodeFirst.Ticket> Tickets { get; set; }







        //public System.Data.Entity.DbSet<Buggity.Models.ApplicationUser> ApplicationUsers { get; set; }


        //public System.Data.Entity.DbSet<Buggity.Models.CodeFirst.AspNetUsers> AspNetUsers { get; set; }

    }
}