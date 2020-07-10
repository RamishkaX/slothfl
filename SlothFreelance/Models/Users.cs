using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SlothFreelance.Models
{
    public class Users
    {
        [Key]
        public int UserId { get; set; }
        [Display(Name = "Имя")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Недопустимая длина строки. Допустимое количество символов: от 2 до 50")]
        [Required]
        public string UserName { get; set; }
        [Display(Name = "Страна")]
        [Required]
        public string Country { get; set; }
        [Display(Name = "Город")]
        [Required]
        public string City { get; set; }
        [Display(Name = "Номер телефона")]
        [StringLength(17, MinimumLength = 11, ErrorMessage = "Недопустимая длина строки")]
        [Required]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }
        [Display(Name = "Почта")]
        [Remote("CheckEmail", "Account", ErrorMessage = "Эта почта уже используется")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Некорректный адрес")]
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Display(Name = "Пароль")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Недопустимая длина пароля. Допустимое количество символов: от 8 до 20")]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [HiddenInput(DisplayValue = false)]
        [Display(Name = "Аватарка")]
        [DataType(DataType.ImageUrl)]
        public string Image { get; set; }
        [Display(Name = "Деньги")]
        public int Money { get; set; }
        [Display(Name = "Роль")]
        [Required]
        [Range(1, 2, ErrorMessage = "Неверный параметр")]
        public int? RoleId { get; set; }
        public Roles Role { get; set; }
        public ICollection<Tasks> Tasks { get; set; }
        public ICollection<Services> Services { get; set; }
        public ICollection<TaskRequests> TaskRequests { get; set; }
        public Users()
        {
            Tasks = new List<Tasks>();
            Services = new List<Services>();
            TaskRequests = new List<TaskRequests>();
        }
    }

    public class LoginModel
    {
        [Display(Name = "Почта")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Некорректный адрес")]
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Display(Name = "Пароль")]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

    public class RegisterModel : Users
    {
        [Display(Name = "Повторите пароль")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [Required]
        [DataType(DataType.Password)]
        public string PasswordAgain { get; set; }
    }

    public class ResetPassword
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Почта")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Некорректный адрес")]
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }

    public class ResetCode
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Код")]
        [Required]
        public string Code { get; set; }
        [Display(Name = "Почта")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Некорректный адрес")]
        [Required]
        [DataType(DataType.EmailAddress)]
        [HiddenInput]
        public string Email { get; set; }
    }

    public class MoneyModel
    {
        [Key]
        public int UserId { get; set; }
        [Display(Name = "Деньги")]
        [Range(100, int.MaxValue, ErrorMessage = "Цена не может быть ниже 100 рублей")]
        public int Money { get; set; }
    }
}