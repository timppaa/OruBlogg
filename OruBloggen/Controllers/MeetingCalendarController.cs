﻿using Ical.Net.Interfaces.Serialization;
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
    [AuthorizeUser, Authorize]
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
            var ctx = new OruBloggenDbContext();
            
                List<CalenderViewModel> meetings = new List<CalenderViewModel>();

                foreach (var item in ctx.Meetings.ToList())
                {
                var creator = ctx.Users.FirstOrDefault(u => u.UserID.Equals(item.MeetingUserID));
                var teamName = ctx.Teams.FirstOrDefault(t => t.TeamID == item.UserModel.UserTeamID).TeamName;

                meetings.Add(new CalenderViewModel
                {
                    MeetingTitle = item.MeetingTitle,
                    MeetingDesc = item.MeetingDesc,
                    MeetingEndDate = item.MeetingEndDate,
                    MeetingStartDate = item.MeetingStartDate,
                    MeetingID = item.MeetingID,
                    MeetingCreator = creator.UserFirstname + " " + creator.UserLastname,
                    MeetingActive = item.MeetingActive,
                    TeamName = teamName
                    });
                }
                return new JsonResult { Data = meetings, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            
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
                var user = ctx.Users.FirstOrDefault(u => u.UserID.Equals(meeting.MeetingUserID));
                sb.AppendLine("BEGIN:VEVENT");
                sb.AppendLine("UID:" + meeting.MeetingID);
                sb.AppendLine("ORGANIZER:" + user.UserFirstname + " " + user.UserLastname);
                sb.AppendLine("SUMMARY;LANGUAGE=en-us:" + meeting.MeetingTitle);
                sb.AppendLine("DESCRIPTION:" + meeting.MeetingDesc);
                sb.AppendLine(string.Format("DTSTART:{0:yyyyMMddTHHmmssZ}", meeting.MeetingStartDate));
                sb.AppendLine(string.Format("DTEND:{0:yyyyMMddTHHmmssZ}", meeting.MeetingEndDate));
                sb.AppendLine("END:VEVENT");
            }            
            sb.AppendLine("END:VCALENDAR");

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());

            return File(bytes, "text/calendar", "calendar.ics");
            }
        }
    }
