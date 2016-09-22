using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Buggity.Models;
using Buggity.Models.CodeFirst;

namespace Buggity.Helpers
{
    public class UserTicketsHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public List<ApplicationUser> GetAssgedUsrs(string assignedTo)
        {            
            List<ApplicationUser> lstAssgedUsrs = new List<ApplicationUser>();
            if(!string.IsNullOrEmpty(assignedTo))
            {
                //string[] assignedUsers = assignedTo.Split("*".ToCharArray());
               // if (assignedUsers.Length > 1)//skip 1st element, which is the one that gets populated on creation
                 {
                   // for (int i = 1; i < assignedUsers.Length; i++)
                    {
                       // string assignedUserID = assignedUsers[i];
                      //  ApplicationUser appusr = db.Users.Find(assignedTo);
                        ApplicationUser appusr = db.Users.Where(u => u.UserName.Equals(assignedTo, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

                        if (!string.IsNullOrEmpty(assignedTo) && appusr != null)
                            lstAssgedUsrs.Add(appusr);
                    }
                }
            }
            return lstAssgedUsrs;
        }


        public bool UserHasTicketsAssigned(Project project, ApplicationUser user)
        {
            foreach (var tikcet in project.Tickets)
            {
                if (tikcet.AssigneeId == user.Id)
                    return true;

            }

            return false;
        }
    }

}
 