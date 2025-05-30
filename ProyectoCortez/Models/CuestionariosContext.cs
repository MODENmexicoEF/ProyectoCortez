using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MySql.Data.MySqlClient;// Necesario para SqlParameter
using ProyectoCortez.SQL;

namespace ProyectoCortez.Models
{
    public class CuestionariosContext : DbContext
    {
        public CuestionariosContext() : base("name=CuestionariosContext") { }

        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Questionnaire> Questionnaires { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Option> Options { get; set; }
        public DbSet<Response> Responses { get; set; }
        public DbSet<ResponseDetail> ResponseDetails { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ResponseDetail>()
                .HasRequired(rd => rd.Option)
                .WithMany()
                .HasForeignKey(rd => new { rd.QuestionnaireID, rd.OptionID });
        }

        // Llamadas a Procedimientos Almacenados
        public List<EstudianteDTO> GetEstudiantesSP()
        {
            return Database.SqlQuery<EstudianteDTO>("CALL GetEstudiantes()").ToList();
        }

        public Response GetUltimaRespuestaAnsiedadSP(int userId)
        {
            return Database.SqlQuery<Response>("CALL GetUltimaRespuestaAnsiedad(@userId)",
                new MySqlParameter("userId", userId)).FirstOrDefault();
        }

        public Response GetUltimaRespuestaEstresSP(int userId)
        {
            // Asumiendo que existe un procedimiento similar para PSS14 (Estrés) según la imagen
            // Si no, necesitarás crear el procedimiento GetUltimaRespuestaEstres en tu DB
            return Database.SqlQuery<Response>("CALL GetUltimaRespuestaEstres(@userId)",
                new MySqlParameter("userId", userId)).FirstOrDefault();
        }


        public List<Response> GetHistorialRespuestasSP(int userId)
        {
            return Database.SqlQuery<Response>("CALL GetHistorialRespuestas(@userId)",
                new MySqlParameter("userId", userId)).ToList();
        }

        public List<PromedioPorUsuarioDTO> GetPromediosPorUsuarioSP()
        {
            return Database.SqlQuery<PromedioPorUsuarioDTO>("CALL GetPromediosPorUsuario()").ToList();
        }
    }

    // DTO para el procedimiento almacenado GetPromediosPorUsuario
    public class PromedioPorUsuarioDTO
    {
        public int UserID { get; set; }
        public string QuestionnaireID { get; set; }
        public double Prom { get; set; }
    }
}