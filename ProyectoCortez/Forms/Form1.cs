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
            Text = "Inicio de Sesión";
            WindowState = FormWindowState.Maximized;

            // ---------- layout principal ----------
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

            // ---------- controles ----------
            txtEmail = new TextBox();
            txtPassword = new TextBox { UseSystemPasswordChar = true };
            cmbRol = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            cmbRol.Items.AddRange(new object[] { "ADMIN", "STUDENT" });

            AddRow(layout, "Correo:", txtEmail);
            AddRow(layout, "Contraseña:", txtPassword);
            AddRow(layout, "Rol:", cmbRol);

            btnLogin = new Button
            {
                Text = "Iniciar sesión",
                Font = new Font("Segoe UI", 16F),
                Size = new Size(200, 50),
                Margin = new Padding(10)
            };
            btnLogin.Click += btnLogin_Click;

            btnVolver = new Button
            {
                Text = "Volver",
                Font = new Font("Segoe UI", 16F),
                Size = new Size(150, 50),
                Margin = new Padding(10)
            };
            btnVolver.Click += btnVolver_Click;

            var buttonPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                Size = new Size(500, 80),
                Padding = new Padding(10),
                Margin = new Padding(20),
                Anchor = AnchorStyles.None
            };
            buttonPanel.Controls.Add(btnLogin);
            buttonPanel.Controls.Add(btnVolver);

            layout.Controls.Add(buttonPanel, 0, layout.RowCount);
            layout.SetColumnSpan(buttonPanel, 2);

            // ---------- contenedor ----------
            var container = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 1
            };
            container.Controls.Add(layout, 0, 0);
            layout.Anchor = AnchorStyles.None;

            Controls.Add(container);
        }

        private static void AddRow(TableLayoutPanel layout, string labelText, Control input)
        {
            var lbl = new Label
            {
                Text = labelText,
                Anchor = AnchorStyles.Right,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 16F),
                Size = new Size(180, 40),
                Margin = new Padding(5)
            };
            input.Font = new Font("Segoe UI", 16F);
            input.Margin = new Padding(5);
            int row = layout.RowCount++;

            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            layout.Controls.Add(lbl, 0, row);
            layout.Controls.Add(input, 1, row);
        }

        // ---------- Login ----------
        private void btnLogin_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string pwd = txtPassword.Text;
            string rolSel = cmbRol.SelectedItem as string;

            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(pwd) ||
                rolSel == null)
            {
                MessageBox.Show("Ingresa correo, contraseña y rol.",
                                "Campos vacíos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var auth = new AuthService();
                var user = auth.Login(email, pwd);

                if (user == null)
                {
                    MessageBox.Show("Usuario o contraseña incorrectos.",
                                    "Acceso denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if ((rolSel == "ADMIN" && user.RoleID != 1) ||
                    (rolSel == "STUDENT" && user.RoleID != 2))
                {
                    MessageBox.Show("Rol incorrecto para esta cuenta.",
                                    "Error de rol", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                MessageBox.Show($"Bienvenido {rolSel.ToLower()}.",
                                "Acceso permitido", MessageBoxButtons.OK, MessageBoxIcon.Information);

                if (user.RoleID == 1)
                {
                    new AdminDashboardForm().Show();
                    Hide();
                }
                else
                {
                    new StudentQuestionnaireForm(user).Show();
                    Hide();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error de conexión:\n" + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnVolver_Click(object sender, EventArgs e)
        {
            new StartupForm().Show();
            Close();
        }
    }
}
