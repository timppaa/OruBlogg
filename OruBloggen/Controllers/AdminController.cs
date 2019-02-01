using OruBloggen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

            

            AVM.postModelList = ctx.Posts.ToList();
            AVM.userModelList = ctx.Users.ToList();
            return View(AVM);
        }

        public ActionResult PostDetails (int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AVM.postModel = ctx.Posts.Find(id);
            if (AVM.postModel == null)
            {
                Console.WriteLine("Test");
                return HttpNotFound();
            }
            return View(AVM);
        }

        public ActionResult DeletePost(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AVM.postModel = ctx.Posts.Find(id);
            AVM.userModelList = ctx.Users.ToList();
            if (AVM.postModel == null)
            {
                return HttpNotFound();
            }
            return View(AVM);
        }

        // POST: PostModels/Delete/5
        [HttpPost, ActionName("DeletePost")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PostModel postModel = ctx.Posts.Find(id);
            
            ctx.Posts.Remove(postModel);
            ctx.SaveChanges();
            return RedirectToAction("Posts");
        }
    }
}