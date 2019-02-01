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
        public IEnumerable<SelectListItem> Users { get; set; }
        public IEnumerable<string> UserIds { get; set; }
        public IEnumerable<SelectListItem> SelectedUsers { get; set; }
        public IEnumerable<string> SelectedUserIds { get; set; }
    }
}