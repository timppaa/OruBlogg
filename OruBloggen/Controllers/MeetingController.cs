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
        public ActionResult Meeting()
        {

            var meetingView = ListUsersBeginning();

            return View(meetingView);
        }

        public MeetingViewModel ListUsersBeginning()
        {
            var ctx = new OruBloggenDbContext();
            var userList = ctx.Users;

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
            var ctx = new OruBloggenDbContext();

            var userList = ctx.Users.Where(u => String.Concat(u.UserFirstname, " ", u.UserLastname)
                                    .Contains(searchString) ||
                                    searchString == null).ToList();

            
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

                var appCtx = new ApplicationDbContext();
                var emails = new List<string>();
                if (model.SelectedUserIds != null)
                {
                    foreach (var item in model.SelectedUserIds)
                    {
                        ctx.UserMeetings.Add(new UserMeetingModel
                        {
                            MeetingID = ctx.Meetings.OrderByDescending(m => m.MeetingID).First().MeetingID,
                            UserID = item
                        });
                        emails.Add(appCtx.Users.FirstOrDefault(u => u.Id.Equals(item)).Email);
                    }
                }

                ctx.SaveChanges();

                var notificationController = new NotificationController();
                var body = "Du har blivit inbjuden till " + model.Meeting.MeetingTitle + Environment.NewLine +
                           "Startdatum: " + model.Meeting.MeetingStartDate.ToShortDateString() + " "
                           + model.Meeting.MeetingStartDate.ToShortTimeString() + Environment.NewLine +

                           "Slutdatum: " + model.Meeting.MeetingEndDate.ToShortDateString() + " "
                           + model.Meeting.MeetingEndDate.ToShortTimeString() + Environment.NewLine +

                           "Beskrivning: " + model.Meeting.MeetingDesc;
                notificationController.SendEmail(emails, "Inbjudan till möte", body);

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
        public ActionResult ListCreatedMeetings()
        {
            var ctx = new OruBloggenDbContext();
            var userId = User.Identity.GetUserId();
            
            var meetings = ctx.Meetings.Where(m => m.MeetingUserID.Equals(userId)).ToList();
            foreach (var meeting in meetings)
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

            return View(meetings);
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
            return RedirectToAction("ListCreatedMeetings");
        }
    }
}