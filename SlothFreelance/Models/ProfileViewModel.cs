using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SlothFreelance.Models
{
    public class ProfileViewModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Image { get; set; }
        public Roles Role { get; set; }
        public bool IsMyPage { get; set; }
        public ICollection<Services> Services { get; set; }
        public ProfileViewModel()
        {
            Services = new List<Services>();
        }
    }
}