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

        [Required]
        [Display(Name = "Title")]
        [DataType(DataType.DateTime)]
        public string MeetingTitle { get; set; }

        [Required]
        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        public string MeetingDesc { get; set; }

        [Required]
        [Display(Name = "Start time")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-ddThh:mm:ss}")]
        public DateTime MeetingStartDate { get; set; }

        [Required]
        [Display(Name ="End time")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-ddThh:mm:ss}")]
        public DateTime MeetingEndDate { get; set; }

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