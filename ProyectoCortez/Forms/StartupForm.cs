using System;
using System.Drawing;
using System.Windows.Forms;

namespace ProyectoCortez
{
    public partial class StartupForm : Form
    {
        private Button btnEstudiante;
        private Button btnAdmin;

        public StartupForm()
        {
            this.Text = "Selecciona tu rol";
            this.WindowState = FormWindowState.Maximized;

            // === Botones ===
            btnEstudiante = new Button
            {
                Text = "Registrarse",
                Font = new Font("Segoe UI", 16F),
                Size = new Size(200, 60),
                Margin = new Padding(15)
            };

            btnAdmin = new Button
            {
                Text = "Iniciar sesión",
                Font = new Font("Segoe UI", 16F),
                Size = new Size(200, 60),
                Margin = new Padding(15)
            };

            btnEstudiante.Click += btnEstudiante_Click;
            btnAdmin.Click += btnAdmin_Click;

            // === Panel para botones ===
            var buttonPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                Anchor = AnchorStyles.None,
                Padding = new Padding(20),
                Margin = new Padding(30)
            };

            buttonPanel.Controls.Add(btnEstudiante);
            buttonPanel.Controls.Add(btnAdmin);

            // === Contenedor principal centrado ===
            var container = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 1
            };
            container.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            container.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            container.Controls.Add(buttonPanel, 0, 0);

            this.Controls.Add(container);
        }

        private void btnEstudiante_Click(object sender, EventArgs e)
        {
            var form = new StudentRegisterForm();
            form.Show();
            this.Hide();
        }

        private void btnAdmin_Click(object sender, EventArgs e)
        {
            var login = new Form1();
            login.Show();
            this.Hide();
        }
    }
}
