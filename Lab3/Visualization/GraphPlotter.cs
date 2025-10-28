using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lab3.PerformanceTesting;
using ScottPlot;

namespace Lab3.Visualization
{
    public class GraphPlotter
    {
        public void CreateComplexityReport(List<PerformanceMeasurement> postfixData, List<PerformanceMeasurement> stackData)
        {
            var analyzer = new ComplexityAnalyzer();

            // Создаем папку для результатов
            string resultsDir = "PerformanceResults";
            Directory.CreateDirectory(resultsDir);

            // Анализ сложности постфиксных вычислений
            if (postfixData.Count >= 3)
            {
                Console.WriteLine("\n--- Анализ сложности постфиксных вычислений ---");
                var postfixAnalysis = analyzer.AnalyzeComplexity(postfixData);
                var postfixTheoretical = analyzer.GenerateTheoreticalData(postfixData, postfixAnalysis.ComplexityType);

                PlotPerformanceComparison(
                    postfixData,
                    postfixTheoretical,
                    $"Сложность алгоритма постфиксных вычислений\n(Аппроксимация: {postfixAnalysis.ComplexityType}, R² = {postfixAnalysis.RSquared:F4})",
                    Path.Combine(resultsDir, "PostfixComplexity.png"));

                PrintAnalysisResults(postfixAnalysis, "Постфиксные вычисления");
            }

            // Анализ сложности операций стека
            if (stackData.Count >= 3)
            {
                Console.WriteLine("\n--- Анализ сложности операций со стеком ---");
                var stackAnalysis = analyzer.AnalyzeComplexity(stackData);
                var stackTheoretical = analyzer.GenerateTheoreticalData(stackData, stackAnalysis.ComplexityType);

                PlotPerformanceComparison(
                    stackData,
                    stackTheoretical,
                    $"Сложность операций со стеком\n(Аппроксимация: {stackAnalysis.ComplexityType}, R² = {stackAnalysis.RSquared:F4})",
                    Path.Combine(resultsDir, "StackComplexity.png"));

                PrintAnalysisResults(stackAnalysis, "Операции со стеком");
            }

            // Сохраняем сырые данные
            SaveRawData(postfixData, Path.Combine(resultsDir, "postfix_raw_data.csv"));
            SaveRawData(stackData, Path.Combine(resultsDir, "stack_raw_data.csv"));

            Console.WriteLine($"\n📁 Все результаты сохранены в папке: {resultsDir}");
        }

        private void PlotPerformanceComparison(
            List<PerformanceMeasurement> experimentalData,
            List<PerformanceMeasurement> theoreticalData,
            string title,
            string outputPath)
        {
            var plt = new Plot(1000, 600);

            // Экспериментальные данные
            double[] expX = experimentalData.Select(m => (double)m.InputSize).ToArray();
            double[] expY = experimentalData.Select(m => m.ExecutionTimeMs).ToArray();

            var expScatter = plt.AddScatter(expX, expY);
            expScatter.Label = "Экспериментальные данные";
            expScatter.MarkerSize = 7;
            expScatter.MarkerShape = MarkerShape.filledCircle;
            expScatter.Color = System.Drawing.Color.Blue;

            // Теоретические данные
            double[] theoryX = theoreticalData.Select(m => (double)m.InputSize).ToArray();
            double[] theoryY = theoreticalData.Select(m => m.ExecutionTimeMs).ToArray();

            var theoryScatter = plt.AddScatter(theoryX, theoryY);
            theoryScatter.Label = "Теоретическая аппроксимация";
            theoryScatter.LineWidth = 2;
            theoryScatter.Color = System.Drawing.Color.Red;
            theoryScatter.LineStyle = LineStyle.Dash;

            // Настройка графика
            plt.Title(title, size: 14, bold: true);
            plt.XLabel("Размер входных данных (n)");
            plt.YLabel("Время выполнения (мс)");
            plt.Legend(location: Alignment.UpperLeft);

            // Сохранение
            plt.SaveFig(outputPath);
            Console.WriteLine($"✅ График сохранен: {outputPath}");
        }

        private void PrintAnalysisResults(ComplexityAnalysis analysis, string algorithmName)
        {
            Console.WriteLine($"\n=== Анализ сложности: {algorithmName} ===");
            Console.WriteLine($"Наиболее вероятная сложность: {analysis.ComplexityType}");
            Console.WriteLine($"Коэффициент детерминации (R²): {analysis.RSquared:F4}");
            Console.WriteLine($"Оценочный коэффициент: {analysis.EstimatedComplexity:E4}");

            string evaluation = analysis.RSquared switch
            {
                > 0.95 => "✓ Высокая точность аппроксимации",
                > 0.85 => "~ Удовлетворительная точность аппроксимации",
                > 0.70 => "~ Приемлемая точность аппроксимации",
                _ => "✗ Низкая точность аппроксимации"
            };
            Console.WriteLine(evaluation);
        }

        private void SaveRawData(List<PerformanceMeasurement> data, string filePath)
        {
            using var writer = new StreamWriter(filePath);
            writer.WriteLine("InputSize,ExecutionTimeMs,AlgorithmType");
            foreach (var measurement in data)
            {
                writer.WriteLine($"{measurement.InputSize},{measurement.ExecutionTimeMs:F6},{measurement.AlgorithmType}");
            }
            Console.WriteLine($"✅ Сырые данные сохранены: {filePath}");
        }
    }
}