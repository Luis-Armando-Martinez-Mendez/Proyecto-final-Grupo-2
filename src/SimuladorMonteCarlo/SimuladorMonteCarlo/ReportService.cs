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

            // Codgio para sacar el VaR de un 95%
            // Buscamos el indice del peor 5% de los casos
            int indice5Percentil = (int)(param.NumeroSimulaciones * 0.05); 
            double valorEnRiesgo95 = res.PreciosFinales[indice5Percentil];
            // El VaR es lo maximo que podemos perder con el 95%
            double VaR = param.CapitalInicial - valorEnRiesgo95; 
            if (VaR < 0) VaR = 0; // El VaR no debe ser negativo

            // Tasa Segura
            // Calculamos cuanto crece el capital en una inversión sin riesgo
            double valorSeguroFinal = param.CapitalInicial * Math.Pow(1 + param.TasaLibreDiaria, param.DiasTotales);
            
            // --- Helpers para imprimir con colores ---
            // Aqui lo que lo ponemos es mas bonito
            void Print(string text, Color c, bool bold = false)
            {
                rtb.SelectionColor = c;
                // Definimos la fuente y si va en negrita
                rtb.SelectionFont = new Font("Consolas", 9, bold ? FontStyle.Bold : FontStyle.Regular); 
                rtb.AppendText(text); // Agregamos el texto al control
            }
            void PrintLn(string text, Color c, bool bold = false) => Print(text + "\n", c, bold); // Agregamos un salto de línea al final para que no esté todo pegado
        }
    }
}
