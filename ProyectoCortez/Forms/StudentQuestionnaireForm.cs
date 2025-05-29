using ProyectoCortez.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ProyectoCortez
{
    public class StudentQuestionnaireForm : Form
    {
        private ComboBox cmbCuestionario;
        private FlowLayoutPanel panelPreguntas;
        private Button btnEnviar;
        private readonly int userId;

        private List<Question> preguntasActuales = new List<Question>();
        private Dictionary<int, byte> respuestasSeleccionadas = new Dictionary<int, byte>();
        private string cuestionarioIDActual;

        // Margen inferior variable entre preguntas
        private Padding MargenPregunta()
        {
            int px = (int)(Font.GetHeight() * 3.0f); // ajusta el factor a tu gusto
            return new Padding(0, px / 2, 0, px);
        }

        public StudentQuestionnaireForm(User user)
        {
            userId = user.UserID;
            Text = "Cuestionarios de Bienestar";
            WindowState = FormWindowState.Maximized;

            // Contenedor principal --------------------------------------------------
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(30)
            };
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var lblTitulo = new Label
            {
                Text = "Selecciona una encuesta:",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                AutoSize = true
            };

            cmbCuestionario = new ComboBox
            {
                Font = new Font("Segoe UI", 16),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Dock = DockStyle.Top
            };
            cmbCuestionario.Items.AddRange(new[] { "GAD7", "PSS14" });
            cmbCuestionario.SelectedIndexChanged += cmbCuestionario_SelectedIndexChanged;

            panelPreguntas = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                Dock = DockStyle.Fill,
                AutoScroll = true,
                WrapContents = false
            };
            panelPreguntas.Resize += (s, e) =>
            {
                foreach (Control c in panelPreguntas.Controls)
                {
                    c.Width = panelPreguntas.ClientSize.Width - 10;
                    foreach (Control sub in c.Controls)
                    {
                        if (sub is Label lbl)
                            lbl.MaximumSize = new Size(c.Width - 20, 0);
                    }
                }
            };

            btnEnviar = new Button
            {
                Text = "Enviar Respuestas",
                Font = new Font("Segoe UI", 16),
                AutoSize = true,
                Dock = DockStyle.Top
            };
            btnEnviar.Click += btnEnviar_Click;

            layout.Controls.Add(lblTitulo, 0, 0);
            layout.Controls.Add(cmbCuestionario, 0, 1);
            layout.Controls.Add(panelPreguntas, 0, 2);
            layout.Controls.Add(btnEnviar, 0, 3);

            Controls.Add(layout);
        }

        // ----------------------------------------------------------------------
        //  Cargar preguntas
        // ----------------------------------------------------------------------
        private void cmbCuestionario_SelectedIndexChanged(object sender, EventArgs e)
        {
            cuestionarioIDActual = cmbCuestionario.SelectedItem.ToString();
            panelPreguntas.Controls.Clear();
            respuestasSeleccionadas.Clear();

            using (var context = new CuestionariosContext())
            {
                var ultima = context.Responses
                                    .Where(r => r.UserID == userId && r.QuestionnaireID == cuestionarioIDActual)
                                    .OrderByDescending(r => r.TakenAt)
                                    .FirstOrDefault();

                if (ultima != null)
                {
                    double diasTranscurridos = (DateTime.Now - ultima.TakenAt).TotalDays;
                    double diasRestantes = 14 - diasTranscurridos;

                    if (diasRestantes > 0)
                    {
                        MessageBox.Show(
                            $"Ya respondiste esta encuesta el día {ultima.TakenAt:dd/MM/yyyy HH:mm}.\n\n" +
                            $"Interpretación anterior: {ultima.Interpretation}\nPuntaje: {ultima.TotalScore}\n\n" +
                            $"Podrás volver a contestarla en {Math.Ceiling(diasRestantes)} día(s).",
                            "Ya contestada",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );

                        return;
                    }
                }


                preguntasActuales = context.Questions
                                           .Where(q => q.QuestionnaireID == cuestionarioIDActual)
                                           .OrderBy(q => q.NumberInForm)
                                           .ToList();

                var opciones = context.Options
                                      .Where(o => o.QuestionnaireID == cuestionarioIDActual)
                                      .OrderBy(o => o.OptionID)
                                      .ToList();

                foreach (var pregunta in preguntasActuales)
                {
                    var groupBox = new GroupBox
                    {
                        Font = new Font("Segoe UI", 12),
                        Dock = DockStyle.Top,
                        AutoSize = true,
                        Padding = new Padding(10),
                        Margin = MargenPregunta()
                    };

                    // Etiqueta de la pregunta
                    var lblPregunta = new Label
                    {
                        Text = $"{pregunta.NumberInForm}. {pregunta.Text}",
                        AutoSize = true,
                        MaximumSize = new Size(panelPreguntas.ClientSize.Width - 40, 0),
                        Font = groupBox.Font
                    };

                    var inner = new FlowLayoutPanel
                    {
                        FlowDirection = FlowDirection.LeftToRight,
                        Dock = DockStyle.Fill,
                        AutoSize = true,
                        WrapContents = true,
                        Margin = new Padding(0, 8, 0, 0) // espacio tras la pregunta
                    };

                    foreach (var opt in opciones)
                    {
                        var radio = new RadioButton
                        {
                            Text = opt.Text,
                            Font = new Font("Segoe UI", 12),
                            Tag = Tuple.Create(pregunta.QuestionID, opt.OptionID),
                            AutoSize = true,
                            Margin = new Padding(10, 5, 10, 5)
                        };

                        radio.CheckedChanged += (s, ev) =>
                        {
                            var t = (Tuple<int, byte>)((RadioButton)s).Tag;
                            if (((RadioButton)s).Checked)
                                respuestasSeleccionadas[t.Item1] = (byte)t.Item2;
                        };

                        inner.Controls.Add(radio);
                    }

                    groupBox.Controls.Add(lblPregunta);
                    groupBox.Controls.Add(inner);
                    panelPreguntas.Controls.Add(groupBox);
                    groupBox.Width = panelPreguntas.ClientSize.Width - 10;
                }
            }
        }

        // ----------------------------------------------------------------------
        //  Enviar
        // ----------------------------------------------------------------------
        private void btnEnviar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cuestionarioIDActual) ||
                respuestasSeleccionadas.Count != preguntasActuales.Count)
            {
                MessageBox.Show("Responde todas las preguntas antes de enviar.",
                                "Incompleto",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var context = new CuestionariosContext())
            {
                var opciones = context.Options
                                      .Where(o => o.QuestionnaireID == cuestionarioIDActual)
                                      .ToDictionary(o => o.OptionID, o => o.Score);

                int total = 0;

                var response = new Response
                {
                    UserID = userId,
                    QuestionnaireID = cuestionarioIDActual,
                    TakenAt = DateTime.Now
                };

                context.Responses.Add(response);
                context.SaveChanges();

                foreach (KeyValuePair<int, byte> kvp in respuestasSeleccionadas)
                {
                    int qid = kvp.Key;
                    byte oid = kvp.Value;

                    total += opciones[oid];

                    context.ResponseDetails.Add(new ResponseDetail
                    {
                        ResponseID = response.ResponseID,
                        QuestionID = qid,
                        QuestionnaireID = cuestionarioIDActual,
                        OptionID = oid
                    });
                }

                response.TotalScore = (byte)total;
                response.Interpretation = Interpretar(cuestionarioIDActual, total);
                context.SaveChanges();

                MessageBox.Show(
                    string.Format("Encuesta completada.\nPuntaje: {0}\nInterpretación: {1}",
                                  total, response.Interpretation),
                    "Gracias por participar",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                var inicio = new StartupForm();  // <-- asegúrate que este form no requiere parámetros
                inicio.Show();

                this.Close();
            }
        }

        // ----------------------------------------------------------------------
        //  Interpretación de puntaje
        // ----------------------------------------------------------------------
        private static string Interpretar(string id, int score)
        {
            if (id == "GAD7")
            {
                if (score <= 5) return "Ansiedad mínima";
                if (score <= 10) return "Ansiedad leve";
                if (score <= 15) return "Ansiedad moderada";
                return "Ansiedad severa";
            }

            if (id == "PSS14")
            {
                if (score <= 20) return "Estrés bajo";
                if (score <= 30) return "Estrés medio";
                return "Estrés alto";
            }

            return "Desconocido";
        }
    }
}
