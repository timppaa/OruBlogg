using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OruBloggen.Models
{
    public class ReportModel
    {
        [Key]
        public int ReportID { get; set; }
        public string ReportReason { get; set; }

        [InverseProperty("ReportModel")]
        public virtual List<PostReportModel> PostReportModels { get; set; }

    }
}