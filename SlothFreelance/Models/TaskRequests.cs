using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace SlothFreelance.Models
{
    public class TaskRequests
    {
        [Key]
        public int RequestId { get; set; }
        [Display(Name = "Текст заявки")]
        [StringLength(600, MinimumLength = 100, ErrorMessage = "Недопустимая длина строки. Допустимое количество символов: от 100 до 600")]
        [Required]
        public string RequestText { get; set; }
        public string Status { get; set; }
        [Display(Name = "Цена")]
        [Required]
        public int Price { get; set; }
        [Required]
        public int? TaskId { get; set; }
        public int? UserId { get; set; }
        public Tasks Task { get; set; }
        public Users User { get; set; }
    }
}