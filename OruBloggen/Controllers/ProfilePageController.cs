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
            return RedirectToAction("ShowInfo");
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

           var creator = ctx.Meetings.Where(m => m.MeetingUserID.Equals(userId)).ToList();
           var invited = ctx.UserMeetings.Where(u => u.UserID.Equals(userId)).Where(u => u.AcceptedInvite == false).ToList();
           var accepted = ctx.UserMeetings.Where(u => u.UserID.Equals(userId)).Where(u => u.AcceptedInvite == true).ToList();
            
            foreach(var accept in accepted)
            {
                var meeting = ctx.Meetings.FirstOrDefault(m => m.MeetingID == accept.MeetingID);
                creator.Add(meeting);
            }

           creator = creator.OrderBy(u => u.MeetingStartDate).ToList();
           creator = creator.Where(u => u.MeetingStartDate >= DateTime.Now).ToList();
           invited = invited.Where(u => u.MeetingModel.MeetingUserID != u.UserID).ToList();


            var notmodel = ctx.Notifications.Where(t => t.UserID == userId).ToList();

            var ListOfCategories = ctx.Categories.ToList();

            var model = new ProfilePageViewModel
            {
                userId = userId,
                OtherUserID = userId,
                ImagePath = path,
                Firstname = Users.UserFirstname,
                Lastname = Users.UserLastname,
                Email = identityUser.Email,
                PhoneNumber = Users.UserPhoneNumber,
                Team = team,
                Position = Users.UserPosition,
                MeetingModels = creator,
                UserMeetings = invited,
                UserEmailNotification = Users.UserEmailNotification,
                UserPmNotification = Users.UserPmNotification,
                UserSmsNotification = Users.UserSmsNotification,
                ListCategories = ListOfCategories,
                IsFollowed = notmodel,
            };

            return View(model);
        }

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
            var PersonIsFollowed = "";
            foreach(var item in notmodel)
            {
                if(item.FollowUserID == id)
                {
                    PersonIsFollowed = item.FollowUserID;
                }
            }

            var MeetingModels = ctx.Meetings.ToList();
            var UserMeetings = ctx.UserMeetings.Where(u => u.UserID.Equals(id)).ToList();

            var creator = ctx.Meetings.Where(m => m.MeetingUserID.Equals(id)).ToList();
            var invited = ctx.UserMeetings.Where(u => u.UserID.Equals(id)).Where(u => u.AcceptedInvite == false).ToList();
            var accepted = ctx.UserMeetings.Where(u => u.UserID.Equals(id)).Where(u => u.AcceptedInvite == true).ToList();

            foreach (var accept in accepted)
            {
                var meeting = ctx.Meetings.FirstOrDefault(m => m.MeetingID == accept.MeetingID);
                creator.Add(meeting);
            }

            creator = creator.OrderBy(u => u.MeetingStartDate).ToList();
            creator = creator.Where(u => u.MeetingStartDate >= DateTime.Now).ToList();

            var model = new ProfilePageViewModel
            {
                userId = userId,
                ImagePath = path,
                Firstname = Users.UserFirstname,
                Lastname = Users.UserLastname,
                Email = identityUser.Email,
                PhoneNumber = Users.UserPhoneNumber,
                Team = team,
                OtherUserID = id,
                FollowedID = id,
                //UserIsFollowed = isFollowed,
                MeetingModels = creator,
                UserMeetings = invited,
                UserIsFollowed = PersonIsFollowed,
                Position = Users.UserPosition,
                UserEmailNotification = Users.UserEmailNotification,
                UserPmNotification = Users.UserPmNotification,
                UserSmsNotification = Users.UserSmsNotification,
            };

            return View("ShowInfo", model);
        }

        public ActionResult AcceptMeeting(int meetingId)
        {
            var ctx = new OruBloggenDbContext();

            var userId = User.Identity.GetUserId();
            
            ctx.UserMeetings.FirstOrDefault(m => m.MeetingID == meetingId && m.UserID.Equals(userId)).AcceptedInvite = true;
            ctx.SaveChanges();


            return RedirectToAction("ShowInfo");

        }

  
        public ActionResult CancelMeeting(int meetingId, string title, DateTime startDate)
        {
            var ctx = new OruBloggenDbContext();
            //var meetingActive = ctx.Meetings.FirstOrDefault(m => m.MeetingID == meetingId).MeetingActive;

            if (ctx.Meetings.FirstOrDefault(m => m.MeetingID == meetingId).MeetingActive)
            {
                ctx.Meetings.FirstOrDefault(m => m.MeetingID == meetingId).MeetingActive = false;

                var appCtx = new ApplicationDbContext();

                var userMeetings = ctx.UserMeetings.Where(m => m.MeetingID == meetingId);
                var emails = new List<string>();
                foreach (var user in userMeetings)
                {
                    emails.Add(appCtx.Users.FirstOrDefault(u => u.Id.Equals(user.UserID)).Email);
                }
                ctx.SaveChanges();

                var notificationController = new NotificationController();
                notificationController.SendEmail(emails, "Mötet är inställt", title + " " + startDate.ToShortDateString() + " är inställt.");
            }
            return RedirectToAction("ShowInfo");
        }

    }
}