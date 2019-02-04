using Microsoft.AspNet.Identity;
using OruBloggen.Models;
using System;
using System.Web.Http;

namespace OruBloggen.Controllers
{
    [RoutePrefix("api")]
    public class PostApiController : ApiController
    {
        //[Route("addPost")]
        //[HttpPost]
        //public void addPost(PostModel post)
        //{
        //    var ctx = new OruBloggenDbContext();
        //    var userID = User.Identity.GetUserId();

        //    if (post != null)
        //    {

        //        if (post.PostText.Length >= 1 && post.PostTitle.Length >= 1)
        //        {

        //            ctx.Posts.Add(new PostModel
        //            {
        //                PostTitle = post.PostTitle,
        //                PostText = post.PostText,
        //                PostDate = DateTime.Now,
        //                PostFilePath = post.PostFilePath,
        //                PostFormal = post.PostFormal,
        //                PostUserID = userID,
        //                PostCategoryID = post.PostCategoryID
        //            });

        //            ctx.SaveChanges();
        //        }
        //    }
        //}
    }
}
