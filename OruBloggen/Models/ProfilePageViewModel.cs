using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OruBloggen.Models
{
    public class ProfilePageViewModel
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Team { get; set; }
        public int PhoneNumber { get; set; }
        public string Position { get; set; }
        public string ImagePath { get; set; }
        public string UserIsFollowed { get; set; }
        public string FollowedID { get; set; }
    }
}