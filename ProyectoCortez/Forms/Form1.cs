using ProyectoCortez.Services;
using ProyectoCortez.Models;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ProyectoCortez
{
    public partial class Form1 : Form
    {
        private TextBox txtEmail;
        private TextBox txtPassword;
        private ComboBox cmbRol;
        private Button btnLogin;
        private Button btnVolver;

        public Form1()
        {
            this.Text = "Inicio de Sesión";
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

            txtEmail = new TextBox();
            txtPassword = new TextBox { UseSystemPasswordChar = true };
            cmbRol = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbRol.Items.AddRange(new object[] { "ADMIN", "STUDENT" });

            AddRow(layout, "Correo:", txtEmail);
            AddRow(layout, "Contraseña:", txtPassword);
            AddRow(layout, "Rol:", cmbRol);

            // === Botones ===
            btnLogin = new Button
            {
                Text = "Iniciar sesión",
                Font = new Font("Segoe UI", 16F),
                AutoSize = false,
                Size = new Size(200, 50),
                Margin = new Padding(10)
            };

            btnVolver = new Button
            {
                Text = "Volver",
                Font = new Font("Segoe UI", 16F),
                AutoSize = false,
                Size = new Size(150, 50),
                Margin = new Padding(10)
            };

            btnLogin.Click += btnLogin_Click;
            btnVolver.Click += btnVolver_Click;

            var buttonPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = false,
                AutoSizeMode = AutoSizeMode.GrowOnly,
                Size = new Size(500, 80),
                Padding = new Padding(10),
                Margin = new Padding(20),
                Anchor = AnchorStyles.None,
                Dock = DockStyle.Fill
            };

            buttonPanel.Controls.Add(btnLogin);
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

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;
            string selectedRole = cmbRol.SelectedItem?.ToString();

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || selectedRole == null)
            {
                MessageBox.Show("Por favor, ingresa tu correo, contraseña y rol.", "Campos vacíos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

                if ((selectedRole == "ADMIN" && user.RoleID != 1) || (selectedRole == "STUDENT" && user.RoleID != 2))
                {
                    MessageBox.Show("Rol incorrecto seleccionado para esta cuenta.", "Error de rol", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                MessageBox.Show($"Bienvenido {selectedRole.ToLower()}.", "Acceso permitido", MessageBoxButtons.OK, MessageBoxIcon.Information);

                if (user.RoleID == 1)
                {
                    new AdminDashboardForm().Show();
                    this.Hide(); // ocultar el login
                }

                else
                {
                    var cuestionarioForm = new StudentQuestionnaireForm(user);
                    cuestionarioForm.Show();
                    this.Hide();
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
            this.Close();
        }
    }
}
