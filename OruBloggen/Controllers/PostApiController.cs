using Microsoft.AspNet.Identity;
using Microsoft.SqlServer.Server;
using OruBloggen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace OruBloggen.Controllers
{
    [RoutePrefix("api")]
    public class PostApiController : ApiController
    {
        [Route("addPost")]
        [HttpPost]
        public void addPost (string title, string text, bool formal, int category, HttpPostedFileBase file)
        {
            var ctx = new OruBloggenDbContext();
            var userID = User.Identity.GetUserId();

            if (title != null)
            {

                if (text.Length >= 1 && title.Length >= 1)
                {

                    ctx.Posts.Add(new PostModel
                    {
                        PostTitle = title,
                        PostText = text,
                        PostDate = DateTime.Now,
                        PostFormal = formal,
                        PostUserID = userID,
                        PostCategoryID = category
                   });

                    ctx.SaveChanges();
                }
            }

            else
            {

            }
        }


    }
}
