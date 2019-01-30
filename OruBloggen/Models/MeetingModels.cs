using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OruBloggen.Models
{
    public class MeetingModel
    {
        [Key]
        public int MeetingID { get; set; }
        public string MeetingTitle { get; set; }
        public string MeetingDesc { get; set; }
        public DateTime MeetingDate { get; set; }
        public bool MeetingActive { get; set; }
        [ForeignKey("MeetingUserID")]
        public virtual UserModel UserModel { get; set; }
        public string MeetingUserID { get; set; }

        [InverseProperty("MeetingModel")]
        public virtual List<UserMeetingModel> UserMeetingModel { get; set; }

        public MeetingModel()
        {
            MeetingActive = true;
        }
    }
}