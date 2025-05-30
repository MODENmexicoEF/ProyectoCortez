using System;
using System.Linq;
using MySql.Data.MySqlClient;
using ProyectoCortez.Models;

namespace ProyectoCortez.Services
{
    public class AuthService
    {
        private readonly CuestionariosContext _ctx = new CuestionariosContext();

        /// <summary>Devuelve el usuario si las credenciales son válidas; de lo contrario null.</summary>
        public User Login(string email, string password)
        {
            var user = _ctx.Database.SqlQuery<User>(
                           "CALL LoginUsuario(@mail)",
                           new MySqlParameter("mail", email))
                       .FirstOrDefault();

            return (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                   ? user
                   : null;
        }
    }
}
