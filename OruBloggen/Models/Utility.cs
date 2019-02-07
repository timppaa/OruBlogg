using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
namespace OruBloggen.Models
{
    public static class Utility
    {



        public static bool isAdmin(string userId)
        {
            var ctx = new OruBloggenDbContext();

            return ctx.Users.FirstOrDefault(u => u.UserID == userId).UserIsAdmin;
        }

        public static string getCategoryById(int categoryId)
        {
            var ctx = new OruBloggenDbContext();
            return ctx.Categories.FirstOrDefault(u => u.CategoryID == categoryId).CategoryName;
        }

        //public static List<SelectListItem> getSearchedUsers(string searchString)
        //{
        //    var ctx = new OruBloggenDbContext();

        //    var userList = ctx.Users.Where(u => String.Concat(u.UserFirstname, " ", u.UserLastname)
        //                             .Contains(searchString) ||
        //                             searchString == null).ToList();

        //    var users = new List<SelectListItem>();
        //    foreach (var item in userList)
        //    {
        //        users.Add(new SelectListItem
        //        {
        //            Text = item.UserFirstname + " " + item.UserLastname,
        //            Value = item.UserID
        //        });
        //    }

        //    return users;
        //}
    }
}