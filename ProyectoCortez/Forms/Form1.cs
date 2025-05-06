using ProyectoCortez.Services;
using ProyectoCortez.Models;
using System;
using System.Windows.Forms;

namespace ProyectoCortez
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;
            string selectedRole = cmbRol.SelectedItem.ToString();

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Por favor, ingresa tu correo y contraseña.", "Campos vacíos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var auth = new AuthService();
                var user = auth.Login(email, password);

                if (user == null)
                {
                    MessageBox.Show("Usuario o contraseña incorrectos.", "Acceso denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Verifica que el rol elegido coincida
                if ((selectedRole == "ADMIN" && user.RoleID != 1) || (selectedRole == "STUDENT" && user.RoleID != 2))
                {
                    MessageBox.Show("Rol incorrecto seleccionado para esta cuenta.", "Error de rol", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Redirigir según el rol
                if (user.RoleID == 1)
                {
                    MessageBox.Show("Bienvenido administrador.");
                    // new AdminDashboard().Show(); this.Hide();
                }
                else
                {
                    MessageBox.Show("Bienvenido estudiante.");
                    // new StudentQuestionnaireForm(user).Show(); this.Hide();
                }
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Error de conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnVolver_Click(object sender, EventArgs e)
        {
            new StartupForm().Show();
            this.Close(); // o this.Hide();
        }

    }
}
