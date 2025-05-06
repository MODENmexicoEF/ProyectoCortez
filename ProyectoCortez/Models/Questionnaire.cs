using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace ProyectoCortez.Models
{
    public class Questionnaire
    {
        [Key]
        public string QuestionnaireID { get; set; }  // "GAD7", "PSS14"

        public string Name { get; set; }
        public byte MaxScore { get; set; }
    }
}