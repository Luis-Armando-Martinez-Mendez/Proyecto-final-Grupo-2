using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonteCarloSim
{
    public class SimulationParameters
    {
        public double CapitalInicial { get; set; }
        public int DiasTotales { get; set; }
        public double VolatilidadDiaria { get; set; }
        public double DerivaDiaria { get; set; }
        public int NumeroSimulaciones { get; set; }
        public double TasaLibreDiaria { get; set; }
        public double TasaInflacionDiaria { get; set; }
        public double InflacionInputTotal { get; set; } 
        public int Hilos { get; set; }
    }

    
    public class SimulationResults
    {
        public double[][] Trayectorias { get; set; } 
        public long TiempoEjecucionMs { get; set; }
        public int HilosUsados { get; set; }
        public double[] PreciosFinales { get; set; } 
    }
}
