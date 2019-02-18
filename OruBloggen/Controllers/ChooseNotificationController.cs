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

        public ActionResult ChosenNotifications(bool isActive, string value)
        {
            var ctx = new OruBloggenDbContext();
            var currentUser = User.Identity.GetUserId();
            var userModel = ctx.Users.FirstOrDefault(u => u.UserID == currentUser);

            if (value == "pm")
            {
                userModel.UserPmNotification = isActive;
            }
            else if (value == "sms")
            {
                userModel.UserSmsNotification = isActive;
            }
            else 
            {
                userModel.UserEmailNotification = isActive;
            }

            ctx.SaveChanges();

            return RedirectToAction("ProfileRedirect", "ProfilePage");
        }
    }
}