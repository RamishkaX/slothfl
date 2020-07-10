using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace SlothFreelance.Models
{
    public class Tasks
    {
        [Key]
        public int TaskId { get; set; }
        [Display(Name = "Название задачи")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Недопустимая длина строки. Допустимое количество символов: от 8 до 100")]
        [Required]
        public string TaskName { get; set; }
        [Display(Name = "Описание задачи")]
        [StringLength(200, MinimumLength = 50, ErrorMessage = "Недопустимая длина строки. Допустимое количество символов: от 50 до 150")]
        [DataType(DataType.MultilineText)]
        [Required]
        public string Description { get; set; }
        [Display(Name = "Цена")]
        [Range(100, int.MaxValue, ErrorMessage = "Цена не может быть ниже 100 рублей")]
        [Required]
        public int Price { get; set; }
        [Display(Name = "Дата публикации")]
        [DataType(DataType.Date)]
        public DateTime PublishDate { get; set; }
        [Display(Name = "Статус")]
        public string Status { get; set; }
        public int? CategoryId { get; set; }
        public int? UserId { get; set; }
        public Categories Category { get; set; } 
        public Users Users { get; set; }
        public ICollection<TaskRequests> Requests { get; set; }
        public ICollection<WorkOnTask> WorkOnTasks { get; set; }
        public Tasks()
        {
            Requests = new List<TaskRequests>();
            WorkOnTasks = new List<WorkOnTask>();
        }
    }

    public class CreateTask
    {
        [Key]
        public int TaskId { get; set; }
        [Display(Name = "Название задачи")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Недопустимая длина строки. Допустимое количество символов: от 8 до 100")]
        [Required]
        public string TaskName { get; set; }
        [Display(Name = "Описание задачи")]
        [StringLength(200, MinimumLength = 50, ErrorMessage = "Недопустимая длина строки. Допустимое количество символов: от 50 до 150")]
        [DataType(DataType.MultilineText)]
        [Required]
        public string Description { get; set; }
        [Display(Name = "Цена")]
        [Range(100, int.MaxValue, ErrorMessage = "Цена не может быть ниже 100 рублей")]
        [Required]
        public int Price { get; set; }
        [Display(Name = "Категория")]
        [Required]
        public int? CategoryId { get; set; }
    }
}