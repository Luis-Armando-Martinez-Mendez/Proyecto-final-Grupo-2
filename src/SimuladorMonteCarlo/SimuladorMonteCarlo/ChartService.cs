using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms.DataVisualization.Charting;

namespace MonteCarloSim
{
    public class ChartService
    {
    }
}
public void DibujarTrayectorias(Chart chart, SimulationResults res, SimulationParameters param)
{
    chart.Series.SuspendUpdates();
    chart.Series.Clear();

    // --- 1. Muestra de rutas ---
    int rutasVisibles = Math.Min(150, param.NumeroSimulaciones);

    for (int i = 0; i < rutasVisibles; i++)
    {
        Series s = new Series
        {
            ChartType = SeriesChartType.FastLine,
            Color = Color.FromArgb(30, Color.DodgerBlue),
            IsVisibleInLegend = false
        };

        int paso = param.DiasTotales > 300 ? 5 : 1;

        for (int d = 0; d < param.DiasTotales; d += paso)
            s.Points.AddXY(d, res.Trayectorias[i][d]);

        chart.Series.Add(s);
    }

    // --- 2. Series de referencia ---
    Series sProm = new Series { Name = "Esperado", ChartType = SeriesChartType.Line, Color = Color.Red, BorderWidth = 3 };
    Series sSafe = new Series { Name = "Tasa Segura", ChartType = SeriesChartType.Line, Color = Color.Gold, BorderWidth = 3, BorderDashStyle = ChartDashStyle.Dash };
    Series sInf = new Series { Name = "Obj. Inflación", ChartType = SeriesChartType.Line, Color = Color.Purple, BorderWidth = 3 };

    int pasoProm = param.DiasTotales > 300 ? 5 : 1;

    for (int d = 0; d < param.DiasTotales; d += pasoProm)
    {
        double suma = 0;

        for (int sim = 0; sim < param.NumeroSimulaciones; sim += 10)
            suma += res.Trayectorias[sim][d];

        double promedioDia = suma / (param.NumeroSimulaciones / 10.0);

        sProm.Points.AddXY(d, promedioDia);
        sSafe.Points.AddXY(d, param.CapitalInicial * Math.Pow(1 + param.TasaLibreDiaria, d));
        sInf.Points.AddXY(d, param.CapitalInicial * Math.Pow(1 + param.TasaInflacionDiaria, d));
    }

    chart.Series.Add(sProm);
    chart.Series.Add(sSafe);
    chart.Series.Add(sInf);

    chart.Series.ResumeUpdates();
}

// --- Histograma ---
public void DibujarHistograma(Chart chart, double[] preciosFinales)
{
    chart.Series.Clear();

    Series sHist = new Series("Frecuencia")
    {
        ChartType = SeriesChartType.Column,
        Color = Color.CornflowerBlue
    };

    double min = preciosFinales.Min();
    double max = preciosFinales.Max();

    int numBins = 20;
    double anchoBin = (max - min) / numBins;

    int[] frecuencias = new int[numBins];

    foreach (double valor in preciosFinales)
    {
        int indice = (int)((valor - min) / anchoBin);
        if (indice >= numBins) indice = numBins - 1;
        frecuencias[indice]++;
    }

    for (int i = 0; i < numBins; i++)
    {
        double rangoInicio = min + (i * anchoBin);
        sHist.Points.AddXY(Math.Round(rangoInicio, 0), frecuencias[i]);
    }

    chart.Series.Add(sHist);
}

// --- Pastel ---
public void DibujarPastel(Chart chart, double[] preciosFinales, double capitalInicial)
{
    chart.Series.Clear();

    int ganaron = preciosFinales.Count(v => v > capitalInicial);
    int perdieron = preciosFinales.Length - ganaron;

    Series sPie = new Series("Probabilidad")
    {
        ChartType = SeriesChartType.Pie
    };

    sPie.Points.AddXY("Ganancia", ganaron);
    sPie.Points.AddXY("Pérdida", perdieron);

    sPie.Points[0].Color = Color.LightGreen;
    sPie.Points[0].Label = $"Ganancia\n#PERCENT";

    sPie.Points[1].Color = Color.Salmon;
    sPie.Points[1].Label = $"Pérdida\n#PERCENT";

    chart.Series.Add(sPie);
}

public void DibujarBoxPlot(Chart chart, double[] preciosFinales)
{
    chart.Series.Clear();

    double min = preciosFinales.Min();
    double max = preciosFinales.Max();
    double median = preciosFinales[preciosFinales.Length / 2];
    double q1 = preciosFinales[preciosFinales.Length / 4];
    double q3 = preciosFinales[(preciosFinales.Length * 3) / 4];

    Series sBox = new Series("Rango")
    {
        ChartType = SeriesChartType.BoxPlot
    };

    DataPoint punto = new DataPoint();
    punto.YValues = new double[] { min, max, q1, q3, 0, median };

    sBox.Points.Add(punto);
    sBox.Color = Color.Orange;

    chart.Series.Add(sBox);
}
