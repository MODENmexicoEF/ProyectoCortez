namespace ProyectoCortez
{
    partial class StartupForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button btnEstudiante;
        private System.Windows.Forms.Button btnAdmin;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.btnEstudiante = new System.Windows.Forms.Button();
            this.btnAdmin = new System.Windows.Forms.Button();
            this.SuspendLayout();

            // btnEstudiante
            this.btnEstudiante.Location = new System.Drawing.Point(40, 30);
            this.btnEstudiante.Name = "btnEstudiante";
            this.btnEstudiante.Size = new System.Drawing.Size(120, 40);
            this.btnEstudiante.Text = "Soy Estudiante";
            this.btnEstudiante.UseVisualStyleBackColor = true;
            this.btnEstudiante.Click += new System.EventHandler(this.btnEstudiante_Click);

            // btnAdmin
            this.btnAdmin.Location = new System.Drawing.Point(180, 30);
            this.btnAdmin.Name = "btnAdmin";
            this.btnAdmin.Size = new System.Drawing.Size(120, 40);
            this.btnAdmin.Text = "Soy Administrador";
            this.btnAdmin.UseVisualStyleBackColor = true;
            this.btnAdmin.Click += new System.EventHandler(this.btnAdmin_Click);

            // StartupForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(340, 100);
            this.Controls.Add(this.btnEstudiante);
            this.Controls.Add(this.btnAdmin);
            this.Name = "StartupForm";
            this.Text = "Selecciona tu rol";
            this.ResumeLayout(false);
        }
    }
}
