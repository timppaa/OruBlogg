using OruBloggen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OruBloggen.Controllers
{
    [Authorize, AuthorizeUser]
    public class ReportPostController : Controller
    {
       [HttpPost]
       public void ReportPost (int postID, string reason)
       {
            var ctx = new OruBloggenDbContext();

            ctx.PostReports.Add(new PostReportModel
            {
                PostID = postID,
                ReportReason = reason
            });

            ctx.SaveChanges();
        }
    }
}