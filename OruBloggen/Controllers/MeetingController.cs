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

            foreach (var item in model.SelectedUserIds)
            {
                ctx.UserMeetings.Add(new UserMeetingModel
                {
                    MeetingID = ctx.Meetings.OrderByDescending(m => m.MeetingID).First().MeetingID,
                    UserID = item
                });
            };
            ctx.SaveChanges();

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
    }
}