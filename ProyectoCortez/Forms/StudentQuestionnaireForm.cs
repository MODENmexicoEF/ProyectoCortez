using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using ProyectoCortez.Models;

namespace ProyectoCortez
{
    public class StudentQuestionnaireForm : Form
    {
        //---------------------------------------------------------------- UI
        private ComboBox cmbCuestionario;
        private Panel panelDescripcion;
        private Label lblDescripcion;
        private FlowLayoutPanel panelPreguntas;
        private Button btnEnviar;

        //---------------------------------------------------------------- Estado
        private readonly int userId;
        private string cuestionarioIDActual;
        private List<Question> preguntasActuales = new List<Question>();
        private readonly Dictionary<int, byte> respuestasSeleccionadas = new Dictionary<int, byte>();

        //---------------------------------------------------------------- ctor
        public StudentQuestionnaireForm(User u)
        {
            userId = u.UserID;
            Text = "Cuestionarios de Bienestar";
            WindowState = FormWindowState.Maximized;
            BuildUI();
        }

        //---------------------------------------------------------------- helpers
        private static Padding MargenPregunta() => new Padding(0, 8, 0, 16);

        //---------------------------------------------------------------- Build UI
        private void BuildUI()
        {
            // ---- botón cerrar sesión
            var btnCerrar = new Button
            {
                Text = "Cerrar sesión",
                Font = new Font("Segoe UI", 12),
                AutoSize = true,
                Dock = DockStyle.Top
            };
            btnCerrar.Click += (s, e) => {
                Hide();
                var st = new StartupForm();
                st.FormClosed += (_, __) => Close();
                st.Show();
            };

            // ---- combo cuestionarios
            cmbCuestionario = new ComboBox
            {
                Font = new Font("Segoe UI", 16),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Margin = new Padding(0, 0, 0, 10)
            };
            cmbCuestionario.Items.AddRange(new[] { "GAD7", "PSS14" });
            cmbCuestionario.SelectedIndexChanged += cmbCuestionario_SelectedIndexChanged;

            // ---- descripción
            lblDescripcion = new Label
            {
                Font = new Font("Segoe UI", 11.5f),
                AutoSize = true,
                MaximumSize = new Size(700, 0),
                Text = "Selecciona una encuesta para ver su descripción.",
                Margin = new Padding(10)
            };
            panelDescripcion = new Panel
            {
                AutoSize = true,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(15, 10, 15, 10)
            };
            panelDescripcion.Controls.Add(lblDescripcion);

            // ---- panel preguntas
            panelPreguntas = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                Dock = DockStyle.Fill,
                AutoScroll = true,
                WrapContents = false
            };
            panelPreguntas.Resize += (s, e) => {
                foreach (Control g in panelPreguntas.Controls)
                {
                    g.Width = panelPreguntas.ClientSize.Width - 10;
                    foreach (Label l in g.Controls.OfType<Label>())
                        l.MaximumSize = new Size(g.Width - 20, 0);
                }
            };

            // ---- botón enviar
            btnEnviar = new Button
            {
                Text = "Enviar Respuestas",
                Font = new Font("Segoe UI", 16),
                AutoSize = true
            };
            btnEnviar.Click += btnEnviar_Click;

            // ---- layout raíz
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 6,
                Padding = new Padding(30)
            };
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            layout.Controls.Add(new Label
            {
                Text = "Selecciona una encuesta:",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                AutoSize = true
            }, 0, 0);
            layout.Controls.Add(cmbCuestionario, 0, 1);
            layout.Controls.Add(panelDescripcion, 0, 2);
            layout.Controls.Add(panelPreguntas, 0, 3);
            layout.Controls.Add(btnEnviar, 0, 4);
            layout.Controls.Add(btnCerrar, 0, 5);

