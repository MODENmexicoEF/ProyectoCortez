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
        private Panel panelDescripcion;
        private Label lblDescripcion;
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

            var btnCerrarSesion = new Button
            {
                Text = "Cerrar sesión",
                Font = new Font("Segoe UI", 12),
                AutoSize = true,
                Dock = DockStyle.Top,
                Margin = new Padding(0, 15, 0, 0)
            };

            btnCerrarSesion.Click += (s, e) =>
            {
                this.Hide(); // Primero oculta el formulario actual
                var startupForm = new StartupForm(); // Crea una nueva instancia
                startupForm.FormClosed += (sender2, args) => this.Close(); // Cierra el actual cuando se cierre el nuevo
                startupForm.Show(); // Muestra el StartupForm
            };

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(30)
            };
            layout.RowCount = 5;
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // lblTitulo
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // cmbCuestionario
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // panelDescripcion                                                                                                                                                                                                                                           // panelDescripcion (altura fija)
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // panelPreguntas ocupa todo el restante
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // btnEnviar


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

            // Añadir espaciado inferior al ComboBox
            cmbCuestionario.Margin = new Padding(0, 0, 0, 10);

            panelDescripcion = new Panel
            {
                AutoScroll = false,
                AutoSize = true,
                Dock = DockStyle.Fill, // CORREGIDO
                Padding = new Padding(15, 10, 15, 10),
                BorderStyle = BorderStyle.FixedSingle
            };



            lblDescripcion = new Label
            {
                Margin = new Padding(10),
                // Establecer el tamaño de fuente para la descripción
                Font = new Font("Segoe UI", 11.5f), // Puedes usar 11 o 11.5f
                AutoSize = true,
                MaximumSize = new Size(700, 0),
                Text = "Selecciona una encuesta para ver su descripción."
            };

            panelDescripcion.Controls.Add(lblDescripcion); // ← ESTA LÍNEA ES LA CLAVE



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
            layout.Controls.Add(panelDescripcion, 0, 2); // aquí va el panel con scroll
            layout.Controls.Add(panelPreguntas, 0, 3);
            layout.Controls.Add(btnEnviar, 0, 4);
            layout.Controls.Add(btnCerrarSesion, 0, 5);
            layout.RowCount = 6; // Aumenta el número de filas
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Nueva fila para el botón



            layout.RowCount = 5; // << aumenta el número de filas para evitar que se sobreponga


            Controls.Add(layout);
        }

        // ----------------------------------------------------------------------
        //  Cargar preguntas
        // ----------------------------------------------------------------------
        private void cmbCuestionario_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbCuestionario.SelectedItem == null)
                return;

            cuestionarioIDActual = cmbCuestionario.SelectedItem.ToString();
            respuestasSeleccionadas.Clear();
            panelPreguntas.Controls.Clear();
            btnEnviar.Enabled = true; // Habilitar el botón por defecto al cambiar la encuesta

            // Actualiza la descripción basada en la selección
            if (cuestionarioIDActual == "PSS14")
            {
                lblDescripcion.Text = "Escala de Estrés Percibido (PSS-10) (Cohen et al., 1983). Es un instrumento que mide el nivel de " +
                                      "estrés percibido durante el último mes. Consta de 14 preguntas con un formato de respuesta tipo " +
                                      "Likert de cinco opciones. Existen evidencias de sus propiedades psicométricas en idioma español ." +
                                      "(Campos-Aria et al., 2009; Gonzáles & Landero, 2007). A partir de este instrumento, se " +
                                      "consideraron las 10 preguntas que constituyen el PSS-10, el cual ha sido aplicado en otras " +
                                      "investigaciones en Latinoamérica (Campos-Aria, et al., 2009; Domínguez-Lara, et al., 2022). Se ha " +
                                      "constatado una estructura de 2 factores en la versión de 14 ítems (PSS-14) y de 10 ítems (PSS-10) y " +
                                      "una confiabilidad adecuada, con índices ω McDonald's de .84 y .81 en el caso del PSS-14, y de .83 " +
                                      "y .70 en el PSS-10 (Domínguez-Lara, et al., 2022).";
            }
            else if (cuestionarioIDActual == "GAD7")
            {
                lblDescripcion.Text = "El GAD-7 es un instrumento auto aplicable de 7 ítems que se utiliza ampliamente para evaluar el " +
                                      "trastorno de ansiedad generalizada durante las últimas 2 semanas según el DSM-51. Cada " +
                                      "elemento se puntúa en una escala Likert de 4 puntos que indica la presencia de los síntomas, que " +
                                      "van de 0 (nada) a 3 (casi todos los días). La puntuación total de GAD-7 puede variar de 0 a 21 y una " +
                                      "puntuación > 10 indica un trastorno de ansiedad generalizada." +
                                      "\n\nUso académico: Muy útil para investigar niveles de ansiedad en grupos poblacionales o estudiantes.";
            }
            else
            {
                lblDescripcion.Text = "Selecciona una encuesta para ver su descripción.";
            }


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

                        btnEnviar.Enabled = false; // Deshabilitar el botón si la encuesta ya fue respondida recientemente
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

                // Verificamos si aún queda alguna encuesta disponible (reutilizando el mismo contexto)
                var encuestas = new[] { "GAD7", "PSS14" };
                bool algunaDisponible = false;

                foreach (var enc in encuestas)
                {
                    var ultima = context.Responses
                                         .Where(r => r.UserID == userId && r.QuestionnaireID == enc)
                                         .OrderByDescending(r => r.TakenAt)
                                         .FirstOrDefault();

                    if (ultima == null || (DateTime.Now - ultima.TakenAt).TotalDays >= 14)
                    {
                        algunaDisponible = true;
                        break;
                    }
                }

                if (algunaDisponible)
                {
                    // Reinicia el formulario
                    cmbCuestionario.SelectedIndex = -1;
                    panelPreguntas.Controls.Clear();
                    respuestasSeleccionadas.Clear();
                    preguntasActuales.Clear();
                    cuestionarioIDActual = null;
                }
                else
                {
                    MessageBox.Show("Gracias por completar todas las encuestas.\nPuedes cerrar esta ventana.",
                                    "Finalizado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // Simplemente desactiva el combo y el botón
                    cmbCuestionario.Enabled = false;
                    btnEnviar.Enabled = false;
                    panelPreguntas.Controls.Clear();

                    MessageBox.Show("Gracias por completar todas las encuestas. Puedes seguir navegando por la aplicación.",
                                    "Encuestas completadas", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
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