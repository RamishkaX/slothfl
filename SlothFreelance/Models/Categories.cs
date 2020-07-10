using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SlothFreelance.Models
{
    public class Categories
    {
        [Key]
        public int CategoryId { get; set; }
        [Display(Name = "Название категории")]
        [Required]
        public string CategoryName { get; set; }
        public ICollection<Tasks> Tasks { get; set; }
        public Categories()
        {
            Tasks = new List<Tasks>();
        }
    }
}