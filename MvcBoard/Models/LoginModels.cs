using System.ComponentModel.DataAnnotations;


namespace MvcBoard.Models
{
    public class Login
    {
        [Required(ErrorMessage = "사용자 ID를 입력하세요.")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "사용자 비밀번호를 입력하세요.")]
        public string UserPassword { get; set; }
    }
}