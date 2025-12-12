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
                // Definimos la fuente y la ponemos en negrita
                rtb.SelectionFont = new Font("Consolas", 9, bold ? FontStyle.Bold : FontStyle.Regular); 
                rtb.AppendText(text); // Agregamos el texto al control
            }
            void PrintLn(string text, Color c, bool bold = false) => Print(text + "\n", c, bold); // Agregamos un salto de línea al final para que no esté todo pegado
            
            // --- Parte del rendimiento ---
            PrintLn("=== BENCHMARK DE PARALELISMO ===", Color.Black, true); // El titulo 
            // Mostramos el tiempo sin paralelismo (usamoe un solo un CPU)
            Print("Tiempo Secuencial (1 CPU): ", Color.Black); PrintLn($"{tiempoSecuencialMs} ms", Color.DarkRed);
            // Mostramos el tiempo real que se logro usando el paralelismo
            Print("Tiempo Paralelo (" + res.HilosUsados + " CPUs): ", Color.Black); PrintLn($"{res.TiempoEjecucionMs} ms", Color.Blue, true);
            
            // Añadimos el Speedup
            // Evitamos dividir por cero si el tiempo es cero (lo forzamos a 1ms)
            double tParalelo = res.TiempoEjecucionMs > 0 ? res.TiempoEjecucionMs : 1; 
            // Calculamos la velocidad que corrio el codigo paralelo vs secuencial
            double speedup = (double)tiempoSecuencialMs / tParalelo; 
            double eficiencia = speedup / res.HilosUsados; // Calculamos el aprovechamiento de los hilos

            Print("Speedup: ", Color.Black); PrintLn($"{speedup:F2}x", Color.DarkGreen, true);
            Print("Eficiencia: ", Color.Black); PrintLn($"{eficiencia:P1}", Color.Gray);

            // Potencia de Calculo
            double segundos = tParalelo / 1000.0; // Aqui paasamos de milisegundos a segundos
            // Calculamos la capacidad de cuantas simulaciones hace poor segundo
            double simPorSegundo = param.NumeroSimulaciones / segundos; 
            Print("Potencia de Calculo: ", Color.Black); PrintLn($"{simPorSegundo:N0} sim/seg", Color.Purple, true);

            PrintLn("----------------------------", Color.Black);

            // --- Parte financiera ---
            PrintLn("RESULTADOS FINANCIEROS:", Color.Black, true); // Titulo
            Print("Capital Final (Nom): ", Color.Black); PrintLn($"${promedioFinal:N2}", Color.Blue, true);
            Print("Ganancia Nominal: ", Color.Black); 
            // Ponemos la ganancia en verde su es positiva o en rojo si es negativa
            PrintLn($"${gananciaNominal:N2}", gananciaNominal >= 0 ? Color.Green : Color.Red); 

            // Calculamos cuanto se pierde
            double perdidaPoderCompra = gananciaNominal - gananciaReal;
            Print("Efecto Inflación: ", Color.Black);
            PrintLn($"Tu dinero vale -${perdidaPoderCompra:N2} menos hoy.", Color.DarkOrange);

            Print("GANANCIA REAL: ", Color.Black, true);
            PrintLn($"${gananciaReal:N2}", gananciaReal >= 0 ? Color.Green : Color.Red, true);

            PrintLn("----------------------------", Color.Black);
        }
    }
}
