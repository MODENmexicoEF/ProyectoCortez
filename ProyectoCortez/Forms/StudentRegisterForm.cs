using ProyectoCortez.Models;
using System;
using System.Drawing;
using System.Windows.Forms;
using BCrypt.Net;

namespace ProyectoCortez
{
    public class StudentRegisterForm : Form
    {
        private TextBox txtNombre, txtEdad, txtMunicipio, txtCarrera, txtNoControl, txtSemestre;
        private TextBox txtEmail, txtPassword, txtConfirmPassword;
        private ComboBox cmbSexo;
        private Button btnContinuar, btnVolver;

        public StudentRegisterForm()
        {
            this.Text = "Registro de Estudiante";
            this.WindowState = FormWindowState.Maximized;

            var layout = new TableLayoutPanel
            {
                ColumnCount = 2,
                AutoSize = true,
                Padding = new Padding(30),
                Anchor = AnchorStyles.None,
                Margin = new Padding(30)
            };

            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));

            // Campos nuevos
            txtEmail = new TextBox();
            txtPassword = new TextBox { UseSystemPasswordChar = true };
            txtConfirmPassword = new TextBox { UseSystemPasswordChar = true };

            // Campos existentes
            txtNombre = new TextBox();
            txtEdad = new TextBox();
            cmbSexo = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            cmbSexo.Items.AddRange(new object[] { "Masculino", "Femenino", "Otro" });
            txtMunicipio = new TextBox();
            txtCarrera = new TextBox();
            txtNoControl = new TextBox();
            txtSemestre = new TextBox();

            // Añadir campos
            AddRow(layout, "Correo institucional:", txtEmail);
            AddRow(layout, "Contraseña:", txtPassword);
            AddRow(layout, "Confirmar contraseña:", txtConfirmPassword);
            AddRow(layout, "Nombre:", txtNombre);
            AddRow(layout, "Edad:", txtEdad);
            AddRow(layout, "Sexo:", cmbSexo);
            AddRow(layout, "Municipio:", txtMunicipio);
            AddRow(layout, "Carrera:", txtCarrera);
            AddRow(layout, "No de Control:", txtNoControl);
            AddRow(layout, "Semestre:", txtSemestre);

            // Botones
            btnContinuar = new Button
            {
                Text = "Continuar",
                Font = new Font("Segoe UI", 16F),
                Size = new Size(180, 50),
                Margin = new Padding(10)
            };

            btnVolver = new Button
            {
                Text = "Volver",
                Font = new Font("Segoe UI", 16F),
                Size = new Size(150, 50),
                Margin = new Padding(10)
            };

            btnContinuar.Click += btnContinuar_Click;
            btnVolver.Click += (s, e) => { new StartupForm().Show(); this.Close(); };

            var buttonPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = false,
                Size = new Size(500, 80),
                Anchor = AnchorStyles.None,
                Padding = new Padding(10),
                Margin = new Padding(20)
            };

            buttonPanel.Controls.Add(btnContinuar);
            buttonPanel.Controls.Add(btnVolver);
            layout.Controls.Add(buttonPanel, 0, layout.RowCount);
            layout.SetColumnSpan(buttonPanel, 2);

            var container = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 1
            };
            container.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            container.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            container.Controls.Add(layout, 0, 0);
            layout.Anchor = AnchorStyles.None;

            this.Controls.Add(container);
        }

        private void AddRow(TableLayoutPanel layout, string labelText, Control inputControl)
        {
            var label = new Label
            {
                Text = labelText,
                AutoSize = false,
                Anchor = AnchorStyles.Right,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 16F),
                Size = new Size(180, 40),
                Margin = new Padding(5)
            };

            inputControl.Font = new Font("Segoe UI", 16F);
            inputControl.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            inputControl.Width = 300;
            inputControl.Margin = new Padding(5);

            int rowIndex = layout.RowCount++;
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            layout.Controls.Add(label, 0, rowIndex);
            layout.Controls.Add(inputControl, 1, rowIndex);
        }

        private void btnContinuar_Click(object sender, EventArgs e)
        {
            // Validar campos
            if (string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text) ||
                string.IsNullOrWhiteSpace(txtConfirmPassword.Text) ||
                txtPassword.Text != txtConfirmPassword.Text ||
                string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtEdad.Text) ||
                string.IsNullOrWhiteSpace(cmbSexo.Text) ||
                string.IsNullOrWhiteSpace(txtMunicipio.Text) ||
                string.IsNullOrWhiteSpace(txtCarrera.Text) ||
                string.IsNullOrWhiteSpace(txtNoControl.Text) ||
                string.IsNullOrWhiteSpace(txtSemestre.Text))
            {
                MessageBox.Show("Todos los campos son obligatorios y las contraseñas deben coincidir.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var context = new CuestionariosContext();

                // Crear usuario
                var user = new User
                {
                    Email = txtEmail.Text,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(txtPassword.Text),
                    RoleID = 2,
                    CreatedAt = DateTime.Now
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

                MessageBox.Show("Registro exitoso.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // new StudentQuestionnaireForm(user).Show(); this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
