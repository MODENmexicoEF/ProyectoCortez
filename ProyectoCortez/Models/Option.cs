using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoCortez.Models
{
    public class Option
    {
        [Key, Column(Order = 0)]
        public string QuestionnaireID { get; set; }

        [Key, Column(Order = 1)]
        public byte OptionID { get; set; }

        public string Text { get; set; }
        public byte Score { get; set; }

        [ForeignKey("QuestionnaireID")]
        public Questionnaire Questionnaire { get; set; }
    }
}