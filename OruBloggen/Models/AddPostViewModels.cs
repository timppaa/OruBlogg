using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OruBloggen.Models
{
    public class AddPostViewModel
    {
        [Required(ErrorMessage = "Inlägget måste ha en titel.")]
        public string PostTitle { get; set; }

        [Required(ErrorMessage = "Inlägget måste ha en text.")]
        public string PostText { get; set; }

        [FileExtensions(Extensions = "pdf, doc, docx, zip, rar, pptx, ppt, xls, xlsx, txt, word, jpg, png, jpeg, gif", ErrorMessage = "Filformatet stöds ej.")]
        public string File { get; set; }

        public bool PostFormal { get; set; }
        public string CategoryFormal { get; set; }
        public string CategoryInformal { get; set; }        
    }
}