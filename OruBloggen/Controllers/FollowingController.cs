using Microsoft.AspNet.Identity;
using OruBloggen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OruBloggen.Controllers
{
    [Authorize, AuthorizeUser]
    public class FollowingController : Controller
    {
        // GET: Following
        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult FollowUser(string followID)
        {
            var ctx = new OruBloggenDbContext();
            ctx.Notifications.Add(new NotificationModel
            {
                UserID = User.Identity.GetUserId(),
                FollowUserID = followID
            });
            ctx.SaveChanges();

            return RedirectToAction("ShowOtherUser", "ProfilePage", new { @id = followID });
        }

        public ActionResult UnfollowUser (string unfollowID)
        {
            var ctx = new OruBloggenDbContext();
            var user = User.Identity.GetUserId();
            var notificationId = ctx.Notifications.Where(t => t.UserID == user).Where(t => t.FollowUserID == unfollowID).Select(t => t.NotificationID).First();
           
            var unfollow = ctx.Notifications.Find(notificationId);
            ctx.Notifications.Remove(unfollow);
            ctx.SaveChanges();

            return RedirectToAction("ShowOtherUser", "ProfilePage", new { @id = unfollowID });
        }

        public ActionResult FollowCategory(int categoryID)
        {
            var ctx = new OruBloggenDbContext();
            var userID = User.Identity.GetUserId();

            ctx.Notifications.Add(new NotificationModel
            {
                UserID = userID,
                FollowCategoryID = categoryID,
            });

            ctx.SaveChanges();

            return RedirectToAction("ShowInfo", "ProfilePage");
        }

        public ActionResult UnfollowCategory(int categoryID)
        {
            var ctx = new OruBloggenDbContext();
            var user = User.Identity.GetUserId();
            var notificationId = ctx.Notifications.Where(t => t.UserID == user).Where(t => t.FollowCategoryID == categoryID).Select(t => t.NotificationID).First();

            var unfollow = ctx.Notifications.Find(notificationId);
            ctx.Notifications.Remove(unfollow);
            ctx.SaveChanges();

            return RedirectToAction("ShowInfo", "ProfilePage");
        }
    }
}