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
        public OruBloggenDbContext ctx = new OruBloggenDbContext();
        // GET: Meeting
        public ActionResult Meeting()
        {
            return View();
        }

        public List<UserModel> SearchUser(string searchString)
        {
            
            var userList = ctx.Users.Where(u => String.Concat(u.UserFirstname, " ", u.UserLastname)
                                    .Contains(searchString) || 
                                    searchString == null).ToList();
          
            return userList;
        }

        //public ActionResult ListUsersLB(string searchString)
        //{
        //    var userList = SearchUser(searchString);
        //    var users = new List<SelectListItem>();
        //    foreach (var item in userList)
        //    {
        //        users.Add(new SelectListItem
        //        {
        //            Text = item.UserFirstname + " " + item.UserLastname,
        //            Value = item.UserID
        //        });
        //    }

        //    var meetingView = new MeetingViewModel
        //    {
        //        Users = users,
        //        SelectedUsers = new List<SelectListItem>()
        //    };

        //    return View(meetingView);
        //}


        public ActionResult AddUserToMeeting(int meetingID, string fullname)
        {
            var nameArray = fullname.Split(' ');
            var firstname = nameArray[0];
            var lastname = nameArray[1];
            var user = ctx.Users.FirstOrDefault(u => u.UserFirstname == firstname && u.UserLastname == lastname);
            
                ctx.UserMeetings.Add(new UserMeetingModel
                {
                    MeetingID = ctx.Meetings.OrderByDescending(m => m.MeetingID).First().MeetingID,
                    UserID = user.UserID
                });
         
            ctx.SaveChanges();
            return RedirectToAction("MeetingDetails", new { id = meetingID });
        }

        [HttpPost]
        public ActionResult CreateMeeting(MeetingViewModel model)
        {

            var meeting = ctx.Meetings.Add(new MeetingModel
            {
                MeetingTitle = model.Meeting.MeetingTitle,
                MeetingDesc = model.Meeting.MeetingDesc,
                MeetingStartDate = model.Meeting.MeetingStartDate,
                MeetingEndDate = model.Meeting.MeetingEndDate,
                MeetingUserID = User.Identity.GetUserId()
            });
            if(ModelState.IsValid) { 
                ctx.SaveChanges();
            }
            return RedirectToAction("MeetingDetails", new { id = meeting.MeetingID});
        }

        public ActionResult MeetingDetails(int? id, string searchString)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var userList = from s in ctx.Users
                           select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                userList = ctx.Users.Where(s => s.UserLastname.Contains(searchString)
                                       || s.UserFirstname.Contains(searchString));
            }
            MeetingModel meeting = ctx.Meetings.Find(id);
            var usermeetingList = ctx.UserMeetings.Where(i => i.MeetingID == id);
            var members = new List<UserModel>();
            var userlisttt = ctx.Users.ToList();
            foreach (var item in usermeetingList)
            {
                var userid = item.UserID;
                userlisttt.Where(u => u.UserID.Equals(userid));
                members.AddRange(userlisttt);
            }

            MeetingViewModel meetingViewModel = new MeetingViewModel();

            if (meeting == null)
            {

                Console.WriteLine("Test");
                return HttpNotFound();
            }
            meetingViewModel.Meeting = meeting;
            meetingViewModel.AllUsers = userList;
            meetingViewModel.Members = members;
            return View(meetingViewModel);
        }


    }
}