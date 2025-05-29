using ProyectoCortez.Models;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Collections.Generic;

namespace ProyectoCortez
{
    public class AdminDashboardForm : Form
    {
        private DataGridView dataGridView;
        private BindingSource bindingSource = new BindingSource();
        private Button btnOrdenAsc, btnOrdenDesc;
        private ComboBox cbEstudiantes;
        private Chart chartHistorial, chartPromAnsiedad, chartPromEstres;

        public AdminDashboardForm()
        {
            this.Text = "Panel de Administrador";
            this.WindowState = FormWindowState.Maximized;

            var label = new Label
            {
                Text = "Bienvenido al panel de administración",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                AutoSize = true,
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleCenter,
                Padding = new Padding(0, 20, 0, 20)
            };

            dataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = true,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            btnOrdenAsc = new Button
            {
                Text = "Orden Ascendente",
                Font = new Font("Segoe UI", 12),
                AutoSize = true,
                Margin = new Padding(10)
            };
            btnOrdenAsc.Click += (s, e) => Ordenar(true);

            btnOrdenDesc = new Button
            {
                Text = "Orden Descendente",
                Font = new Font("Segoe UI", 12),
                AutoSize = true,
                Margin = new Padding(10)
            };
            btnOrdenDesc.Click += (s, e) => Ordenar(false);

            var topPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true
            };
            topPanel.Controls.Add(btnOrdenAsc);
            topPanel.Controls.Add(btnOrdenDesc);

            cbEstudiantes = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 300,
                Font = new Font("Segoe UI", 12),
                DisplayMember = "Value",
                ValueMember = "Key"
            };
            cbEstudiantes.SelectedIndexChanged += (s, e) => MostrarHistorial();

            chartHistorial = CrearChart("Historial individual", "Fecha", "Puntaje");
            chartPromAnsiedad = CrearChart("Promedio Ansiedad", "Estudiante", "Puntaje");
            chartPromEstres = CrearChart("Promedio Estrés", "Estudiante", "Puntaje");

