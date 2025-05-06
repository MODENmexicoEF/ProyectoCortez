using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoCortez.Models
{
    public class Student
    {
        [Key, ForeignKey("User")]
        public int UserID { get; set; }

        public string Nombre { get; set; }
        public byte Edad { get; set; }
        public string Sexo { get; set; }
        public string Municipio { get; set; }
        public string Carrera { get; set; }
        public int NoControl { get; set; }
        public int semestre { get; set; }

        public User User { get; set; }
    }
}