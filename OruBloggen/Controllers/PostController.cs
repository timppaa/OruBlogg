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
        
        public void FillReportList()
        {
            var ctx = new OruBloggenDbContext(); 
            foreach(var item in ctx.PostReports.ToList() )
            {
                HomePostList.PostReportModels.Add(item);
            }
        }

        public string ChangeButton(string postID)
        {
            var ctx = new OruBloggenDbContext();
            string isReported = "notReported";

            foreach (var item in ctx.PostReports )
            {
                   if (item.PostID.ToString() == postID) {
                    isReported = "reported";
                   }
            }

            return isReported;
            
        }

        // GET: Post
        public ActionResult FormalPost()
        {
            ListFormelItems();
            FillPostList(true);
            try
            {
                HomePostList.PostViewModel.Reverse(); //Kika på en annan lösning?
            }
            catch { }
            
            return View(HomePostList);
        }

    // GET: Post
    public ActionResult InformalPost()
        {
            ListInformelItems();
            FillPostList(false);
            try
            {
                FillReportList();
                HomePostList.PostViewModel.Reverse(); //Kika på en annan lösning?
            }
            catch { }

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



        [HttpPost]
        public ActionResult AddPost(HomePostViewModel post, HttpPostedFileBase file)
        {
            var ctx = new OruBloggenDbContext();
            var userID = User.Identity.GetUserId();
            var filePath = SaveFile(file);

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
                    PostCategoryID = int.Parse(post.AddPostViewModel.PostCategory)
                });

                ctx.SaveChanges();
            }

            if(post.AddPostViewModel.PostFormal)
            {
                return RedirectToAction("FormalPost");
            }

            else return RedirectToAction("InformalPost");

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
                string fileName = Path.GetFileNameWithoutExtension(file.FileName) + " (" +postID+ ")";
                filePath = fileName.ToString() + fileType;
                string path = Path.Combine(Server.MapPath("~/PostFiles/" + filePath));
                file.SaveAs(path);

            }

            return filePath;
        }

        public ActionResult RemoveMyPost(int postID, bool isFormal)
        {
            var ctx = new OruBloggenDbContext();
            var post = ctx.Posts.FirstOrDefault(p => p.PostID == postID);
            ctx.Posts.Remove(post);
            ctx.SaveChanges();

            if (isFormal)
            {
                return RedirectToAction("FormalPost");
            }

            else return RedirectToAction("InformalPost");
        }




        private void FillPostList(bool isFormal)
        {
            var ctx = new OruBloggenDbContext();
            var list = new List<PostViewModel>();

            if (isFormal) //Listar formella inlägg
            {
                foreach (var post in ctx.Posts)
                {
                    if (post.PostFormal)
                    {
                        AddToHomePostList(post, list);
                    }
                }
            }

            else //Listar informella inlägg
            {
                foreach (var post in ctx.Posts)
                {
                    if (!post.PostFormal)
                    {
                        AddToHomePostList(post, list);

                    }
                }
            }
        }

        private void AddToHomePostList(PostModel post, List<PostViewModel> list)
        {
            var ctx = new OruBloggenDbContext();
            var categories = ctx.Categories.ToList();
            var users = ctx.Users.ToList();

            var postSenderName = users.FirstOrDefault(u => u.UserID == post.PostUserID).UserFirstname + " " + users.FirstOrDefault(u => u.UserID == post.PostUserID).UserLastname;
            string senderImage = users.FirstOrDefault(u => u.UserID == post.PostUserID).UserImagePath;

            list.Add(new PostViewModel
            {
                PostID = post.PostID,
                PostTitle = post.PostTitle,
                PostText = post.PostText,
                PostDate = post.PostDate,
                PostCategory = categories.FirstOrDefault(c => c.CategoryID == post.PostCategoryID).CategoryName,
                //PostFormal = post.PostFormal,
                PostSenderName = postSenderName,
                PostFilePath = post.PostFilePath,
                SenderProfilePath = senderImage,
                PostSender = post.PostUserID

            });

            HomePostList.PostViewModel = list;
        }






        public ActionResult FilterPosts (int filterID, bool isFormal)
        {
            var ctx = new OruBloggenDbContext();
            var list = new List<PostViewModel>();

            if (isFormal) //Listar formella inlägg
            {
                foreach (var post in ctx.Posts)
                {
                    if (post.PostFormal)
                    {
                        if (post.PostCategoryID == filterID)
                        {
                            AddToHomePostList(post, list);
                        }
                    }
                }
            }

            else //Listar informella inlägg
            {
                foreach (var post in ctx.Posts)
                {
                    if (!post.PostFormal)
                    {
                        if (post.PostCategoryID == filterID)
                        {
                            AddToHomePostList(post, list);
                        }

                    }
                }
            }


            ListInformelItems(filterID);
            ListFormelItems(filterID);

            if (isFormal)
            {
                return View("FormalPost", HomePostList);
            }

            else return View("InformalPost", HomePostList);
        }

        public void ListInformelItems(int filterID)
        {
            var ctx = new OruBloggenDbContext();
            List<SelectListItem> list = new List<SelectListItem>();

            foreach (var item in ctx.Categories)
            {
                if (!item.IsFormel)
                {
                    list.Add(new SelectListItem() { Text = item.CategoryName, Value = item.CategoryID.ToString(), Selected = (item.CategoryID == filterID) });
                }
            }

            ViewData["CategoriesInformal"] = list;
        }

        public void ListFormelItems(int filterID)
        {
            var ctx = new OruBloggenDbContext();
            List<SelectListItem> list = new List<SelectListItem>();

            foreach (var item in ctx.Categories)
            {
                if (item.IsFormel)
                {
                    list.Add(new SelectListItem() { Text = item.CategoryName, Value = item.CategoryID.ToString(), Selected = (item.CategoryID == filterID) });
                }
            }

            ViewData["CategoriesFormal"] = list;
        }
    }
}