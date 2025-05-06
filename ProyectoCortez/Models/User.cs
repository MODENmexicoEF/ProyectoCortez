using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoCortez.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public byte RoleID { get; set; }
        public DateTime CreatedAt { get; set; }

        [ForeignKey("RoleID")]
        public Role Role { get; set; }
    }
}