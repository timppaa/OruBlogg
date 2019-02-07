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

        public ActionResult ShowInfo()
        {
            var ctx = new OruBloggenDbContext();
            var identityCtx = new ApplicationDbContext();


            var userId = User.Identity.GetUserId();
            var Users = ctx.Users.Find(userId);
            var identityUser = identityCtx.Users.Find(userId);
            var teamId = Users.UserTeamID;
            var team = ctx.Teams.FirstOrDefault(t => t.TeamID == teamId).TeamName;
            var path = "/Images/" + Users.UserImagePath;

            var model = new ProfilePageViewModel
            {
            /*model.*/ImagePath = path,
            /*model.*/Firstname = Users.UserFirstname,
            /*model.*/Lastname = Users.UserLastname,
            /*model.*/Email = identityUser.Email,
            /*model*/PhoneNumber = Users.UserPhoneNumber,
            /*model.*/Team = team,
            /*model.*/Position = Users.UserPosition
            };

            return View(model);
        }

        public ActionResult ShowOtherUser(string id)
        {
            var ctx = new OruBloggenDbContext();
            var identityCtx = new ApplicationDbContext();



            var Users = ctx.Users.Find(id);
            var identityUser = identityCtx.Users.Find(id);
            var teamId = Users.UserTeamID;
            var team = ctx.Teams.FirstOrDefault(t => t.TeamID == teamId).TeamName;
            var path = "/Images/" + Users.UserImagePath;

            var model = new ProfilePageViewModel
            {
                /*model.*/
                ImagePath = path,
                /*model.*/
                Firstname = Users.UserFirstname,
                /*model.*/
                Lastname = Users.UserLastname,
                /*model.*/
                Email = identityUser.Email,
                /*model*/
                PhoneNumber = Users.UserPhoneNumber,
                /*model.*/
                Team = team,
                /*model.*/
                Position = Users.UserPosition
            };

            return View("ShowInfo", model);
        }

    }
}