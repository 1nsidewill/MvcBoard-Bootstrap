using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MvcBoard.Models
{
    public class User
    {
        [Key]
        public int UserNo { get; set; }

        [Required(ErrorMessage = "이름을 입력하세요!")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "ID를 입력하세요!")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "비밀번호를 입력하세요!")]
        public string UserPassword { get; set; }

        [Required]
        public DateTime RegisterDate { get; set; }
    }
}