# Simulador de Riesgo Financiero Monte Carlo Paralelo (Grupo #2)

[![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)](https://dotnet.microsoft.com/)
[![.NET](https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Windows Forms](https://img.shields.io/badge/Windows_Forms-0078D6?style=for-the-badge&logo=microsoft&logoColor=white)](https://docs.microsoft.com/en-us/dotnet/desktop/winforms/)
[![Programación Paralela](https://img.shields.io/badge/Paralelismo-TPL-9C27B0?style=for-the-badge&logo=cplusplus&logoColor=white)](https://learn.microsoft.com/en-us/dotnet/standard/parallel-programming/)
[![Visual Studio](https://img.shields.io/badge/Visual_Studio-5C2D91?style=for-the-badge&logo=visualstudio&logoColor=white)]()

## Descripción General

Este proyecto implementa un **Simulador de Riesgo Financiero** utilizando el método de **Monte Carlo** y el modelo de **Movimiento Browniano Geométrico (GBM)**.

El enfoque principal es la **Programación Paralela**, que nos permite distribuir miles de simulaciones de forma simultánea entre los núcleos de la CPU. Esto hace que en un sistema se pueda ejecutar análisis complejos de riesgo y retorno **casi al instante**, con alta escalabilidad y eficiencia, demostrando la potencia de la TPL de .NET

## Características Técnicas Clave

| Característica | Implementación Técnica | Beneficio |
| :--- | :--- | :--- |
| **Aceleración Paralela** | Uso de la `Task Parallel Library` (TPL) con `Parallel.For` para particionar el trabajo masivo. | Ejecución del análisis en **milisegundos**, maximizando el *throughput*. |
| **Seguridad de Hilos** | **Escritura Lock-Free** (sin bloqueos) a la matriz de resultados y **Generación de Números Aleatorios Aislada** por hilo (`Guid.NewGuid()`). | Garantiza la integridad y precisión estadística sin sacrificar rendimiento. |
| **Análisis de Riesgo Real** | Cálculo de **Valor en Riesgo (VaR 95%)** y métricas de **Ganancia Real** ajustadas por la inflación. | Proporciona métricas estándar usadas por instituciones financieras (e.g., J.P. Morgan). |
| **Escalabilidad y Benchmark** | El diseño **Embarrassingly Parallel** ofrece escalabilidad lineal. Se mide formalmente el **Speedup** y la **Eficiencia**. | El rendimiento mejora proporcionalmente a la adición de recursos de hardware. |

---

## Equipo del Proyecto

Este proyecto fue desarrollado en conjunto para la materia de Programación Paralela.

| Rol | Nombre | Matrícula |
| :--- | :--- | :--- |
| **Líder del Grupo** | Luis Armando Martínez Méndez | 2024-0202 |
| **Integrantes** | Wilker José Capellán | 2024-0217 |
| **Integrantes** | Emill Peralta Encarnación | 2023-1151 |
| **Integrantes** | Nahuel Alexander Rodríguez | 2023-1108 |

---

## Estructura y Componentes Principales

El sistema utiliza una arquitectura modular basada en la Separación de Responsabilidades:

| Archivo | Rol |
| :--- | :--- |
| `MonteCarloEngine.cs` | **Motor de Cálculo:** Contiene el algoritmo GBM, la lógica paralela (`Parallel.For`), y el método secuencial para *benchmark*. |
| `ReportService.cs` | **Análisis:** Realiza los cálculos financieros finales (VaR, Speedup, Eficiencia) y da formato al reporte de texto con colores y estilos. |
| `ChartService.cs` | **Visualización:** Se encarga de dibujar los gráficos (Histogramas, Trayectorias y Box Plots). |
| `Models.cs` | **Contrato de Datos:** Define las estructuras `SimulationParameters` (Input) y `SimulationResults` (Output). |

---

## Flujo de Trabajo y Contribución

### Ejecución

1.  Clonar el repositorio.
2.  **Instalar Librería:** Asegurarse de tener la librería System.Windows.Forms.DataVisualization instalada, ya que es necesaria para los gráficos.
3.  Abrir el archivo (`Form1.cs`) en Visual Studio.
4.  Ejecutar la aplicación (`F5`).

```text
Co-authored-by: <Emill Peralta>
