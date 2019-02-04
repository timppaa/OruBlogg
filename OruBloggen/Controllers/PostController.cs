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
        HomePostViewModel HomePostList = new HomePostViewModel();

        // GET: Post
        public ActionResult Post()
        {
            ListFormelItems();
            ListInformelItems();
            ListPostTypes();
            FillPostList();
            HomePostList.PostViewModel.Reverse(); //Kika på en annan lösning?
            return View(HomePostList);
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
        public ActionResult AddPost(HomePostViewModel post, HttpPostedFileBase file)
        {
            var ctx = new OruBloggenDbContext();
            var userID = User.Identity.GetUserId();
            string categoryID = null;
            var filePath = SaveFile(file);

            if (post.AddPostViewModel.PostFormal)
            {
                categoryID = post.AddPostViewModel.CategoryFormal;
            }

            else
            {
                categoryID = post.AddPostViewModel.CategoryInformal;
            }

            if (!string.IsNullOrEmpty(post.AddPostViewModel.PostTitle) && !string.IsNullOrEmpty(post.AddPostViewModel.PostText))
            {
                ctx.Posts.Add(new PostModel
                {
                    PostTitle = post.AddPostViewModel.PostTitle,
                    PostText = post.AddPostViewModel.PostText,
                    PostDate = DateTime.Now,
                    PostFilePath = filePath,
                    PostFormal = post.AddPostViewModel.PostFormal,
                    PostUserID = userID,
                    PostCategoryID = int.Parse(categoryID)
                });

                ctx.SaveChanges();
            }

            return RedirectToAction("Post");

        }

        private string SaveFile (HttpPostedFileBase file)
        {
            var ctx = new OruBloggenDbContext();
            string filePath = null;
            var postID = 1;

            try
            {
                postID = ctx.Posts.OrderByDescending(p => p.PostID).FirstOrDefault().PostID;
                postID += 1;
            }

            catch
            {

            }

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

        private void FillPostList()
        {
            var ctx = new OruBloggenDbContext();
            var list = new List<PostViewModel>();
            var categories = ctx.Categories;

            foreach (var post in ctx.Posts)
            {
                list.Add(new PostViewModel
                {
                    PostID = post.PostID,
                    PostTitle = post.PostTitle,
                    PostText = post.PostText,
                    PostDate = post.PostDate,
                    PostCategory = post.PostCategoryID.ToString(),
                    PostFormal = post.PostFormal,
                    PostSender = post.PostUserID,
                    PostFilePath = post.PostFilePath
                });
            }

            HomePostList.PostViewModel = list;

        }

    }
}