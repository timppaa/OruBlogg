using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OruBloggen.Models
{
    public class MeetingViewModel
    {
        public MeetingModel Meeting { get; set; }

        public IEnumerable<UserModel> AllUsers { get; set; }
        public IEnumerable<UserModel> Members { get; set; }

    }

    public class CalenderViewModel
    {
        public int MeetingID { get; set; }
        public string MeetingTitle { get; set; }
        public string MeetingDesc { get; set; }
        public string MeetingCreator { get; set; }
        public DateTime MeetingStartDate { get; set; }
        public DateTime MeetingEndDate { get; set; }
        
    }

    public class UserMeetingViewModel
    {
        public virtual UserModel UserModel { get; set; }
        public string UserID { get; set; }
    }
}