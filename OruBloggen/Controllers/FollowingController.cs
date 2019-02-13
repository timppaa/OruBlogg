using Microsoft.AspNet.Identity;
using OruBloggen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OruBloggen.Controllers
{
    public class FollowingController : Controller
    {
        // GET: Following
        public ActionResult Index()
        {
            return View();
        }
        
        public void FollowUser(string followID)
        {
            var ctx = new OruBloggenDbContext();
            ctx.Notifications.Add(new NotificationModel
            {
                UserID = User.Identity.GetUserId(),
                FollowUserID = followID
            });
            ctx.SaveChanges();

            RedirectToAction("ShowInfo", "ProfilePage", followID);
        }

        public void UnfollowUser (string unfollowID)
        {
            var ctx = new OruBloggenDbContext();
            var user = User.Identity.GetUserId();
            var notificationId = ctx.Notifications.Where(t => t.UserID == user).Where(t => t.FollowUserID == unfollowID).Select(t => t.NotificationID).First();
           
            var unfollow = ctx.Notifications.Find(notificationId);
            ctx.Notifications.Remove(unfollow);
            ctx.SaveChanges();

            RedirectToAction("ShowInfo", "ProfilePage");
        }
    }
}