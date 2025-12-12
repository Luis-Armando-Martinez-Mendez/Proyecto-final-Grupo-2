using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace MonteCarloSim
{
    public partial class Form1 : Form
    {
        private MonteCarloEngine engine = new MonteCarloEngine();
        private ChartService grafico = new ChartService();
        private ReportService reporte = new ReportService();

        // Controles UI
        private TextBox txtPrecio, txtPlazo, txtVolatilidad, txtDeriva, txtSimulaciones, txtTasaLibre, txtInflacion;
        private ComboBox cmbUnidadTiempo, cmbUnidadVol, cmbUnidadDeriva;
        private NumericUpDown numHilos;
        private RichTextBox rtbResultados;
        private Button btnSimular;

        private Panel panelControles;
        private TabControl tabGraficos;
        private Chart chartLineas, chartHistograma, chartPastel, chartCaja;

        public Form1()
        {
            InitializeComponent();
            ConstruirInterfaz();
        }

        private void BtnSimular_Click(object sender, EventArgs e)
        {
            try
            {
                var param = LeerParametrosUI();

                //Metodo secuencial
                rtbResultados.Text = "Calculando base secuencial (1 nucleo)...";
                rtbResultados.Refresh();
                long tiempoSecuencial = engine.EjecutarSimulacionSecuencial(param);

                //Paralelismo
                rtbResultados.Text = $"Simulando en paralelo ({param.Hilos} núcleos)...";
                rtbResultados.Refresh();

                var resultados = engine.EjecutarSimulacion(param);

                //Reportes
                reporte.GenerarReporte(rtbResultados, resultados, param, tiempoSecuencial);

                //Graficos
                grafico.DibujarTrayectorias(chartLineas, resultados, param);
                grafico.DibujarHistograma(chartHistograma, resultados.PreciosFinales);
                grafico.DibujarPastel(chartPastel, resultados.PreciosFinales, param.CapitalInicial);
                grafico.DibujarBoxPlot(chartCaja, resultados.PreciosFinales);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private SimulationParameters LeerParametrosUI()
        {
            if (!double.TryParse(txtPrecio.Text, out double precio)) throw new Exception("Capital inicial inválido");
            if (!int.TryParse(txtPlazo.Text, out int plazo)) throw new Exception("Plazo inválido");
            if (!double.TryParse(txtVolatilidad.Text, out double volRaw)) throw new Exception("Volatilidad inválida");
            if (!double.TryParse(txtDeriva.Text, out double derRaw)) throw new Exception("Rendimiento inválido");
            if (!double.TryParse(txtTasaLibre.Text, out double tasaLibre)) throw new Exception("Tasa segura inválida");
            if (!double.TryParse(txtInflacion.Text, out double inflacion)) throw new Exception("Inflación inválida");
            if (!int.TryParse(txtSimulaciones.Text, out int nSim)) throw new Exception("Num simulaciones inválido");

            int dias = 0;
            string tiempo = cmbUnidadTiempo.SelectedItem.ToString();
            if (tiempo == "Días") dias = plazo;
            else if (tiempo == "Meses") dias = plazo * 21;
            else dias = plazo * 252;

            double volDiaria = (cmbUnidadVol.SelectedItem.ToString() == "Anual") ? (volRaw / 100.0) / Math.Sqrt(252) : volRaw / 100.0;
            double derDiaria = (cmbUnidadDeriva.SelectedItem.ToString() == "Anual") ? (derRaw / 100.0) / 252.0 : derRaw / 100.0;
            double tasaLibreDiaria = (tasaLibre / 100.0) / 252.0;
            double tasaInflacionDiaria = Math.Pow(1 + (inflacion / 100.0), 1.0 / 252.0) - 1;

            return new SimulationParameters
            {
                CapitalInicial = precio,
                DiasTotales = dias,
                VolatilidadDiaria = volDiaria,
                DerivaDiaria = derDiaria,
                NumeroSimulaciones = nSim,
                TasaLibreDiaria = tasaLibreDiaria,
                TasaInflacionDiaria = tasaInflacionDiaria,
                InflacionInputTotal = inflacion,
                Hilos = (int)numHilos.Value
            };
        }

        private void ConstruirInterfaz()
        {
            this.Size = new Size(1350, 900);
            this.Text = "Dashboard Modular - Monte Carlo Paralelo";
            this.BackColor = Color.WhiteSmoke;

            panelControles = new Panel { Dock = DockStyle.Left, Width = 340, BackColor = Color.White, AutoScroll = true, Padding = new Padding(15) };
            tabGraficos = new TabControl { Dock = DockStyle.None, Left = 340, Top = 0, Width = this.ClientSize.Width - 340, Height = this.ClientSize.Height, Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, Font = new Font("Segoe UI", 10) };

            this.Controls.Add(tabGraficos);
            this.Controls.Add(panelControles);

            int yPos = 10;
            void AddHeader(string t, Color c) { new Label { Parent = panelControles, Text = t, Top = yPos, AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = c }; yPos += 25; }
            void AddLabel(string t) { new Label { Parent = panelControles, Text = t, Top = yPos + 3, Left = 10, AutoSize = true, Font = new Font("Segoe UI", 8) }; }

            AddHeader("1.DATOS DEL ACTIVO", Color.DarkSlateGray);
            AddLabel("Capital Inicial ($):"); txtPrecio = new TextBox { Parent = panelControles, Text = "10000", Top = yPos, Left = 120, Width = 180 }; yPos += 30;
            AddLabel("Plazo:"); txtPlazo = new TextBox { Parent = panelControles, Text = "1", Top = yPos, Left = 120, Width = 50 }; cmbUnidadTiempo = new ComboBox { Parent = panelControles, Top = yPos, Left = 180, Width = 120, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9) }; cmbUnidadTiempo.Items.AddRange(new object[] { "Días", "Meses", "Años" }); cmbUnidadTiempo.SelectedIndex = 2; yPos += 35;
            AddLabel("Volatilidad (%):"); txtVolatilidad = new TextBox { Parent = panelControles, Text = "20", Top = yPos, Left = 120, Width = 80 }; cmbUnidadVol = new ComboBox { Parent = panelControles, Top = yPos, Left = 210, Width = 90, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9) }; cmbUnidadVol.Items.AddRange(new object[] { "Diaria", "Anual" }); cmbUnidadVol.SelectedIndex = 1; yPos += 30;
            AddLabel("Rendimiento (%):"); txtDeriva = new TextBox { Parent = panelControles, Text = "12", Top = yPos, Left = 120, Width = 80 }; cmbUnidadDeriva = new ComboBox { Parent = panelControles, Top = yPos, Left = 210, Width = 90, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9) }; cmbUnidadDeriva.Items.AddRange(new object[] { "Diaria", "Anual" }); cmbUnidadDeriva.SelectedIndex = 1; yPos += 45;

            AddHeader("2.CONTEXTO ECONOMICO", Color.DarkBlue);
            AddLabel("Tasa Segura (%):"); txtTasaLibre = new TextBox { Parent = panelControles, Text = "5", Top = yPos, Left = 120, Width = 180 }; yPos += 30;
            AddLabel("Inflación Est. (%):"); txtInflacion = new TextBox { Parent = panelControles, Text = "4", Top = yPos, Left = 120, Width = 180 }; yPos += 45;

            AddHeader("3.MOTOR PARALELO", Color.DarkRed);
            AddLabel("Escenarios:"); txtSimulaciones = new TextBox { Parent = panelControles, Text = "5000", Top = yPos, Left = 120, Width = 180 }; yPos += 30;
            AddLabel("Nucleos CPU:"); numHilos = new NumericUpDown { Parent = panelControles, Top = yPos, Left = 120, Width = 180, Minimum = 1, Maximum = 64, Value = Environment.ProcessorCount }; yPos += 50;

            btnSimular = new Button { Parent = panelControles, Text = "CALCULAR RIESGO", Top = yPos, Width = 300, Height = 50, BackColor = Color.RoyalBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 11, FontStyle.Bold), Cursor = Cursors.Hand };
            btnSimular.Click += BtnSimular_Click; yPos += 60;

            AddHeader("REPORTE FINANCIERO", Color.Black);
            rtbResultados = new RichTextBox { Parent = panelControles, Top = yPos, Left = 10, Width = 300, Height = 300, BorderStyle = BorderStyle.None, BackColor = Color.WhiteSmoke, ReadOnly = true, Text = "Esperando..." };

            chartLineas = CrearChart("Trayectorias"); AddTab(chartLineas, "Trayectorias");
            chartHistograma = CrearChart("Distribución"); AddTab(chartHistograma, "Histograma");
            chartPastel = CrearChart("Probabilidad"); AddTab(chartPastel, "Pastel");
            chartCaja = CrearChart("Rango"); AddTab(chartCaja, "Box Plot");

            panelControles.BringToFront();
        }

        private void AddTab(Chart c, string title)
        {
            TabPage t = new TabPage(title);
            t.Controls.Add(c);
            tabGraficos.TabPages.Add(t);
        }

        private Chart CrearChart(string title)
        {
            Chart c = new Chart { Dock = DockStyle.Fill };
            ChartArea a = new ChartArea("Main");
            a.AxisX.MajorGrid.LineColor = Color.LightGray;
            a.AxisY.MajorGrid.LineColor = Color.LightGray;
            c.ChartAreas.Add(a);
            c.Titles.Add(new Title(title, Docking.Top, new Font("Segoe UI", 12, FontStyle.Bold), Color.Black));
            c.Legends.Add(new Legend { Docking = Docking.Bottom });
            return c;
        }

    }

}
