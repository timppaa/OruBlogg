﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OruBloggen.Models
{
    public class PostViewModel
    {
        public int PostID { get; set; }
        public string PostTitle { get; set; }
        public string PostText { get; set; }
        public DateTime PostDate { get; set; }
        public string PostFilePath { get; set; }
        public bool PostFormal { get; set; }
        public string PostSender { get; set; }
        public string PostCategory { get; set; }
    }
}