using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OruBloggen.Models;
using Microsoft.AspNet.Identity;


namespace OruBloggen.Controllers
{
    public class ChooseNotificationController : Controller
    {
        // GET: ChooseNotification
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ChosenNotifications(ProfilePageViewModel ppv)
        {
            var ctx = new OruBloggenDbContext();
            var currentUser = User.Identity.GetUserId();
            var userModel = ctx.Users.FirstOrDefault(u => u.UserID == currentUser);

            userModel.UserPmNotification = ppv.UserPmNotification;
            userModel.UserSmsNotification = ppv.UserSmsNotification;
            userModel.UserEmailNotification = ppv.UserEmailNotification;

            ctx.SaveChanges();

            return RedirectToAction("ProfileRedirect", "ProfilePage");
        }


    }
}