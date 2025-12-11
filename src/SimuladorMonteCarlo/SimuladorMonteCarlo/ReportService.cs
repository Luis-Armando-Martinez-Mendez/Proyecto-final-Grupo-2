using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MonteCarloSim
{
    public class ReportService
    {
        public void GenerarReporte(RichTextBox rtb, SimulationResults res, SimulationParameters param, long tiempoSecuencialMs)
        {
            rtb.Clear();
            // --- Calculo de la ganancia, presion y la inflacion para el reporte ---
            double promedioFinal = res.PreciosFinales.Average(); // Se saca el promedio de todos los precios
            double gananciaNominal = promedioFinal - param.CapitalInicial; // Se saca la ganancia 
            
            // Aqui hacemos el ajuste de la inflación
            // Calculamos cuanto se comio la inflación en el periodo
            double inflacionAcumulada = Math.Pow(1 + param.TasaInflacionDiaria, param.DiasTotales) - 1;
            // Descontamos la inflación para tener el valor real
            double valorRealFinal = promedioFinal / (1 + inflacionAcumulada); 
            // Esta es la ganancia real, la que vale
            double gananciaReal = valorRealFinal - param.CapitalInicial; 
        }
    }
}