            var panelGraficas = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                AutoSize = true,
                FlowDirection = FlowDirection.TopDown,
                Padding = new Padding(10)
            };
            panelGraficas.Controls.Add(cbEstudiantes);
            panelGraficas.Controls.Add(chartHistorial);
            panelGraficas.Controls.Add(chartPromAnsiedad);
            panelGraficas.Controls.Add(chartPromEstres);

            this.Controls.Add(panelGraficas);
            this.Controls.Add(dataGridView);
            this.Controls.Add(topPanel);
            this.Controls.Add(label);

            CargarDatos();
        }

        private void Ordenar(bool asc)
        {
            if (bindingSource.DataSource is DataTable dt)
            {
                DataView dv = dt.DefaultView;
                dv.Sort = "Nombre " + (asc ? "ASC" : "DESC");
                bindingSource.DataSource = dv.ToTable();
            }
        }

        private void CargarDatos()
        {
            using (var context = new CuestionariosContext())
            {
                var usuarios = (from u in context.Users
                                join s in context.Students on u.UserID equals s.UserID
                                select new
                                {
                                    s.NoControl,
                                    s.Nombre,
                                    u.UserID
                                }).ToList();

                cbEstudiantes.DataSource = new BindingSource(
                    usuarios.ToDictionary(u => u.UserID, u => $"{u.NoControl} - {u.Nombre}"), null);

                var respuestas = context.Responses.ToList();

                var table = new DataTable();
                table.Columns.Add("NoControl", typeof(int));
                table.Columns.Add("Nombre", typeof(string));
                table.Columns.Add("Ansiedad", typeof(string));
                table.Columns.Add("Estres", typeof(string));
                table.Columns.Add("Tiempo restante ansiedad", typeof(string));
                table.Columns.Add("Tiempo restante estrés", typeof(string));

                foreach (var est in usuarios)
                {
                    var rAns = respuestas.Where(r => r.UserID == est.UserID && r.QuestionnaireID == "GAD7")
                                         .OrderByDescending(r => r.TakenAt).FirstOrDefault();

                    var rEst = respuestas.Where(r => r.UserID == est.UserID && r.QuestionnaireID == "PSS14")
                                         .OrderByDescending(r => r.TakenAt).FirstOrDefault();

                    string tiempoAns = CalcularTiempo(rAns?.TakenAt);
                    string tiempoEst = CalcularTiempo(rEst?.TakenAt);

                    table.Rows.Add(
                        est.NoControl,
                        est.Nombre,
                        rAns?.TotalScore.ToString() ?? "N/A",
                        rEst?.TotalScore.ToString() ?? "N/A",
                        tiempoAns,
                        tiempoEst
                    );
                }

                bindingSource.DataSource = table;
                dataGridView.DataSource = bindingSource;

                MostrarPromedios(context);
            }
        }

        private string CalcularTiempo(DateTime? ultima)
        {
            if (!ultima.HasValue) return "Listo";

            var transcurrido = DateTime.Now - ultima.Value;
            var restante = TimeSpan.FromDays(14) - transcurrido;

            if (restante.TotalSeconds <= 0)
                return "Listo";

            return string.Format("{0}d {1}h {2}m {3}s",
                restante.Days, restante.Hours, restante.Minutes, restante.Seconds);
        }

        private Chart CrearChart(string titulo, string ejeX, string ejeY)
        {
            var chart = new Chart { Height = 300, Width = 800 };
            var area = new ChartArea();
            area.AxisX.Title = ejeX;
            area.AxisY.Title = ejeY;
            chart.ChartAreas.Add(area);
            chart.Titles.Add(titulo);
            return chart;
        }

        private void MostrarHistorial()
        {
            if (cbEstudiantes.SelectedItem is null) return;

            var userID = (int)((KeyValuePair<int, string>)cbEstudiantes.SelectedItem).Key;

            using (var context = new CuestionariosContext())
            {
                var historial = context.Responses
                    .Where(r => r.UserID == userID)
                    .OrderBy(r => r.TakenAt)
                    .ToList();

                chartHistorial.Series.Clear();
                var serieGAD = new Series("Ansiedad") { ChartType = SeriesChartType.Line };
                var seriePSS = new Series("Estrés") { ChartType = SeriesChartType.Line };

                foreach (var r in historial)
                {
                    if (r.QuestionnaireID == "GAD7")
                        serieGAD.Points.AddXY(r.TakenAt.ToShortDateString(), r.TotalScore);
                    else if (r.QuestionnaireID == "PSS14")
                        seriePSS.Points.AddXY(r.TakenAt.ToShortDateString(), r.TotalScore);
                }

                chartHistorial.Series.Add(serieGAD);
                chartHistorial.Series.Add(seriePSS);
            }
        }

        private void MostrarPromedios(CuestionariosContext context)
        {
            chartPromAnsiedad.Series.Clear();
            chartPromEstres.Series.Clear();

            var promedios = context.Responses
                .GroupBy(r => new { r.UserID, r.QuestionnaireID })
                .Select(g => new
                {
                    g.Key.UserID,
                    g.Key.QuestionnaireID,
                    Prom = g.Average(r => r.TotalScore)
                }).ToList();

            var estudiantes = context.Students.ToDictionary(s => s.UserID, s => s.Nombre);

            var serieAns = new Series("Ansiedad") { ChartType = SeriesChartType.Column };
            var serieEst = new Series("Estrés") { ChartType = SeriesChartType.Column };

            foreach (var p in promedios)
            {
                if (!estudiantes.ContainsKey(p.UserID)) continue;
                var nombre = estudiantes[p.UserID];

                if (p.QuestionnaireID == "GAD7")
                    serieAns.Points.AddXY(nombre, p.Prom);
                else if (p.QuestionnaireID == "PSS14")
                    serieEst.Points.AddXY(nombre, p.Prom);
            }

            chartPromAnsiedad.Series.Add(serieAns);
            chartPromEstres.Series.Add(serieEst);
        }
    }
}
