using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SlothFreelance.Models
{
    public class Roles
    {
        [Key]
        public int RoleId { get; set; }
        public string RoleName { get; set; }

        public ICollection<Users> Users { get; set; }
        public Roles()
        {
            Users = new List<Users>();
        }
    }
}