using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SlothFreelance.Models
{
    public class Services
    {
        [Key]
        public int ServiceId { get; set; }
        [DisplayName("Название услуги")]
        [Required]
        public string ServiceName { get; set; }
        [DisplayName("Изображение услуги")]
        public string ServiceImage { get; set; }
        [HiddenInput]
        public int UserId { get; set; }
        public Users User { get; set; }
    }

    public class AddService
    {
        [Key]
        public int ServiceId { get; set; }
        [DisplayName("Название услуги")]
        [Required]
        public string ServiceName { get; set; }
    }

    public class ServiceImageEdit
    {
        [Key]
        [HiddenInput]
        [Required]
        public int ServiceId { get; set; }
        [DisplayName("Изображение услуги")]
        public string ServiceImage { get; set; }
    }
}