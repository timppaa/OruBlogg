using Microsoft.AspNet.Identity;
using OruBloggen.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OruBloggen.Controllers
{
    [Authorize]
    public class MessageController : Controller
    {
        // GET: Message
        [AuthorizeUser]
        public ActionResult Index()
        {
            ListUsers();
            return View();       
        }

        [AuthorizeUser]
        public ActionResult SendMessage(MessageViewModel model, string Users)
        {
            //ListUsers();
            var ctx = new OruBloggenDbContext();
            var userId = User.Identity.GetUserId();
            var sender = ctx.Users.Find(userId);
            var name = sender.UserFirstname + " " + sender.UserLastname;
            var message = name + " har skickat ett meddelande: " + model.MessageText;

            ctx.Messages.Add(new MessageModel
            {
                MessageReceiverID = Users,
                MessageSenderID = userId,
                MessageTitle = model.MessageTitle,
                MessageText = message,
            });

            ctx.SaveChanges();

            return RedirectToAction("index");
        }

        [AuthorizeUser]
        public void SendPmNotification(string userId, string receiverId, string title, string desc)
        {
            //ListUsers();
            var ctx = new OruBloggenDbContext();
            //var userId = User.Identity.GetUserId();

            ctx.Messages.Add(new MessageModel
            {
                MessageReceiverID = receiverId,
                MessageSenderID = userId,
                MessageTitle = title,
                MessageText = desc,
            });

            ctx.SaveChanges();

        }

        [AuthorizeUser]
        public ActionResult ReturnResponsePage(string userId)
        {
            var ctx = new OruBloggenDbContext();
            var items = new List<SelectListItem>();
            var user = ctx.Users.Find(userId);
            
                items.Add(new SelectListItem()
                {
                    Text = user.UserFirstname + " " + user.UserLastname,
                    Value = user.UserID,
                });

            ViewData["Users"] = items;

            var model = new MessageViewModel()
            {
                MessageReceiverID = userId,
                ReceiverName = user.UserFirstname + " " + user.UserLastname
                
            };
            return View("RespondToMessage", model);
        }

        [AuthorizeUser]
        public ActionResult RespondToMessage(string MessageTitle, string MessageText, string Users)
        {
            
            var ctx = new OruBloggenDbContext();
            var userId = User.Identity.GetUserId();
            var sender = ctx.Users.Find(userId);
            var name = sender.UserFirstname + " " + sender.UserLastname;
            var message = name + " har skickat ett meddelande: " + MessageText;

            ctx.Messages.Add(new MessageModel
            {
                MessageReceiverID = Users,
                MessageSenderID = userId,
                MessageTitle = MessageTitle,
                MessageText = message
            });

            ctx.SaveChanges();

            return RedirectToAction("ShowMessages");
        }

        [AuthorizeUser]
        public void ListUsers()
        {
            var ctx = new OruBloggenDbContext();
            List<SelectListItem> List = new List<SelectListItem>();
            foreach (var item in ctx.Users)
            {
                List.Add(new SelectListItem() { Text = item.UserFirstname + " " + item.UserLastname, Value = item.UserID });
            }
            ViewData["Users"] = List;
        }

        [AuthorizeUser]
        public ActionResult ShowMessages()
        {
            var ctx = new OruBloggenDbContext();
            var userId = User.Identity.GetUserId();

            var messages = ctx.Messages
                .Where(m => m.MessageReceiverID == userId)
                .OrderByDescending(m => m.MessageID)
                .ToList();

            foreach (var item in messages)
            {
                item.MessageRead = true;
                ctx.Entry(item).State = EntityState.Modified;
            }
            ctx.SaveChanges();
        
            //var senders = new List<UserModel>();

            //foreach (var item in messages)
            //{
            //    senders.AddRange(ctx.Users
            //        .Distinct()
            //        .Where(s => s.UserID == item.MessageSenderID));

            //}

            var model = new MessageViewModel()
            {
                ListOfMessages = messages,
                //    ListOfSenders = senders.Distinct().ToList(),
            };

            return View(model);
        }

        public ActionResult Count()
        {
            var ctx = new OruBloggenDbContext();
            var userId = User.Identity.GetUserId();

            var messages = ctx.Messages.Where(u => u.MessageReceiverID == userId).Where(m => m.MessageRead == false).ToList();

            return Content(messages.Count.ToString());
        }

    }
}