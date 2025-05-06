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
    public class Response
    {
        [Key]
        public long ResponseID { get; set; }

        public int UserID { get; set; }
        public string QuestionnaireID { get; set; }
        public DateTime TakenAt { get; set; }
        public byte TotalScore { get; set; }
        public string Interpretation { get; set; }

        [ForeignKey("UserID")]
        public User User { get; set; }

        [ForeignKey("QuestionnaireID")]
        public Questionnaire Questionnaire { get; set; }
    }
}