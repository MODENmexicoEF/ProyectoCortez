using ProyectoCortez.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using MySql.Data.MySqlClient;

namespace ProyectoCortez
{
    public class AdminDashboardForm : Form
    {
        // ---------------- UI -----------------
        private readonly DataGridView grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AutoGenerateColumns = true,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };

        private readonly Button btnAsc = new Button { Text = "Orden Ascendente", AutoSize = true };
        private readonly Button btnDesc = new Button { Text = "Orden Descendente", AutoSize = true };

        private readonly ComboBox cbEst = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font = new Font("Segoe UI", 12),
            Width = 260
        };

        private readonly Chart chHist = NuevoChart("Historial individual", "Fecha", "Puntaje");
        private readonly Chart chPromA = NuevoChart("Promedio Ansiedad", "Estudiante", "Puntaje");
        private readonly Chart chPromE = NuevoChart("Promedio Estrés", "Estudiante", "Puntaje");

        // ---------------- ctor ---------------
        public AdminDashboardForm()
        {
            Text = "Panel de Administrador";
            WindowState = FormWindowState.Maximized;

            var lblTop = new Label
            {
                Text = "Bienvenido al panel de administración",
                Dock = DockStyle.Top,
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                Padding = new Padding(0, 15, 0, 15),
                AutoSize = true
            };

            // botones de orden
            var pnlBtns = new FlowLayoutPanel { AutoSize = true, Dock = DockStyle.Top, Padding = new Padding(5) };
            pnlBtns.Controls.Add(btnAsc);
            pnlBtns.Controls.Add(btnDesc);
            btnAsc.Click += (s, e) => Ordenar(true);
            btnDesc.Click += (s, e) => Ordenar(false);

            // zona gráficas
            var pnlCharts = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                FlowDirection = FlowDirection.TopDown,
                AutoSize = true,
                Padding = new Padding(10)
            };
            pnlCharts.Controls.Add(cbEst);
            pnlCharts.Controls.Add(chHist);
            pnlCharts.Controls.Add(chPromA);
            pnlCharts.Controls.Add(chPromE);

            Controls.Add(grid);
            Controls.Add(pnlCharts);
            Controls.Add(pnlBtns);
            Controls.Add(lblTop);

            cbEst.SelectedIndexChanged += (s, e) => CargarHistorial();
            CargarDashboard();
        }

        // ===================================================================
        // 1.  Dashboard (grid + combos + promedios)
        // ===================================================================
        private void CargarDashboard()
        {
            using (var ctx = new CuestionariosContext())
            {
                // -------- 1.a  DataGrid --------------------------------------
                List<DTOResumen> resumen = ctx.Database
                    .SqlQuery<DTOResumen>("CALL GetResumenEstudiantes()")
                    .ToList();

                DataTable table = new DataTable();
                table.Columns.Add("NoControl", typeof(int));
                table.Columns.Add("Nombre", typeof(string));
                table.Columns.Add("Ansiedad", typeof(object));
                table.Columns.Add("Estrés", typeof(object));
                table.Columns.Add("Tiempo restante ansiedad", typeof(string));
                table.Columns.Add("Tiempo restante estrés", typeof(string));

                foreach (DTOResumen r in resumen)
                {
                    table.Rows.Add(
                        r.NoControl,
                        r.Nombre,
                        r.UltGAD_Score ?? (object)"N/A",
                        r.UltPSS_Score ?? (object)"N/A",
                        FormatearRestante(r.UltGAD_Taken),
                        FormatearRestante(r.UltPSS_Taken));
                }
                grid.DataSource = table;

                // -------- 1.b  Combo estudiantes ----------------------------
                cbEst.DataSource = resumen
                    .OrderBy(r => r.Nombre)
                    .Select(r => new KeyValuePair<int, string>(r.UserID, $"{r.NoControl} - {r.Nombre}"))
                    .ToList();
                cbEst.DisplayMember = "Value";
                cbEst.ValueMember = "Key";

                // -------- 1.c  Promedios (2 gráficas) -----------------------
                chPromA.Series.Clear();
                chPromE.Series.Clear();

                List<DTOProm> proms = ctx.Database
                    .SqlQuery<DTOProm>("CALL GetPromediosPorUsuario()")
                    .ToList();

                Dictionary<int, string> mapaNom = resumen.ToDictionary(r => r.UserID, r => r.Nombre);

                Series sA = new Series("Ansiedad") { ChartType = SeriesChartType.Column };
                Series sE = new Series("Estrés") { ChartType = SeriesChartType.Column };

                foreach (DTOProm p in proms)
                {
                    string nom;
                    if (!mapaNom.TryGetValue(p.UserID, out nom)) continue;

                    if (p.QuestionnaireID == "GAD7")
                        sA.Points.AddXY(nom, p.Prom);
                    else if (p.QuestionnaireID == "PSS14")
                        sE.Points.AddXY(nom, p.Prom);
                }
                chPromA.Series.Add(sA);
                chPromE.Series.Add(sE);
            }
        }

        // ===================================================================
        // 2.  Historial individual
        // ===================================================================
        private void CargarHistorial()
        {
            if (cbEst.SelectedItem == null) return;
            var sel = (KeyValuePair<int, string>)cbEst.SelectedItem;
            int userId = sel.Key;

            using (var ctx = new CuestionariosContext())
            {
                List<DTOHist> hist = ctx.Database
                    .SqlQuery<DTOHist>("CALL GetHistorialRespuestas(@u)",
                                       new MySqlParameter("u", userId))
                    .OrderBy(h => h.TakenAt)
                    .ToList();

                chHist.Series.Clear();
                Series sG = new Series("Ansiedad") { ChartType = SeriesChartType.Line };
                Series sP = new Series("Estrés") { ChartType = SeriesChartType.Line };

                foreach (DTOHist h in hist)
                {
                    string fecha = h.TakenAt.ToString("dd/MM/yyyy");
                    if (h.QuestionnaireID == "GAD7")
                        sG.Points.AddXY(fecha, h.TotalScore);
                    else if (h.QuestionnaireID == "PSS14")
                        sP.Points.AddXY(fecha, h.TotalScore);
                }
                chHist.Series.Add(sG);
                chHist.Series.Add(sP);
            }
        }

        // ===================================================================
        // utilidades
        // ===================================================================
        private static string FormatearRestante(DateTime? ultima)
        {
            if (ultima == null) return "Listo";
            TimeSpan restante = TimeSpan.FromDays(14) - (DateTime.Now - ultima.Value);
            return restante.TotalSeconds <= 0
                ? "Listo"
                : string.Format("{0}d {1}h {2}m", restante.Days, restante.Hours, restante.Minutes);
        }

        private static Chart NuevoChart(string titulo, string ejeX, string ejeY)
        {
            Chart c = new Chart { Width = 600, Height = 260 };
            ChartArea area = new ChartArea();
            area.AxisX.Title = ejeX;
            area.AxisY.Title = ejeY;
            c.ChartAreas.Add(area);
            c.Titles.Add(titulo);
            return c;
        }

        private void Ordenar(bool asc)
        {
            DataTable dt = grid.DataSource as DataTable;
            if (dt == null) return;

            dt.DefaultView.Sort = "Nombre " + (asc ? "ASC" : "DESC");
            grid.DataSource = dt.DefaultView.ToTable();
        }

        // ===================================================================
        // DTOs locales (proyecciones de los SP)
        // ===================================================================
        private sealed class DTOResumen
        {
            public int UserID { get; set; }
            public int NoControl { get; set; }
            public string Nombre { get; set; }
            public int? UltGAD_Score { get; set; }
            public DateTime? UltGAD_Taken { get; set; }
            public int? UltPSS_Score { get; set; }
            public DateTime? UltPSS_Taken { get; set; }
        }

        private sealed class DTOHist
        {
            public string QuestionnaireID { get; set; }
            public byte TotalScore { get; set; }
            public DateTime TakenAt { get; set; }
        }

        private sealed class DTOProm
        {
            public int UserID { get; set; }
            public string QuestionnaireID { get; set; }
            public double Prom { get; set; }
        }
    }
}
