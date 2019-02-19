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
    [Authorize, AuthorizeUser]
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

        //Users
        public ActionResult AllUsers()
        {
            var adminView = new AdminViewModel();
            var adminViewList = new List<UserAdminViewModel>();
            var teams = ctx.Teams.ToList();
            var currentUser = User.Identity.GetUserId();
            adminView.MyAccount = ctx.Users.FirstOrDefault(u => u.UserID == currentUser);

            var autoCtx = new ApplicationDbContext();
            var usersInfoList = autoCtx.Users;

            foreach(var user in ctx.Users)
            {
                if (User.Identity.GetUserId() != user.UserID)
                {

                    var email = usersInfoList.FirstOrDefault(u => u.Id == user.UserID).Email;


                    adminViewList.Add(new UserAdminViewModel
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
                        ImagePath = user.UserImagePath,
                        Email = email
                    });
                }
            }

            adminView.Userlist = adminViewList;
            adminView.Teams = ListTeams();

            return View(adminView);
        }

        public List<SelectListItem> ListTeams()
        {
            var ctx = new OruBloggenDbContext();
            List<SelectListItem> List = new List<SelectListItem>();
            foreach (var item in ctx.Teams)
            {
                List.Add(new SelectListItem() { Text = item.TeamName, Value = item.TeamID.ToString() });
            }

            return List;

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

            var autoCtx = new ApplicationDbContext();
            var loginInfo = autoCtx.Users.FirstOrDefault(l => l.Id == item.UserID);
            loginInfo.Email = item.Email;
            loginInfo.UserName = item.Email;
            autoCtx.SaveChanges();

            return RedirectToAction("AllUsers");
        }

        public ActionResult RemoveUser(string id)
        {
            var userMeeting = ctx.UserMeetings.Where(u => u.UserID == id);
            foreach(var um in userMeeting)
            {
                ctx.UserMeetings.Remove(um);
                ctx.SaveChanges();
            }

            var projects = ctx.Projects.Where(p => p.ProjectUserID == id);
            foreach(var p in projects)
            {
                RemoveProjectFiles(p.ProjectID);
                RemoveComments(p.ProjectID);
                ctx.Projects.Remove(p);
                ctx.SaveChanges();
            }

            var posts = ctx.Posts.Where(p => p.PostUserID == id);
            foreach(var p in posts)
            {
                RemovePostFiles(p.PostID);
                RemovePostReports(p.PostID);
                ctx.Posts.Remove(p);
                ctx.SaveChanges();
            }

            var noticifation = ctx.Notifications.Where(n => n.UserID == id);
            foreach(var n in noticifation)
            {
                ctx.Notifications.Remove(n);
                ctx.SaveChanges();
            }

            var postReceiver = ctx.Messages.Where(m => m.MessageReceiverID == id);
            var postSender = ctx.Messages.Where(m => m.MessageSenderID == id);
            foreach(var pr in postReceiver)
            {
                ctx.Messages.Remove(pr);
                ctx.SaveChanges();
            }

            foreach(var ps in postSender)
            {
                ctx.Messages.Remove(ps);
                ctx.SaveChanges();
            }

            var meeting = ctx.Meetings.Where(m => m.MeetingUserID == id);
            foreach(var m in meeting)
            {
                RemoveInvited(m.MeetingID);
                ctx.Meetings.Remove(m);
                ctx.SaveChanges();
            }

            var user = ctx.Users.FirstOrDefault(u => u.UserID == id);
            ctx.Users.Remove(user);
            ctx.SaveChanges();

            var autoCtx = new ApplicationDbContext();
            var account = autoCtx.Users.FirstOrDefault(p => p.Id == id);
            autoCtx.Users.Remove(account);
            autoCtx.SaveChanges();

            return RedirectToAction("AllUsers");
        }

        private void RemoveProjectFiles(int projectID)
        {
            var fileList = ctx.ProjectFiles.Where(f => f.ProjectID == projectID);

            foreach (var file in fileList)
            {
                ctx.ProjectFiles.Remove(file);
                ctx.SaveChanges();
            }
        }

        private void RemoveComments(int projectID)
        {
            var comments = ctx.ProjectComments.Where(c => c.ProjectID == projectID);

            foreach (var comment in comments)
            {
                ctx.ProjectComments.Remove(comment);
                ctx.SaveChanges();
            }
        }

        private void RemovePostFiles(int postID)
        {
            var postList = ctx.PostFiles.Where(f => f.PostID == postID);

            foreach (var post in postList)
            {
                ctx.PostFiles.Remove(post);
                ctx.SaveChanges();
            }
        }

        private void RemovePostReports(int postID)
        {
            var reportList = ctx.PostReports.Where(p => p.PostID == postID);
            
            foreach(var report in reportList)
            {
                ctx.PostReports.Remove(report);
                ctx.SaveChanges();
            }
        }

        private void RemoveInvited(int meetingID)
        {
            var invited = ctx.UserMeetings.Where(u => u.MeetingID == meetingID);
            foreach(var i in invited)
            {
                ctx.UserMeetings.Remove(i);
                ctx.SaveChanges();
            }
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

        //Posts
        public ActionResult AdminPosts ()
        {

            var adminView = new AdminViewModel();
            adminView.PostList = FillPostList();

            return View(adminView);
        }

        public List<PostViewModel> FillPostList ()
        {
            var list = new List<PostViewModel>();
            var posts = ctx.Posts.ToList();
            var categories = ctx.Categories.ToList();
            var users = ctx.Users.ToList();

            foreach (var post in posts)
            {
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

            return list;
        }

        [HttpPost]
        public ActionResult ChangePost(PostViewModel post)
        {
            var ctx = new OruBloggenDbContext();
            var previousPost = ctx.Posts.FirstOrDefault(p => p.PostID == post.PostID);

            previousPost.PostTitle = post.PostTitle;
            previousPost.PostText = post.PostText;
            ctx.SaveChanges();

            return RedirectToAction("AdminPosts");
        }

        public ActionResult RemovePost(int postID)
        {
            var reportedPost = ctx.PostReports.FirstOrDefault(p => p.PostID == postID);

            if (reportedPost != null)
            {
                ctx.PostReports.Remove(reportedPost);
            }

            var post = ctx.Posts.FirstOrDefault(p => p.PostID == postID);
            var fileList = ctx.PostFiles.Where(f => f.PostID == postID);

            foreach (var file in fileList)
            {
                ctx.PostFiles.Remove(file);
            }

            ctx.Posts.Remove(post);
            ctx.SaveChanges();

            return RedirectToAction("AdminPosts");
        }




        //Accept user
        public ActionResult ActivateUser ()
        {
            var UserList = ctx.Users.Where(u => !u.UserActive);

            return View(UserList);
        }

        public ActionResult ActivateNewUser(string id)
        {
            var account = ctx.Users.FirstOrDefault(u => u.UserID == id);
            account.UserActive = true;
            ctx.SaveChanges();
            return RedirectToAction("ActivateUser");
        }

        public int CountUnactivatedUsers()
        {
            int count = 0;
            count = ctx.Users.Where(u => !u.UserActive).Count();

            return count;
        }
    }
}