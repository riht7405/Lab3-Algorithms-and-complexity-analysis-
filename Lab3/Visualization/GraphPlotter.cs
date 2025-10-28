using Lab3.PerformanceTesting;
using ScottPlot;

namespace Lab3.Visualization
{
    public class GraphPlotter
    {
        public void PlotPerformanceComparison(
            List<PerformanceMeasurement> experimentalData,
            List<PerformanceMeasurement> theoreticalData,
            string title,
            string outputPath)
        {
            // Фильтруем нулевые и отрицательные значения
            var validExperimentalData = experimentalData
                .Where(m => m.ExecutionTimeMs > 0.001)
                .ToList();

            var validTheoreticalData = theoreticalData
                .Where(m => m.ExecutionTimeMs > 0.001)
                .ToList();

            if (validExperimentalData.Count == 0 || validTheoreticalData.Count == 0)
            {
                Console.WriteLine($"Недостаточно данных для построения графика: {title}");
                return;
            }

            var plt = new Plot(1200, 800);

            // Подготовка данных
            double[] experimentalX = experimentalData.Select(m => (double)m.InputSize).ToArray();
            double[] experimentalY = experimentalData.Select(m => m.ExecutionTimeMs).ToArray();

            double[] theoreticalX = theoreticalData.Select(m => (double)m.InputSize).ToArray();
            double[] theoreticalY = theoreticalData.Select(m => m.ExecutionTimeMs).ToArray();

            // Экспериментальные данные
            var expScatter = plt.AddScatter(experimentalX, experimentalY);
            expScatter.Label = "Экспериментальные данные";
            expScatter.MarkerSize = 7;
            expScatter.MarkerShape = MarkerShape.filledCircle;
            expScatter.Color = System.Drawing.Color.Blue;

            // Теоретические данные
            var theoryScatter = plt.AddScatter(theoreticalX, theoreticalY);
            theoryScatter.Label = "Теоретическая аппроксимация";
            theoryScatter.LineWidth = 2;
            theoryScatter.Color = System.Drawing.Color.Red;
            theoryScatter.LineStyle = LineStyle.Dash;

            // Настройка графика
            plt.Title(title, size: 16, bold: true);
            plt.XLabel("Количество операций/токенов");
            plt.YLabel("Время выполнения (мс)");

            plt.Legend(location: Alignment.UpperLeft);

            // Сохранение
            plt.SaveFig(outputPath);
            Console.WriteLine($"График сохранен: {outputPath}");
        }

        public void CreateComplexityReport(List<PerformanceMeasurement> postfixData, List<PerformanceMeasurement> stackData)
        {
            var analyzer = new ComplexityAnalyzer();

            // Анализ сложности постфиксных вычислений
            var postfixAnalysis = analyzer.AnalyzeComplexity(postfixData);
            var postfixTheoretical = analyzer.GenerateTheoreticalData(postfixData, postfixAnalysis.ComplexityType);

            // Анализ сложности операций стека
            var stackAnalysis = analyzer.AnalyzeComplexity(stackData);
            var stackTheoretical = analyzer.GenerateTheoreticalData(stackData, stackAnalysis.ComplexityType);

            // Создание графиков
            string resultsDir = "PerformanceResults";
            Directory.CreateDirectory(resultsDir);

            PlotPerformanceComparison(
                postfixData,
                postfixTheoretical,
                $"Сложность алгоритма постфиксных вычислений\n(Аппроксимация: {postfixAnalysis.ComplexityType}, R² = {postfixAnalysis.RSquared:F4})",
                Path.Combine(resultsDir, "PostfixComplexity.png"));

            PlotPerformanceComparison(
                stackData,
                stackTheoretical,
                $"Сложность операций со стеком\n(Аппроксимация: {stackAnalysis.ComplexityType}, R² = {stackAnalysis.RSquared:F4})",
                Path.Combine(resultsDir, "StackComplexity.png"));

            // Вывод результатов в консоль
            PrintAnalysisResults(postfixAnalysis, "Постфиксные вычисления");
            PrintAnalysisResults(stackAnalysis, "Операции со стеком");
        }

        private void PrintAnalysisResults(ComplexityAnalysis analysis, string algorithmName)
        {
            Console.WriteLine($"\n=== Анализ сложности: {algorithmName} ===");
            Console.WriteLine($"Наиболее вероятная сложность: {analysis.ComplexityType}");
            Console.WriteLine($"Коэффициент детерминации (R²): {analysis.RSquared:F4}");
            Console.WriteLine($"Оценочный коэффициент: {analysis.EstimatedComplexity:E4}");

            if (analysis.RSquared > 0.95)
            {
                Console.WriteLine("✓ Высокая точность аппроксимации");
            }
            else if (analysis.RSquared > 0.85)
            {
                Console.WriteLine("~ Удовлетворительная точность аппроксимации");
            }
            else
            {
                Console.WriteLine("✗ Низкая точность аппроксимации");
            }
        }
    }
}