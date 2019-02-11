using Microsoft.AspNet.Identity;
using OruBloggen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace OruBloggen.Controllers
{
    public class MeetingController : Controller
    {
        // GET: Meeting
        public ActionResult Meeting(string searchString)
        {

            var meetingView = ListUsersBeginning(searchString);

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

        public MeetingViewModel ListUsersBeginning(string searchString)
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

            return meetingView;
        }

        public JsonResult ListSearchedUsers(string searchString)
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
            return new JsonResult { Data = users, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }

        [HttpPost]
        public ActionResult CreateMeeting(MeetingViewModel model)
        {
            var ctx = new OruBloggenDbContext();

            var meeting = ctx.Meetings.Add(new MeetingModel
            {
                MeetingTitle = model.Meeting.MeetingTitle,
                MeetingDesc = model.Meeting.MeetingDesc,
                MeetingStartDate = model.Meeting.MeetingStartDate,
                MeetingEndDate = model.Meeting.MeetingEndDate,
                MeetingUserID = User.Identity.GetUserId()
            });
            ctx.SaveChanges();

            if (model.SelectedUserIds != null)
            {

                foreach (var item in model.SelectedUserIds)
                {
                    ctx.UserMeetings.Add(new UserMeetingModel
                    {
                        MeetingID = ctx.Meetings.OrderByDescending(m => m.MeetingID).First().MeetingID,
                        UserID = item
                    });
                };
                ctx.SaveChanges();
            }
            //return RedirectToAction("MeetingDetails", new { id = meeting.MeetingID});
            return RedirectToAction("Index", "MeetingCalendar");
        }

        public ActionResult MeetingDetails(int? id)
        {
            var ctx = new OruBloggenDbContext();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MeetingModel meeting = ctx.Meetings.Find(id);
            if (meeting == null)
            {
                Console.WriteLine("Test");
                return HttpNotFound();
            }
            return View(meeting);
        }

        //GET
        public ActionResult YourMeetings()
        {
            var userId = User.Identity.GetUserId();
            var ctx = new OruBloggenDbContext();

            var model = ctx.Meetings.Where(m => m.MeetingUserID.Equals(userId)).ToList();

            foreach (var meeting in model)
            {
                if (meeting.MeetingActive)
                {
                    if (DateTime.Now > meeting.MeetingEndDate)
                    {
                        meeting.MeetingActive = false;
                    }
                }
            }
            ctx.SaveChanges();

            return View(model);
        }

        public ActionResult CancelMeeting(int meetingId, string title, DateTime startDate)
        {
            var ctx = new OruBloggenDbContext();

            if (ctx.Meetings.FirstOrDefault(m => m.MeetingID == meetingId).MeetingActive)
            {
                ctx.Meetings.FirstOrDefault(m => m.MeetingID == meetingId).MeetingActive = false;

                var userMeetingList = ctx.UserMeetings.Where(m => m.MeetingID == meetingId);

                var appCtx = new ApplicationDbContext();

                var emails = new List<string>();
                foreach (var user in userMeetingList)
                {
                    emails.Add(appCtx.Users.FirstOrDefault(u => u.Id.Equals(user.UserID)).Email);
                }

                var notificationController = new NotificationController();
                notificationController.SendEmail(emails, "Mötet är inställt", title + " " + startDate.ToShortDateString() + " är inställt.");

                ctx.SaveChanges();
            }
            return RedirectToAction("YourMeetings");
        }
    }
}