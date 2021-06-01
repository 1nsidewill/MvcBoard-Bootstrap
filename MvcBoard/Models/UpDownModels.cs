using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MvcBoard.Models
{
    public class Upload
    {
        [DataType(DataType.Upload)]
        [Display(Name = "Select File")]
        public HttpPostedFileBase files { get; set; }
    }

    public class UpDown
    {
        public int FileId { get; set; }
        [Display(Name = "Uploaded File")]
        public String FileTitle { get; set; }
        public byte[] FileContent { get; set; }
    }
}