using System;
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

        [Required]
        public string UserId { get; set; }


        [Display(Name = "Files")]
        public string FileTitle { get; set; }

        public string FileContent { get; set; }
        [Display(Name = "첨부 파일")]
        public HttpPostedFileBase upfiles { get; set; }
    }

    public class FileDetailsModel
    {
        [Display(Name = "Uploaded File")]
        public String FileName { get; set; }
        public byte[] FileContent { get; set; }
    }
}