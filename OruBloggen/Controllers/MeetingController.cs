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
            foreach(var item in ctx.Users)
            {
                meetingView.Users.Add(
                    new SelectListItem() { Text = item.UserFirstname + " " + item.UserLastname, Value = item.UserID }
                    );
            }
            return View(meetingView);
        }

        //public void ListUsers()
        //{
        //    var ctx = new OruBloggenDbContext();
        //    List<SelectListItem> userList = new List<SelectListItem>();
        //    foreach (var item in ctx.Users)
        //    {
        //        userList.Add(new SelectListItem() { Text = item.UserFirstname + " " + item.UserLastname, Value = item.UserID });
        //    }
        //    ViewData["Users"] = userList;
        //}

        [HttpPost]
        public ActionResult CreateMeeting(MeetingModel model)
        {
            var ctx = new OruBloggenDbContext();

            ctx.Meetings.Add(new MeetingModel
            {
                MeetingTitle = model.MeetingTitle,
                MeetingDesc = model.MeetingDesc,
                MeetingStartDate = model.MeetingStartDate,
                MeetingEndDate = model.MeetingEndDate,
                MeetingUserID = User.Identity.GetUserId()
            });

            ctx.SaveChanges();

            foreach(var item in ViewData["SelectedUsers"] as List<SelectListItem>) {
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