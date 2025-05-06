using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoCortez.Models
{
    public class ResponseDetail
    {
        [Key, Column(Order = 0)]
        public long ResponseID { get; set; }

        [Key, Column(Order = 1)]
        public int QuestionID { get; set; }

        public string QuestionnaireID { get; set; } // para poder relacionar con Option
        public byte OptionID { get; set; }

        [ForeignKey("ResponseID")]
        public Response Response { get; set; }

        [ForeignKey("QuestionID")]
        public Question Question { get; set; }

        [ForeignKey("QuestionnaireID,OptionID")]
        public Option Option { get; set; }
    }
}