            Controls.Add(layout);
        }

        //---------------------------------------------------------------- cargar preguntas
        private void cmbCuestionario_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbCuestionario.SelectedItem == null) return;

            cuestionarioIDActual = cmbCuestionario.SelectedItem.ToString();
            respuestasSeleccionadas.Clear();
            panelPreguntas.Controls.Clear();
            btnEnviar.Enabled = true;

            using (var ctx = new CuestionariosContext())
            {
                // --- descripción larga
                lblDescripcion.Text = ctx.Database.SqlQuery<string>(
                    "CALL GetDescripcionCuestionario(@q)",
                    new MySqlParameter("q", cuestionarioIDActual))
                    .FirstOrDefault() ?? "Descripción no disponible.";

                // --- última respuesta (bloquea 14 días)
                var ult = ctx.Database.SqlQuery<Response>(
                    "CALL GetUltimaRespuesta(@u,@q)",
                    new MySqlParameter("u", userId),
                    new MySqlParameter("q", cuestionarioIDActual)).FirstOrDefault();

                if (ult != null && (DateTime.Now - ult.TakenAt).TotalDays < 14)
                {
                    double faltan = 14 - (DateTime.Now - ult.TakenAt).TotalDays;
                    MessageBox.Show($"Ya contestaste el {ult.TakenAt:dd/MM/yyyy}. " +
                                    $"Vuelve en {Math.Ceiling(faltan)} día(s).\n\n" +
                                    $"Puntaje previo: {ult.TotalScore} – {ult.Interpretation}",
                                    "Ya contestada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnEnviar.Enabled = false;
                    return;
                }

                // --- preguntas y opciones
                preguntasActuales = ctx.Database.SqlQuery<Question>(
                    "CALL GetPreguntasPorCuestionario(@q)",
                    new MySqlParameter("q", cuestionarioIDActual)).ToList();

                var opciones = ctx.Database.SqlQuery<Option>(
                    "CALL GetOpcionesPorCuestionario(@q)",
                    new MySqlParameter("q", cuestionarioIDActual)).ToList();

                foreach (var p in preguntasActuales)
                {
                    var gb = new GroupBox
                    {
                        Font = new Font("Segoe UI", 12),
                        Dock = DockStyle.Top,
                        AutoSize = true,
                        Padding = new Padding(10),
                        Margin = MargenPregunta()
                    };
                    var lbl = new Label
                    {
                        Text = $"{p.NumberInForm}. {p.Text}",
                        AutoSize = true,
                        MaximumSize = new Size(panelPreguntas.ClientSize.Width - 40, 0),
                        Font = gb.Font
                    };
                    var inner = new FlowLayoutPanel
                    {
                        FlowDirection = FlowDirection.LeftToRight,
                        AutoSize = true,
                        Dock = DockStyle.Fill,
                        Margin = new Padding(0, 8, 0, 0)
                    };

                    foreach (var op in opciones)
                    {
                        var rb = new RadioButton
                        {
                            Text = op.Text,
                            Font = new Font("Segoe UI", 12),
                            Tag = Tuple.Create(p.QuestionID, op.OptionID),
                            AutoSize = true,
                            Margin = new Padding(10, 5, 10, 5)
                        };
                        rb.CheckedChanged += (s2, e2) => {
                            var t = (Tuple<int, byte>)((RadioButton)s2).Tag;
                            if (((RadioButton)s2).Checked)
                                respuestasSeleccionadas[t.Item1] = t.Item2;
                        };
                        inner.Controls.Add(rb);
                    }

                    gb.Controls.Add(lbl);
                    gb.Controls.Add(inner);
                    panelPreguntas.Controls.Add(gb);
                    gb.Width = panelPreguntas.ClientSize.Width - 10;
                }
            }
        }

        //---------------------------------------------------------------- enviar respuestas  🔹🔹🔹
        private void btnEnviar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cuestionarioIDActual) ||
               respuestasSeleccionadas.Count != preguntasActuales.Count)
            {
                MessageBox.Show("Responde todas las preguntas antes de enviar.",
                                "Incompleto", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var ctx = new CuestionariosContext())
            {
                var optScores = ctx.Database.SqlQuery<Option>(
                    "CALL GetOpcionesPorCuestionario(@q)",
                    new MySqlParameter("q", cuestionarioIDActual))
                    .ToDictionary(o => o.OptionID, o => o.Score);

                int total = 0;
                long responseId;

                using (var tx = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        // 1 ▪ cabecera
                        responseId = ctx.Database.SqlQuery<long>(
                            "CALL InsertarRespuestaGeneral(@u,@q,@t)",
                            new MySqlParameter("u", userId),
                            new MySqlParameter("q", cuestionarioIDActual),
                            new MySqlParameter("t", DateTime.Now)).First();

                        // 2 ▪ detalle
                        foreach (var kvp in respuestasSeleccionadas)
                        {
                            int qid = kvp.Key;
                            byte oid = kvp.Value;
                            total += optScores[oid];

                            ctx.Database.ExecuteSqlCommand(
                                "CALL InsertarDetalleRespuesta(@rid,@qid,@qnid,@oid)",
                                new MySqlParameter("rid", responseId),
                                new MySqlParameter("qid", qid),
                                new MySqlParameter("qnid", cuestionarioIDActual),
                                new MySqlParameter("oid", oid));
                        }

                        // 3 ▪ totales
                        string interp = Interpretar(cuestionarioIDActual, total);
                        ctx.Database.ExecuteSqlCommand(
                            "CALL ActualizarInterpretacionTotal(@rid,@tot,@int)",
                            new MySqlParameter("rid", responseId),
                            new MySqlParameter("tot", total),
                            new MySqlParameter("int", interp));

                        tx.Commit();

                        MessageBox.Show($"Encuesta completada.\n" +
                                        $"Puntaje: {total}\nInterpretación: {interp}",
                                        "Gracias", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        MessageBox.Show("Error al guardar: " + ex.Message,
                                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                // 4 ▪ ¿queda algo por contestar?
                bool disponible = new[] { "GAD7", "PSS14" }.Any(enc => {
                    var u = ctx.Database.SqlQuery<Response>(
                        "CALL GetUltimaRespuesta(@u,@q)",
                        new MySqlParameter("u", userId),
                        new MySqlParameter("q", enc)).FirstOrDefault();
                    return u == null || (DateTime.Now - u.TakenAt).TotalDays >= 14;
                });

                if (disponible)
                {
                    cmbCuestionario.SelectedIndex = -1;
                    panelPreguntas.Controls.Clear();
                    respuestasSeleccionadas.Clear();
                    preguntasActuales.Clear();
                    cuestionarioIDActual = null;
                }
                else
                {
                    MessageBox.Show("Has completado todas las encuestas disponibles.",
                                    "Finalizado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    cmbCuestionario.Enabled = false;
                    btnEnviar.Enabled = false;
                    panelPreguntas.Controls.Clear();
                }
            }
        }

        //---------------------------------------------------------------- interpretación
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
