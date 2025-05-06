using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Linq;
using ProyectoCortez.Models;
using BCrypt.Net;

namespace ProyectoCortez.Services
{
    public class AuthService
    {
        private readonly CuestionariosContext _context;

        public AuthService()
        {
            _context = new CuestionariosContext();
        }

        public User Login(string email, string password)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == email);
                if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                    return user;

                return null;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("No se pudo conectar con la base de datos. Revisa la configuración. \n\nDetalles técnicos: " + ex.Message);
            }
        }


    }
}
