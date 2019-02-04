using Microsoft.AspNet.Identity;
using OruBloggen.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OruBloggen.Controllers
{
    [Authorize]
    public class PostController : Controller
    {
        // GET: Post
        public ActionResult Post()
        {
            ListFormelItems();
            ListInformelItems();
            ListPostTypes();
            return View();
        }

        public void ListFormelItems ()
        {
            var ctx = new OruBloggenDbContext();
            List<SelectListItem> list = new List<SelectListItem>();

            foreach(var item in ctx.Categories)
            {
                if (item.IsFormel)
                {
                    list.Add(new SelectListItem() { Text = item.CategoryName, Value = item.CategoryID.ToString() });
                }
            }

            ViewData["CategoriesFormal"] = list;
        }

        public void ListInformelItems()
        {
            var ctx = new OruBloggenDbContext();
            List<SelectListItem> list = new List<SelectListItem>();

            foreach (var item in ctx.Categories)
            {
                if (!item.IsFormel)
                {
                    list.Add(new SelectListItem() { Text = item.CategoryName, Value = item.CategoryID.ToString() });
                }
            }

            ViewData["CategoriesInformal"] = list;
        }

        public void ListPostTypes()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem { Text = "Formellt inlägg", Value = "true" });
            list.Add(new SelectListItem { Text = "Informellt inlägg", Value = "false" });

            ViewData["PostTypes"] = list;
        }

        [HttpPost]
        public ActionResult AddPost(AddPostViewModel post, HttpPostedFileBase file)
        {
            var ctx = new OruBloggenDbContext();
            var userID = User.Identity.GetUserId();
            string categoryID = null;
            var filePath = SaveFile(file);
            
            if (post.PostFormal)
            {
                categoryID = post.CategoryFormal;
            }

            else
            {
                categoryID = post.CategoryInformal;
            }

            if (!string.IsNullOrEmpty(post.PostTitle) && !string.IsNullOrEmpty(post.PostText))
            {
                ctx.Posts.Add(new PostModel
                {
                    PostTitle = post.PostTitle,
                    PostText = post.PostText,
                    PostDate = DateTime.Now,
                    PostFilePath = filePath,
                    PostFormal = post.PostFormal,
                    PostUserID = userID,
                    PostCategoryID = int.Parse(categoryID)
                });

                if (ModelState.IsValid)
                {
                    ctx.SaveChanges();
                }

            }

            return RedirectToAction("Post");

        }

        private string SaveFile (HttpPostedFileBase file)
        {
            var ctx = new OruBloggenDbContext();
            string filePath = null;
            var postID = ctx.Posts.OrderByDescending(p => p.PostID).FirstOrDefault().PostID;
            postID += 1;

            if (file != null)
            {
                string fileType = Path.GetExtension(file.FileName).ToLower();
                int fileName = postID;
                filePath = fileName.ToString() + fileType;
                string path = Path.Combine(Server.MapPath("~/PostFiles/" + filePath));
                file.SaveAs(path);

            }

            return filePath;
        }

    }
}