using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoCortez.Models
{
    public class Question
    {
        [Key]
        public int QuestionID { get; set; }

        public string QuestionnaireID { get; set; }
        public byte NumberInForm { get; set; }
        public string Text { get; set; }
        public bool IsReverse { get; set; }

        [ForeignKey("QuestionnaireID")]
        public Questionnaire Questionnaire { get; set; }
    }
}