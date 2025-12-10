using System.ComponentModel.DataAnnotations;

namespace ScheduleApp.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Введите логин.")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Введите пароль.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
