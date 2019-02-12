using OruBloggen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;


namespace OruBloggen.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var list = NewsList();

            return View(list);
        }

        public List<NewsModel> NewsList()
        {
            var ctx = new OruBloggenDbContext();
            var list = new List<NewsModel>();
            list = ctx.News.OrderByDescending(n => n.NewsDate).Take(3).ToList();

            return list;
        }

    }
}