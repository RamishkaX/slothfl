using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;

namespace SlothFreelance.Models
{
    public class WorkOnTask
    {
        [Key]
        public int WorkId { get; set; }
        [Display(Name = "Комментарий")]
        public string Comment { get; set; }
        [Display(Name = "Файл")]
        public string File { get; set; }
        public int? UserId { get; set; }
        public int? TaskId { get; set; }
        public Users Users { get; set; }
        public Tasks Tasks { get; set; }
    }
}