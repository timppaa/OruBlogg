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
        public ActionResult Meeting(string searchString)
        {
            var userList = SearchUser(searchString);

            var users = new List<SelectListItem>();
            foreach (var item in userList)
            {
                users.Add(new SelectListItem
                {
                    Text = item.UserFirstname + " " + item.UserLastname,
                    Value = item.UserID
                });
            }

            var meetingView = new MeetingViewModel
            {
                Users = users,
                SelectedUsers = new List<SelectListItem>()
            };

            return View(meetingView);
        }

        public List<UserModel> SearchUser(string searchString)
        {
            var ctx = new OruBloggenDbContext();

            
            var userList = ctx.Users.Where(u => String.Concat(u.UserFirstname, " ", u.UserLastname)
                                    .Contains(searchString) || 
                                    searchString == null).ToList();
          
            return userList;
        }

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

            foreach(var item in model.SelectedUserIds) {
                ctx.UserMeetings.Add(new UserMeetingModel
                {
                    MeetingID = ctx.Meetings.OrderByDescending(m => m.MeetingID).First().MeetingID,
                    UserID = item
                });
            };
            ctx.SaveChanges();

            return RedirectToAction("Meeting");
        }

        //GET
        public ActionResult YourMeetings()
        {
            var userId = User.Identity.GetUserId();
            var ctx = new OruBloggenDbContext();

            var model = ctx.Meetings.Where(m => m.MeetingUserID.Equals(userId)).ToList();

            return View(model);
        }

        public ActionResult CancelMeeting(int meetingId)
        {
            var ctx = new OruBloggenDbContext();

            ctx.Meetings.FirstOrDefault(m => m.MeetingID == meetingId).MeetingActive = false;

            ctx.SaveChanges();

            return RedirectToAction("YourMeetings");
        }
    }
}