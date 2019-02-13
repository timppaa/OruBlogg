using OruBloggen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace OruBloggen.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        public OruBloggenDbContext ctx = new OruBloggenDbContext();

        //AdminView
        public ActionResult Index()
        {
            return View();
        }


        //Reports
        public ActionResult ReportDetails ()
        {
            var pageViewList = new PostReportPageViewModel();
            var reportList = ListReports();
            pageViewList.ReportList = reportList;
            return View("ReportDetails", pageViewList);
        }

        private List<PostReportViewModel> ListReports ()
        {
            var list = new List<PostReportViewModel>();
            var reportList = ctx.PostReports.ToList();
            var postList = ctx.Posts.ToList();
            var users = ctx.Users.ToList();

            foreach(var post in postList)
            {
                foreach(var report in reportList)
                {
                    if(post.PostID == report.PostID)
                    {
                        var senderName = users.FirstOrDefault(u => u.UserID == post.PostUserID).UserFirstname + " " + users.FirstOrDefault(u => u.UserID == post.PostUserID).UserLastname;
                        var isFormal = "Formellt inlägg";

                        if(!post.PostFormal)
                        {
                            isFormal = "Informellt inlägg";
                        }

                        var fileList = ctx.PostFiles.Where(f => f.PostID == post.PostID).ToList();

                        list.Add(new PostReportViewModel
                        {
                            PostID = post.PostID,
                            PostTitle = post.PostTitle,
                            PostText = post.PostText,
                            PostSenderName = senderName,
                            PostFilePath = fileList,
                            PostFormal = isFormal,
                            ReportID = report.PostReportID,
                            ReportReason = report.ReportReason
                        });
                    }
                }
            }

            return list;
        }

        public ActionResult DeclinePost(int postID, int reportID)
        {
            var report = ctx.PostReports.FirstOrDefault(r => r.PostReportID == reportID);
            ctx.PostReports.Remove(report);
            ctx.SaveChanges();

            var fileList = ctx.PostFiles.Where(f => f.PostID == postID);

            foreach (var file in fileList)
            {
                ctx.PostFiles.Remove(file);
            }
            ctx.SaveChanges();

            var post = ctx.Posts.FirstOrDefault(p => p.PostID == postID);
            ctx.Posts.Remove(post);
            ctx.SaveChanges();

            return RedirectToAction("ReportDetails");
        }

        public ActionResult AcceptPost(int reportID)
        {
            var report = ctx.PostReports.FirstOrDefault(r => r.PostReportID == reportID);
            ctx.PostReports.Remove(report);
            ctx.SaveChanges();

            return RedirectToAction("ReportDetails");
        }

        public int CountReports ()
        {
            int count = 0;
            count = ctx.PostReports.Count();
            
            return count;
        }

        //Categories
        public ActionResult AdminCategories()
        {
            var categoriesList = ctx.Categories.OrderByDescending(c => c.IsFormel).ToList();

            return View(categoriesList);
        }

        public int CountPostAtCategories (int categoryID)
        {
            int count = 0;
            var posts = ctx.Posts.Where(p => p.PostCategoryID == categoryID).ToList();
            count = posts.Count();

            return count;
        }

        public ActionResult ChangeCategory (CategoryModel category)
        {
            var cat = ctx.Categories.FirstOrDefault(c => c.CategoryID == category.CategoryID);

            if (!String.IsNullOrEmpty(cat.CategoryName))
            {
                cat.CategoryName = category.CategoryName;
            }

            ctx.SaveChanges();

            return RedirectToAction("AdminCategories");
        }

        public ActionResult RemoveCategory (int id)
        {
            var category = ctx.Categories.FirstOrDefault(c => c.CategoryID == id);
            
            if(category.IsFormel)
            {
                var postList = ctx.Posts.Where(p => p.PostCategoryID == id).ToList();
                foreach(var post in postList)
                {
                    post.PostCategoryID = 2;
                    ctx.SaveChanges();
                }
            }

            else
            {
                var postList = ctx.Posts.Where(p => p.PostCategoryID == id).ToList();
                foreach (var post in postList)
                {
                    post.PostCategoryID = 1;
                    ctx.SaveChanges();
                }
            }

            ctx.Categories.Remove(category);
            ctx.SaveChanges();


            return RedirectToAction("AdminCategories");
        }
   
        // Posts
        //public ActionResult Posts ()
        //{
        //    AVM.postModelList = ctx.Posts.ToList();
        //    return View(AVM);
        //}

        //public ActionResult PostDetails (int? id)
        //{

        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    AVM.postModel = ctx.Posts.Find(id);
        //    AVM.userModel = ctx.Users.FirstOrDefault(u => u.UserID == AVM.postModel.PostUserID);
        //    if (AVM.postModel == null)
        //    {
        //        Console.WriteLine("Test");
        //        return HttpNotFound();
        //    }
        //    return View(AVM);
        //}

        //public ActionResult DeletePost(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    AVM.postModel = ctx.Posts.Find(id);
        //    AVM.userModelList = ctx.Users.ToList();
        //    if (AVM.postModel == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(AVM);
        //}

        // POST: PostModels/Delete/5
        [HttpPost, ActionName("DeletePost")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {

            PostModel postModel = ctx.Posts.Find(id);
            
            ctx.Posts.Remove(postModel);
            ctx.SaveChanges();
            return RedirectToAction("Posts");
        }



        //Users
        public ActionResult AllUsers()
        {
            var adminView = new AdminViewModel();
            var adminViewList = new List<UserAdminViewModel>();
            var teams = ctx.Teams.ToList();
            var currentUser = User.Identity.GetUserId();
            adminView.MyAccount = ctx.Users.FirstOrDefault(u => u.UserID == currentUser);

            foreach(var user in ctx.Users)
            {
                adminViewList.Add( new UserAdminViewModel
                {
                    UserID = user.UserID,
                    Firstname = user.UserFirstname,
                    Lastname = user.UserLastname,
                    Socialnumber = user.UserBirthDate,
                    Phonenumber = user.UserPhoneNumber,
                    Position = user.UserPosition,
                    isAdmin = user.UserIsAdmin,
                    TeamID = user.UserTeamID,
                    Team = teams.FirstOrDefault(t => t.TeamID == user.UserTeamID).TeamName,
                    ImagePath = user.UserImagePath
                });
            }

            adminView.Userlist = adminViewList;
            ListTeams();

            return View(adminView);
        }

        public void ListTeams()
        {
            var ctx = new OruBloggenDbContext();
            List<SelectListItem> List = new List<SelectListItem>();
            foreach (var item in ctx.Teams)
            {
                List.Add(new SelectListItem() { Text = item.TeamName, Value = item.TeamID.ToString() });
            }
            ViewData["Teams"] = List;

        }

        public ActionResult AssignRoles(string id)
        {
            UserModel userModel = ctx.Users.FirstOrDefault(u => u.UserID == id);
            if (!userModel.UserIsAdmin)
            {
                userModel.UserIsAdmin = true;
            }
            else
            {
                userModel.UserIsAdmin = false;
            }

            ctx.SaveChanges();
            return RedirectToAction("AllUsers");

        }

        public ActionResult UpdateUser (UserAdminViewModel item)
        {
            var account = ctx.Users.FirstOrDefault(c => c.UserID == item.UserID);

            account.UserFirstname = item.Firstname;
            account.UserLastname = item.Lastname;
            account.UserPosition = item.Position;
            account.UserPhoneNumber = item.Phonenumber;
            account.UserIsAdmin = item.isAdmin;
            account.UserTeamID = item.TeamID;
            ctx.SaveChanges();

            return RedirectToAction("AllUsers");
        }

        //News
        public ActionResult AdminNews ()
        {
            var newsView = new NewsViewModel();
            newsView.NewsList = FillNewsList();
            return View(newsView);
        }

        public List<NewsModel> FillNewsList ()
        {
            var list = new List<NewsModel>();

            foreach(var news in ctx.News)
            {
                list.Add(new NewsModel
                {
                    NewsID = news.NewsID,
                    NewsTitle = news.NewsTitle,
                    NewsText = news.NewsText,
                    NewsDate = news.NewsDate
                });
            }

            return list;
        }

        [HttpPost]
        public ActionResult AddNews(NewsModel news)
        {
            ctx.News.Add(new NewsModel {
                NewsTitle = news.NewsTitle,
                NewsText = news.NewsText,
                NewsDate = DateTime.Now
            });

            ctx.SaveChanges();

            return RedirectToAction("AdminNews");
        }

        public ActionResult DeclineNews(int newsID)
        {
            var news = ctx.News.FirstOrDefault(n => n.NewsID == newsID);
            ctx.News.Remove(news);
            ctx.SaveChanges();

            return RedirectToAction("AdminNews");
        }
    }
}