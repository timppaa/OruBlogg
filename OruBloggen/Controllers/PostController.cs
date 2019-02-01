using OruBloggen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OruBloggen.Controllers
{
    [Authorize]
    public class PostController : Controller
    {
        // GET: Post
        public ActionResult Post()
        {
            ListFormelItems();
            ListInformelItems();
            return View();
        }


        public void ListFormelItems ()
        {
            var ctx = new OruBloggenDbContext();
            List<SelectListItem> list = new List<SelectListItem>();

            foreach(var item in ctx.Categories)
            {
                if (item.IsFormel)
                {
                    list.Add(new SelectListItem() { Text = item.CategoryName, Value = item.CategoryID.ToString() });
                }
            }

            ViewData["CategoriesFormal"] = list;
        }

        public void ListInformelItems()
        {
            var ctx = new OruBloggenDbContext();
            List<SelectListItem> list = new List<SelectListItem>();

            foreach (var item in ctx.Categories)
            {
                if (!item.IsFormel)
                {
                    list.Add(new SelectListItem() { Text = item.CategoryName, Value = item.CategoryID.ToString() });
                }
            }

            ViewData["CategoriesInformal"] = list;
        }


    }
}