using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ProyectoCortez.Models
{
    public class Role
    {
        [Key]
        public byte RoleID { get; set; }
        public string RoleName { get; set; }
    }
}