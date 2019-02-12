using Microsoft.AspNet.Identity;
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
    public class NotificationController : Controller
    {
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

        public void SendMeetingPm(string userId, List<UserModel> userModels, string title, string description, DateTime start, DateTime end)
        {
            MessageController messageController = new MessageController();

            description +=  "Startdatum: " + start.ToShortDateString() + start.ToShortTimeString() + 
                "Slutdatum: " + end.ToShortDateString() + end.ToShortTimeString();

            foreach(var item in userModels)
            {
                if(item.UserPmNotification)
                {
                    messageController.SendPmNotification(userId, item.UserID, title, description);
                }
            }
        }
    }
}