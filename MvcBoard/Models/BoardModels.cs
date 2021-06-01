﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MvcBoard.Models
{
    public class Board
    {
        [Key]
        public int board_postNo { get; set; }
        [Required(ErrorMessage = "제목을 입력하세요")]
        public string board_subject { get; set; }
        [Required(ErrorMessage = "내용을 입력하세요")]
        public string board_content { get; set; }
        public DateTime board_writeTime { get; set; }
        public int board_readCount { get; set; }
        public string board_name { get; set; }

        [DataType(DataType.Upload)]
        [Display(Name = "Select File")]
        public HttpPostedFileBase files { get; set; }
    }

    public class FileDetailsModel
    {
        public int FileId { get; set; }
        [Display(Name = "Uploaded File")]
        public String FileTitle {get; set; }
        public byte[] FileContent { get; set; }
    }
}