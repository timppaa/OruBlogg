using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OruBloggen.Models
{
    public class MessageViewModel
    {
        public string MessageSenderID { get; set; }
        public string MessageReceiverID { get; set; }
        public string MessageTitle { get; set; }
        public string MessageText { get; set; }
        public string ReceiverName { get; set; }

        public List<MessageModel> ListOfMessages { get; set; }
        public List<UserModel> ListOfSenders { get; set; }

    }
}