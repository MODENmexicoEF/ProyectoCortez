namespace ProyectoCortez
{
    partial class StudentRegisterForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TextBox txtNombre;
        private System.Windows.Forms.TextBox txtEdad;
        private System.Windows.Forms.ComboBox cmbSexo;
        private System.Windows.Forms.TextBox txtMunicipio;
        private System.Windows.Forms.TextBox txtCarrera;
        private System.Windows.Forms.TextBox txtNoControl;
        private System.Windows.Forms.TextBox txtSemestre;
        private System.Windows.Forms.Button btnContinuar;
        private System.Windows.Forms.Label lblNombre;
        private System.Windows.Forms.Label lblEdad;
        private System.Windows.Forms.Label lblSexo;
        private System.Windows.Forms.Label lblMunicipio;
        private System.Windows.Forms.Label lblCarrera;
        private System.Windows.Forms.Label lblNoControl;
        private System.Windows.Forms.Label lblSemestre;
        private System.Windows.Forms.Button btnVolver;


        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.txtNombre = new System.Windows.Forms.TextBox();
            this.txtEdad = new System.Windows.Forms.TextBox();
            this.cmbSexo = new System.Windows.Forms.ComboBox();
            this.txtMunicipio = new System.Windows.Forms.TextBox();
            this.txtCarrera = new System.Windows.Forms.TextBox();
            this.txtNoControl = new System.Windows.Forms.TextBox();
            this.txtSemestre = new System.Windows.Forms.TextBox();
            this.btnContinuar = new System.Windows.Forms.Button();
            this.lblNombre = new System.Windows.Forms.Label();
            this.lblEdad = new System.Windows.Forms.Label();
            this.lblSexo = new System.Windows.Forms.Label();
            this.lblMunicipio = new System.Windows.Forms.Label();
            this.lblCarrera = new System.Windows.Forms.Label();
            this.lblNoControl = new System.Windows.Forms.Label();
            this.lblSemestre = new System.Windows.Forms.Label();

            this.SuspendLayout();

            // Labels
            this.lblNombre.Location = new System.Drawing.Point(30, 20);
            this.lblNombre.Size = new System.Drawing.Size(100, 20);
            this.lblNombre.Text = "Nombre:";

            this.lblEdad.Location = new System.Drawing.Point(30, 50);
            this.lblEdad.Size = new System.Drawing.Size(100, 20);
            this.lblEdad.Text = "Edad:";

            this.lblSexo.Location = new System.Drawing.Point(30, 80);
            this.lblSexo.Size = new System.Drawing.Size(100, 20);
            this.lblSexo.Text = "Sexo:";

            this.lblMunicipio.Location = new System.Drawing.Point(30, 110);
            this.lblMunicipio.Size = new System.Drawing.Size(100, 20);
            this.lblMunicipio.Text = "Municipio:";

            this.lblCarrera.Location = new System.Drawing.Point(30, 140);
            this.lblCarrera.Size = new System.Drawing.Size(100, 20);
            this.lblCarrera.Text = "Carrera:";

            this.lblNoControl.Location = new System.Drawing.Point(30, 170);
            this.lblNoControl.Size = new System.Drawing.Size(120, 20);
            this.lblNoControl.Text = "Número de Control:";

            this.lblSemestre.Location = new System.Drawing.Point(30, 200);
            this.lblSemestre.Size = new System.Drawing.Size(100, 20);
            this.lblSemestre.Text = "Semestre:";

            // TextBoxes and ComboBox
            this.txtNombre.Location = new System.Drawing.Point(150, 20);
            this.txtNombre.Size = new System.Drawing.Size(250, 20);

            this.txtEdad.Location = new System.Drawing.Point(150, 50);
            this.txtEdad.Size = new System.Drawing.Size(250, 20);

            this.cmbSexo.Location = new System.Drawing.Point(150, 80);
            this.cmbSexo.Size = new System.Drawing.Size(250, 21);
            this.cmbSexo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSexo.Items.AddRange(new object[] { "Masculino", "Femenino", "Otro" });

            this.txtMunicipio.Location = new System.Drawing.Point(150, 110);
            this.txtMunicipio.Size = new System.Drawing.Size(250, 20);

            this.txtCarrera.Location = new System.Drawing.Point(150, 140);
            this.txtCarrera.Size = new System.Drawing.Size(250, 20);

            this.txtNoControl.Location = new System.Drawing.Point(150, 170);
            this.txtNoControl.Size = new System.Drawing.Size(250, 20);

            this.txtSemestre.Location = new System.Drawing.Point(150, 200);
            this.txtSemestre.Size = new System.Drawing.Size(250, 20);

            // btnContinuar
            this.btnContinuar.Location = new System.Drawing.Point(150, 240);
            this.btnContinuar.Size = new System.Drawing.Size(100, 30);
            this.btnContinuar.Text = "Continuar";
            this.btnContinuar.UseVisualStyleBackColor = true;
            this.btnContinuar.Click += new System.EventHandler(this.btnContinuar_Click);

            this.btnVolver = new System.Windows.Forms.Button();
            // 
            // btnVolver
            // 
            this.btnVolver.Location = new System.Drawing.Point(260, 240);
            this.btnVolver.Name = "btnVolver";
            this.btnVolver.Size = new System.Drawing.Size(100, 30);
            this.btnVolver.Text = "Volver";
            this.btnVolver.UseVisualStyleBackColor = true;
            this.btnVolver.Click += new System.EventHandler(this.btnVolver_Click);
            this.Controls.Add(this.btnVolver);

            // Form
            this.ClientSize = new System.Drawing.Size(450, 300);
            this.Controls.Add(this.lblNombre);
            this.Controls.Add(this.lblEdad);
            this.Controls.Add(this.lblSexo);
            this.Controls.Add(this.lblMunicipio);
            this.Controls.Add(this.lblCarrera);
            this.Controls.Add(this.lblNoControl);
            this.Controls.Add(this.lblSemestre);
            this.Controls.Add(this.txtNombre);
            this.Controls.Add(this.txtEdad);
            this.Controls.Add(this.cmbSexo);
            this.Controls.Add(this.txtMunicipio);
            this.Controls.Add(this.txtCarrera);
            this.Controls.Add(this.txtNoControl);
            this.Controls.Add(this.txtSemestre);
            this.Controls.Add(this.btnContinuar);
            this.Name = "StudentRegisterForm";
            this.Text = "Registro de Estudiante";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
