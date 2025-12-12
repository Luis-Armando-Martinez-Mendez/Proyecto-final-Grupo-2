using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimuladorMonteCarlo
{
    internal class ChartService
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
