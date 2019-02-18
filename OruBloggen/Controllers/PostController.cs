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
        public ActionResult FormalPost()
        {
            ListFormelItems();
            FillPostList(true);
            try
            {
                FillReportList();
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
        public ActionResult AddPost(HomePostViewModel post, HttpPostedFileBase[] file)
        {
            var ctx = new OruBloggenDbContext();
            var userID = User.Identity.GetUserId();

            if (!string.IsNullOrEmpty(post.AddPostViewModel.PostTitle) && !string.IsNullOrEmpty(post.AddPostViewModel.PostText))
            {
                ctx.Posts.Add(new PostModel
                {
                    PostTitle = post.AddPostViewModel.PostTitle,
                    PostText = post.AddPostViewModel.PostText,
                    PostDate = DateTime.Now,
                    PostFormal = post.AddPostViewModel.PostFormal,
                    PostUserID = userID,
                    PostCategoryID = int.Parse(post.AddPostViewModel.PostCategory)
                });

                ctx.SaveChanges();

                if (file != null)
                {
                    foreach (var oneFile in file)
                    {
                        SaveFile(oneFile);
                    }
                }
            }
            NotificationController notificationController = new NotificationController();
            notificationController.SendPostPm(userID, post.AddPostViewModel.PostTitle, post.AddPostViewModel.PostText, DateTime.Now, post.AddPostViewModel.PostFormal, int.Parse(post.AddPostViewModel.PostCategory));

            if(post.AddPostViewModel.PostFormal)
            {
                return RedirectToAction("FormalPost");
            }

            else return RedirectToAction("InformalPost");

        }

        private void SaveFile (HttpPostedFileBase file)
        {
            var ctx = new OruBloggenDbContext();
            var post = ctx.Posts.OrderByDescending(p => p.PostID).FirstOrDefault();

            if (file != null)
            {

                string fileType = Path.GetExtension(file.FileName).ToLower();
                string fileName = Path.GetFileNameWithoutExtension(file.FileName) + " (" + post.PostID + ")";
                var filePath = fileName.ToString() + fileType;
                string path = Path.Combine(Server.MapPath("~/PostFiles/" + filePath));
                file.SaveAs(path);

                ctx.PostFiles.Add(new PostFilesModel
                {
                    PostID = post.PostID,
                    FilePath = filePath
                });

                ctx.SaveChanges();
            }
            

        }

        private void SaveFile(HttpPostedFileBase file, PostModel post)
        {
            var ctx = new OruBloggenDbContext();

            if (file != null)
            {

                string fileType = Path.GetExtension(file.FileName).ToLower();
                string fileName = Path.GetFileNameWithoutExtension(file.FileName) + " (" + post.PostID + ")";
                var filePath = fileName.ToString() + fileType;
                string path = Path.Combine(Server.MapPath("~/PostFiles/" + filePath));
                file.SaveAs(path);

                ctx.PostFiles.Add(new PostFilesModel
                {
                    PostID = post.PostID,
                    FilePath = filePath
                });

                ctx.SaveChanges();
            }


        }

        public ActionResult RemoveMyPost(int postID, bool isFormal)
        {
            var ctx = new OruBloggenDbContext();
            var reportedPost = ctx.PostReports.FirstOrDefault(p => p.PostID == postID);

            if (reportedPost != null)
            {
                ctx.PostReports.Remove(reportedPost);
            }

            var post = ctx.Posts.FirstOrDefault(p => p.PostID == postID);
            var fileList = ctx.PostFiles.Where(f => f.PostID == postID);
            
            foreach(var file in fileList)
            {
                ctx.PostFiles.Remove(file);
            }

            ctx.Posts.Remove(post);
            ctx.SaveChanges();

            if (isFormal)
            {
                return RedirectToAction("FormalPost");
            }

            else return RedirectToAction("InformalPost");
        }

        [HttpPost]
        public ActionResult ChangePost (PostViewModel item, HttpPostedFileBase[] file)
        {
            var ctx = new OruBloggenDbContext();
            var post = ctx.Posts.FirstOrDefault(p => p.PostID == item.PostID);

            post.PostTitle = item.PostTitle;
            post.PostText = item.PostText;
            ctx.SaveChanges();

            if (file != null)
            {
                foreach (var oneFile in file)
                {
                    SaveFile(oneFile, post);
                }
            }


            if (item.PostFormal)
            {
                return RedirectToAction("FormalPost");
            }

            else return RedirectToAction("InformalPost");
        }

        public void RemoveFile(int postFileID)
        {
            var ctx = new OruBloggenDbContext();
            var file = ctx.PostFiles.FirstOrDefault(f => f.PostFileID == postFileID);
            ctx.PostFiles.Remove(file);
            ctx.SaveChanges();
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

            HomePostList.PostViewModel = list.OrderByDescending(p => p.PostDate).ToList();

        }

        private void AddToHomePostList(PostModel post, List<PostViewModel> list)
        {
            var ctx = new OruBloggenDbContext();
            var categories = ctx.Categories.ToList();
            var users = ctx.Users.ToList();

            var postSenderName = users.FirstOrDefault(u => u.UserID == post.PostUserID).UserFirstname + " " + users.FirstOrDefault(u => u.UserID == post.PostUserID).UserLastname;
            string senderImage = users.FirstOrDefault(u => u.UserID == post.PostUserID).UserImagePath;

            var fileList = ctx.PostFiles.Where(f => f.PostID == post.PostID).ToList();

            list.Add(new PostViewModel
            {
                PostID = post.PostID,
                PostTitle = post.PostTitle,
                PostText = post.PostText,
                PostDate = post.PostDate,
                PostCategory = categories.FirstOrDefault(c => c.CategoryID == post.PostCategoryID).CategoryName,
                PostFormal = post.PostFormal,
                PostSenderName = postSenderName,
                PostFilePath = fileList,
                SenderProfilePath = senderImage,
                PostSender = post.PostUserID

            });
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

            HomePostList.PostViewModel = list.OrderByDescending(p => p.PostDate).ToList();

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

        public void FillReportList()
        {
            var ctx = new OruBloggenDbContext();
            foreach (var item in ctx.PostReports.ToList())
            {
                HomePostList.PostReportModels.Add(item);
            }
        }

        public string ChangeButton(string postID)
        {
            var ctx = new OruBloggenDbContext();
            string isReported = "notReported";

            foreach (var item in ctx.PostReports)
            {
                if (item.PostID.ToString() == postID)
                {
                    isReported = "reported";
                }
            }

            return isReported;

        }

        public ActionResult AddCategory(string category, bool isFormal)
        {
            var ctx = new OruBloggenDbContext();

            ctx.Categories.Add(new CategoryModel
            {
                CategoryName = category,
                IsFormel = isFormal
            });
            ctx.SaveChanges();

            if (isFormal)
            {
                return RedirectToAction("FormalPost");
            }
            else return RedirectToAction("InformalPost");
        }
    }
}