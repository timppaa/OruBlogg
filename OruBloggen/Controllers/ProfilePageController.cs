using Microsoft.AspNet.Identity;
using OruBloggen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OruBloggen.Controllers
{
    public class ProfilePageController : Controller
    {
        // GET: ProfilePage
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ProfileRedirect()
        {
            return RedirectToAction("ShowOtherUser", new { id = User.Identity.GetUserId() });
        }

        //public ActionResult ShowInfo()
        //{
        //    var ctx = new OruBloggenDbContext();
        //    var identityCtx = new ApplicationDbContext();


        //    var userId = User.Identity.GetUserId();
        //    var Users = ctx.Users.Find(userId);
        //    var identityUser = identityCtx.Users.Find(userId);
        //    var teamId = Users.UserTeamID;
        //    var team = ctx.Teams.FirstOrDefault(t => t.TeamID == teamId).TeamName;
        //    var path = "/Images/" + Users.UserImagePath;

        //    var model = new ProfilePageViewModel
        //    {
        //    /*model.*/ImagePath = path,
        //    /*model.*/Firstname = Users.UserFirstname,
        //    /*model.*/Lastname = Users.UserLastname,
        //    /*model.*/Email = identityUser.Email,
        //    /*model*/PhoneNumber = Users.UserPhoneNumber,
        //    /*model.*/Team = team,
        //    /*model.*/Position = Users.UserPosition,
        //    };

        //    return View(model);
        //}

        public ActionResult ShowOtherUser(string id)
        {
            var ctx = new OruBloggenDbContext();
            var identityCtx = new ApplicationDbContext();

            var userId = User.Identity.GetUserId();

            var Users = ctx.Users.Find(id);
            var identityUser = identityCtx.Users.Find(id);
            var teamId = Users.UserTeamID;
            var team = ctx.Teams.FirstOrDefault(t => t.TeamID == teamId).TeamName;
            var path = "/Images/" + Users.UserImagePath;
            var userID = User.Identity.GetUserId();
            var notmodel = ctx.Notifications.Where(t => t.UserID == userID).ToList();
            var isFollowed = "";
            foreach(var item in notmodel)
            {
                if(item.FollowUserID == id)
                {
                    isFollowed = item.FollowUserID;
                }
            }
            var model = new ProfilePageViewModel
            {
                userId = userId,
                ImagePath = path,
                Firstname = Users.UserFirstname,
                Lastname = Users.UserLastname,
                Email = identityUser.Email,
                PhoneNumber = Users.UserPhoneNumber,
                Team = team,
                FollowedID = id,
                UserIsFollowed = isFollowed,
                Position = Users.UserPosition,
                UserEmailNotification = Users.UserEmailNotification,
                UserPmNotification = Users.UserPmNotification,
                UserSmsNotification = Users.UserSmsNotification
            };

            return View("ShowInfo", model);
        }

    }
}