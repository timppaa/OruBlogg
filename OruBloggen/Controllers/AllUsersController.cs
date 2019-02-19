using OruBloggen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OruBloggen.Controllers
{
    [Authorize]
    public class AllUsersController : Controller
    {
        // GET: AllUsers
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ShowAllUsers()
        {
            var ctx = new OruBloggenDbContext();
            var userList = new List<UserModel>();

            foreach(var item in ctx.Users)
            {
                userList.Add(item);
            }
            var model = new AllUsersViewModelcs
            {
                AllUsers = userList,
                
            };
            return View(model);
        }
    }
}