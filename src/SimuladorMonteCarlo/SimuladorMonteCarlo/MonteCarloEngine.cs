using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;


namespace MonteCarloSim
{
    public class MonteCarloEngine
    {
        // Método Paralelo Inicial
        public SimulationResults EjecutarSimulacion(SimulationParameters param)
        {
            if (param.DiasTotales < 1) throw new Exception("Días insuficientes.");

            double[][] resultados = new double[param.NumeroSimulaciones][];
            Stopwatch sw = Stopwatch.StartNew();

            var opcionesParalelas = new ParallelOptions { MaxDegreeOfParallelism = param.Hilos };

            Parallel.For(0, param.NumeroSimulaciones, opcionesParalelas, i =>
            {
                Random azar = new Random(Guid.NewGuid().GetHashCode());
                double[] trayectoria = new double[param.DiasTotales];
                double precioActual = param.CapitalInicial;
                trayectoria[0] = precioActual;

                for (int dia = 1; dia < param.DiasTotales; dia++)
                {
                    double u1 = 1.0 - azar.NextDouble();
                    double u2 = 1.0 - azar.NextDouble();
                    double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);

                    double cambio = param.DerivaDiaria + (param.VolatilidadDiaria * randStdNormal);
                    precioActual = precioActual * (1 + cambio);

                    //validaciones de límites 
                    double cambio = param.DerivaDiaria + (param.VolatilidadDiaria * randStdNormal);
                    precioActual = precioActual * (1 + cambio);

                    if (precioActual < 0) precioActual = 0;
                    if (precioActual > param.CapitalInicial * 1000000) precioActual = param.CapitalInicial * 1000000;

                    trayectoria[dia] = precioActual;
                }
                resultados[i] = trayectoria;
            });

            sw.Stop();

            double[] finales = resultados.Select(r => r[param.DiasTotales - 1]).ToArray();
            Array.Sort(finales);

            return new SimulationResults
            {
                Trayectorias = resultados,
                TiempoEjecucionMs = sw.ElapsedMilliseconds,
                HilosUsados = param.Hilos,
                PreciosFinales = finales
            };
        }

    }
}