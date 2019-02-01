using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OruBloggen.Models
{
    public class AdminViewModel
    {
        public List<PostModel> postModels { get; set; }

        public ReportModel reportModel { get; set; }

        public PostReportModel postReportModel { get; set; }

        public UserModel userModel { get; set; }
    }
}