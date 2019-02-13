using OruBloggen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OruBloggen.Controllers
{
    public class MeetingCalendarController : Controller
    {
        // GET: Calendar
        public ActionResult Index()
        {
            var ctx = new OruBloggenDbContext();
            var meetings = ctx.Meetings.ToList();
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

            return View();
        }

        public JsonResult GetEvents()
        {
            using (var ctx = new OruBloggenDbContext())
            {
                List<CalenderViewModel> meetings = new List<CalenderViewModel>();

                var events = ctx.Meetings.ToList();
                foreach (var item in events)
                {
                    foreach (var user in ctx.Users.Where(u => u.UserID == item.MeetingUserID))
                    {
                        meetings.Add(new CalenderViewModel
                        {
                            MeetingTitle = item.MeetingTitle,
                            MeetingDesc = item.MeetingDesc,
                            MeetingEndDate = item.MeetingEndDate,
                            MeetingStartDate = item.MeetingStartDate,
                            MeetingID = item.MeetingID,
                            MeetingCreator = user.UserFirstname + " " + user.UserLastname,
                            MeetingActive = item.MeetingActive
                        });
                    }
                }
                return new JsonResult { Data = meetings, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        public JsonResult ListMembers(int meetingID)
        {
            var ctx = new OruBloggenDbContext();
            var lista = new List<UserMeetingModel>();
            var viewList = new List<string>();

            lista.AddRange(ctx.UserMeetings.Where(u => u.MeetingID.Equals(meetingID)));
            foreach (var item in lista)
            {
                foreach (var user in ctx.Users.Where(u => u.UserID == item.UserID))
                {
                    viewList.Add(" " + user.UserFirstname + " " + user.UserLastname);
                }
            }
            return new JsonResult { Data = viewList, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}