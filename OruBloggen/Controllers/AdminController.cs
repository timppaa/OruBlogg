using OruBloggen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OruBloggen.Controllers
{
    public class AdminController : Controller
    {
        public OruBloggenDbContext ctx = new OruBloggenDbContext();
        public AdminViewModel AVM = new AdminViewModel();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Posts ()
        {

            ctx.Posts.Add(new PostModel() {
                PostTitle = "Hej",
                PostDate = DateTime.Now,
                PostFormal = true,
                PostText = "Första Inlägget",
                PostUserID = "5a55a092-765d-4948-bf31-e43062ba3c40",
            });
            ctx.SaveChanges();
            

            AVM.postModels = ctx.Posts.ToList();

            return View(AVM);
        }
    }
}