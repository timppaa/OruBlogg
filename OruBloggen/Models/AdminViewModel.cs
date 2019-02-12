using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OruBloggen.Models
{
    public class AdminViewModel
    {
        public List<PostModel> postModelList { get; set; }
        public PostModel postModel { get; set; }

        public PostReportModel postReportModel { get; set; }

        public UserModel userModel { get; set; }
        public List<UserModel> userModelList { get; set; }
    }
}