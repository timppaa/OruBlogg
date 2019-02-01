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
        public List<SelectListItem> Users { get; set; }
        public List<SelectListItem> SelectedUsers { get; set; }
    }
}