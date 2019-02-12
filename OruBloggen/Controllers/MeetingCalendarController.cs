using Ical.Net.Interfaces.Serialization;
using OruBloggen.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;


namespace OruBloggen.Controllers
{
    public class MeetingCalendarController : Controller
    {
        // GET: Calendar
        public ActionResult Index()
        {
            
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
      
        public ActionResult ExportToIcal()
        {
            var ctx = new OruBloggenDbContext();
            var meetings = ctx.Meetings.ToList();

            var sb = new StringBuilder();

            sb.AppendLine("BEGIN:VCALENDAR");
            sb.AppendLine("PRODID:OruCalendar");
            sb.AppendLine("VERSION:2.0");

            foreach (var meeting in meetings)
            {
                sb.AppendLine("BEGIN:VEVENT");
                sb.AppendLine("UID:" + meeting.MeetingID);
                sb.AppendLine("ORGANIZER:" + meeting.MeetingUserID);
                sb.AppendLine("SUMMARY;LANGUAGE=en-us:" + meeting.MeetingTitle);
                sb.AppendLine("DESCRIPTION:" + meeting.MeetingDesc);
                sb.AppendLine("CLASS:PUBLIC");             
                sb.AppendLine(string.Format("DTSTART:{0:yyyyMMddTHHmmssZ}", meeting.MeetingStartDate));
                sb.AppendLine(string.Format("DTEND:{0:yyyyMMddTHHmmssZ}", meeting.MeetingEndDate));
                sb.AppendLine("SEQUENCE:0");                
                sb.AppendLine("END:VEVENT");             
            }
            sb.AppendLine("END:VCALENDAR");

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());

            return File(bytes, "text/calendar", "calendar.ics");
            }
        }
    }
