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
            return View();
        }

        public List<UserModel> SearchUser(string searchString)
        {
            var ctx = new OruBloggenDbContext();

            
            var userList = ctx.Users.Where(u => String.Concat(u.UserFirstname, " ", u.UserLastname)
                                    .Contains(searchString) || 
                                    searchString == null).ToList();
          
            return userList;
        }

        public ActionResult ListUsersLB(string searchString)
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


        //public JsonResult GetSearchingData(string searchString)
        //{
        //    var ctx = new OruBloggenDbContext();


        //    var userList = ctx.Users.Where(u => String.Concat(u.UserFirstname, " ", u.UserLastname)
        //                            .Contains(searchString) ||
        //                            searchString == null).ToList();

        //    return Json(userList, JsonRequestBehavior.AllowGet);
        //}

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
            var ctx = new OruBloggenDbContext();
            var userList = SearchUser(searchString);
            MeetingModel meeting = ctx.Meetings.Find(id);

            var users = new List<SelectListItem>();
            foreach (var item in userList)
            {
                users.Add(new SelectListItem
                {
                    Text = item.UserFirstname + " " + item.UserLastname,
                    Value = item.UserID
                });
            }

            MeetingViewModel meetingViewModel = new MeetingViewModel();

            if (meeting == null)
            {

                Console.WriteLine("Test");
                return HttpNotFound();
            }
            meetingViewModel.Meeting = meeting;
            meetingViewModel.Users = users;
            meetingViewModel.SelectedUsers = new List<SelectListItem>();
            return View(meetingViewModel);
        }


    }
}