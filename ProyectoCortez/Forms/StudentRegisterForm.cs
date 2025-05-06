using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ProyectoCortez.Models;

namespace ProyectoCortez
{
    public partial class StudentRegisterForm : Form
    {
        public StudentRegisterForm()
        {
            InitializeComponent();
        }

        private void btnContinuar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtEdad.Text) ||
                string.IsNullOrWhiteSpace(cmbSexo.Text) ||
                string.IsNullOrWhiteSpace(txtMunicipio.Text) ||
                string.IsNullOrWhiteSpace(txtCarrera.Text) ||
                string.IsNullOrWhiteSpace(txtNoControl.Text) ||
                string.IsNullOrWhiteSpace(txtSemestre.Text))
            {
                MessageBox.Show("Todos los campos son obligatorios.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var context = new CuestionariosContext();
                var user = new User
                {
                    Email = $"student_{Guid.NewGuid()}@auto.com",
                    PasswordHash = "[AUTO]",
                    RoleID = 2
                };
                context.Users.Add(user);
                context.SaveChanges();

                var student = new Student
                {
                    UserID = user.UserID,
                    Nombre = txtNombre.Text,
                    Edad = byte.Parse(txtEdad.Text),
                    Sexo = cmbSexo.Text,
                    Municipio = txtMunicipio.Text,
                    Carrera = txtCarrera.Text,
                    NoControl = int.Parse(txtNoControl.Text),
                    semestre = int.Parse(txtSemestre.Text)
                };
                context.Students.Add(student);
                context.SaveChanges();

                MessageBox.Show("Datos guardados. Bienvenido/a.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // Aquí puedes abrir el formulario del cuestionario
                // new StudentQuestionnaireForm(user).Show(); this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnVolver_Click(object sender, EventArgs e)
        {
            new StartupForm().Show();
            this.Close(); // o this.Hide();
        }

    }
}
