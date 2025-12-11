using SimuladorMonteCarlo;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace MonteCarloSim
{
    public partial class Form1 : Form
    {
        //private MonteCarloEngine engine = new MonteCarloEngine();
        //private ChartService grafico = new ChartService();
        //private ReportService reporte = new ReportService();

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
    }
}
