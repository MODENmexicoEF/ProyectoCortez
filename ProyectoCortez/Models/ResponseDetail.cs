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

        public string QuestionnaireID { get; set; }
        public byte OptionID { get; set; }

        [ForeignKey("ResponseID")]
        public virtual Response Response { get; set; }

        [ForeignKey("QuestionID")]
        public virtual Question Question { get; set; }

        public virtual Option Option { get; set; } // relación por clave compuesta se configura en Fluent API
    }
}
