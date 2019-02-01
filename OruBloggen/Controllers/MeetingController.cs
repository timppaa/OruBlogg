using Microsoft.AspNet.Identity;
using OruBloggen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OruBloggen.Controllers
{
    public class MeetingController : Controller
    {
        // GET: Meeting
        public ActionResult Meeting()
        {
            var ctx = new OruBloggenDbContext();
            var meetingView = new MeetingViewModel();
            var users = new List<SelectListItem>();

            foreach (var item in ctx.Users)
            {
                users.Add(
                    new SelectListItem() { Text = item.UserFirstname + " " + item.UserLastname, Value = item.UserID }
                    );
            }
            meetingView.Users = users;
            meetingView.SelectedUsers = new List<SelectListItem>();
            return View(meetingView);
        }

        //public void ListUsers()
        //{
        //    var ctx = new OruBloggenDbContext();
        //    List<SelectListItem> users = new List<SelectListItem>();
        //    foreach (var item in ctx.Users)
        //    {
        //        users.Add(new SelectListItem() { Text = item.UserFirstname + " " + item.UserLastname, Value = item.UserID });
        //    }
        //    ViewData["Users"] = users;
        //}

        [HttpPost]
        public ActionResult CreateMeeting(MeetingViewModel model)
        {
            var ctx = new OruBloggenDbContext();

            ctx.Meetings.Add(new MeetingModel
            {
                MeetingTitle = model.Meeting.MeetingTitle,
                MeetingDesc = model.Meeting.MeetingDesc,
                MeetingStartDate = model.Meeting.MeetingStartDate,
                MeetingEndDate = model.Meeting.MeetingEndDate,
                MeetingUserID = User.Identity.GetUserId()
            });

            ctx.SaveChanges();

            foreach(var item in model.SelectedUsers) {
                ctx.UserMeetings.Add(new UserMeetingModel
                {
                    MeetingID = ctx.Meetings.Last().MeetingID,
                    UserID = item.Value
                });
            };

            ctx.SaveChanges();

            return RedirectToAction("Meeting");
        }
    }
}