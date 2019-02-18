using Microsoft.AspNet.Identity;
using OruBloggen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

using Twilio.TwiML;
using Twilio.AspNet.Mvc;

namespace OruBloggen.Controllers
{
    [Authorize, AuthorizeUser]
    public class NotificationController : Controller
    {
        MessageController messageController = new MessageController();

        // GET: Notification
        public ActionResult Index()
        {
            return View();
        }

        public void SendEmail(List<string> emails, string subject, string body)
        {
            try
            {
                var senderEmail = new MailAddress("oru.notifications@gmail.com", "Oru Notification");
                var password = "Orunotice1!";

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,

                    Credentials = new NetworkCredential(senderEmail.Address, password)
                };

                using (var message = new MailMessage()
                {
                    Subject = subject,
                    Body = body
                })
                {
                    message.From = senderEmail;
                    foreach (var email in emails)
                    {
                        message.To.Add(new MailAddress(email));
                    }
                    smtp.Send(message);
                }
            }
            catch (Exception)
            {
                ViewBag.Error = "Something went wrong with sending email notification!";
            }
        }
        public ActionResult SendSms(string number, string body)
        {
            var accountSid = "AC7e22c16e4946115eb10eadea990ff8ec";
            var authToken = "649f135d51c828c66b12e2fd83b24408";


            try
            {
                TwilioClient.Init(accountSid, authToken);
                var from = new PhoneNumber("+46765193038");
                var to = new PhoneNumber("+46" + number);

                var message = MessageResource.Create(
                    from: from,
                    to: to,
                    body: body
                );


                return Content(message.Sid);

            }
            catch (Exception e)
            {
                return Content("fail hihi");
            }
        }



        public void SendMeetingPm(string userId, List<UserModel> userModels, string title, string description, DateTime start, DateTime end)
        {
            var messeage = "Du har blivit inbjuden till ett möte. " + "Innehåll: " + description + ". Startdatum: " + start.ToShortDateString() + ": " + start.ToShortTimeString() +
                ". Slutdatum: " + end.ToShortDateString() + ": " + end.ToShortTimeString() + ".";

            
            foreach(var item in userModels)
            {
                if(item.UserPmNotification)
                {
                    messageController.SendPmNotification(userId, item.UserID, title, messeage);
                }
            }
        }
        public void SendPostPm(string userid, string title, string text, DateTime date, bool isFormal, int categoryId)
        {
            var ctx = new OruBloggenDbContext();
            var userFollowers = ctx.Notifications.Where(u => u.FollowUserID == userid).ToList();
            var categoryFollowers = ctx.Notifications.Where(c => c.FollowCategoryID == categoryId).ToList();
            var user = ctx.Users.Find(userid);
            var name = user.UserFirstname + " " + user.UserLastname;
            var category = ctx.Categories.Find(categoryId).CategoryName;
            var formal = "";
            if(isFormal)
            {
                formal = "formellt";
            }
            else
            {
                formal = "informellt";
            }
            var message = name + " har gjort ett nytt " + formal + " inlägg. " + " Innehåll: " + text + " Datum: " + date.ToShortDateString() + ". I kategorin: " + category;
            foreach(var item in userFollowers)
            {
                var userPmNotification = ctx.Users.Find(item.UserID);
                if (userPmNotification.UserPmNotification)
                {
                    messageController.SendPmNotification(userid, item.UserID, title, message);
                }
            }
            foreach (var item in categoryFollowers)
            {
                var userPmNotification = ctx.Users.Find(item.UserID);
                if (userPmNotification.UserPmNotification)
                {
                    messageController.SendPmNotification(userid, item.UserID, title, message);
                }

            }
        }


    }
}