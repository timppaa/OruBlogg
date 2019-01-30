using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OruBloggen.Models
{
    public class ProjectModel
    {
        [Key]
        public int ProjectID { get; set; }
        public string ProjectType { get; set; }
        public string ProjectName { get; set; }
        public string ProjectDesc { get; set; }
        public string ProjectStatus { get; set; }
        public string ProjectFilePath { get; set; }
        [ForeignKey("ProjectTeamID")]
        public virtual TeamModel TeamModel { get; set; }
        public int ProjectTeamID { get; set; }
        [ForeignKey("ProjectUserID")]
        public virtual UserModel UserModel { get; set; }
        public string ProjectUserID { get; set; }
    }
}