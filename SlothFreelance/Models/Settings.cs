using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SlothFreelance.Models
{
    public class Settings
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
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Некорректный адрес")]
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Display(Name = "Пароль")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Недопустимая длина пароля. Допустимое количество символов: от 8 до 20")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [HiddenInput(DisplayValue = false)]
        [Display(Name = "Аватарка")]
        [DataType(DataType.ImageUrl)]
        public string Image { get; set; }
        [Display(Name = "Роль")]
        [Required]
        [Range(1, 2, ErrorMessage = "Неверный параметр")]
        public int? RoleId { get; set; }
        [Display(Name = "Повторите пароль")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        public string PasswordAgain { get; set; }
    }

    public class ImageModel
    {
        [Display(Name = "Изображение")]
        [DataType(DataType.Upload)]
        public string UploadImage { get; set; }
    }
}