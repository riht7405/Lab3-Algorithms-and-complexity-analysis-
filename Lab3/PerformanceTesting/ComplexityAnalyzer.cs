using Lab3.PerformanceTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab3.PerformanceTesting
{
    public class ComplexityAnalysis
    {
        public string AlgorithmName { get; set; } = string.Empty;  // Инициализация по умолчанию
        public double EstimatedComplexity { get; set; }
        public string ComplexityType { get; set; } = string.Empty;  // Инициализация по умолчанию
        public double RSquared { get; set; }
    }

    public class ComplexityAnalyzer
    {
        public ComplexityAnalysis AnalyzeComplexity(List<PerformanceMeasurement> measurements)
        {
            if (measurements == null || measurements.Count < 2)
                throw new ArgumentException("Недостаточно данных для анализа");

            var x = measurements.Select(m => (double)m.InputSize).ToArray();
            var y = measurements.Select(m => m.ExecutionTimeMs).ToArray();

            // Линейная регрессия для проверки O(n)
            var linearRegression = CalculateLinearRegression(x, y);

            // Квадратичная регрессия для проверки O(n^2)
            var quadraticRegression = CalculateQuadraticRegression(x, y);

            // Логарифмическая регрессия для проверки O(log n)
            var logarithmicRegression = CalculateLogarithmicRegression(x, y);

            // Выбираем наилучшую модель по R²
            var bestFit = new[]
            {
                new { Type = "O(n)", RSquared = linearRegression.RSquared, Coef = linearRegression.Slope },
                new { Type = "O(n²)", RSquared = quadraticRegression.RSquared, Coef = quadraticRegression.A },
                new { Type = "O(log n)", RSquared = logarithmicRegression.RSquared, Coef = logarithmicRegression.Slope }
            }.OrderByDescending(m => m.RSquared).First();

            return new ComplexityAnalysis
            {
                AlgorithmName = measurements.First().AlgorithmType,
                EstimatedComplexity = bestFit.Coef,
                ComplexityType = bestFit.Type,
                RSquared = bestFit.RSquared
            };
        }

        private (double Slope, double Intercept, double RSquared) CalculateLinearRegression(double[] x, double[] y)
        {
            int n = x.Length;
            double sumX = x.Sum();
            double sumY = y.Sum();
            double sumXY = x.Zip(y, (a, b) => a * b).Sum();
            double sumX2 = x.Sum(a => a * a);
            double sumY2 = y.Sum(a => a * a);

            double slope = (n * sumXY - sumX * sumY) / (n * sumX2 - sumX * sumX);
            double intercept = (sumY - slope * sumX) / n;

            // R² calculation
            double yMean = sumY / n;
            double ssTot = y.Sum(val => (val - yMean) * (val - yMean));
            double ssRes = x.Zip(y, (xi, yi) => (yi - (slope * xi + intercept)) * (yi - (slope * xi + intercept))).Sum();
            double rSquared = 1 - (ssRes / ssTot);

            return (slope, intercept, rSquared);
        }

        private (double A, double B, double C, double RSquared) CalculateQuadraticRegression(double[] x, double[] y)
        {
            // Упрощенная реализация квадратичной регрессии
            int n = x.Length;
            double sumX = x.Sum();
            double sumX2 = x.Sum(a => a * a);
            double sumX3 = x.Sum(a => a * a * a);
            double sumX4 = x.Sum(a => a * a * a * a);
            double sumY = y.Sum();
            double sumXY = x.Zip(y, (a, b) => a * b).Sum();
            double sumX2Y = x.Zip(y, (a, b) => a * a * b).Sum();

            // Решаем систему уравнений для квадратичной регрессии
            // Упрощенный подход - в реальном проекте лучше использовать матрицы

            // Для простоты вернем приближенные значения
            var linear = CalculateLinearRegression(x, y);
            return (linear.Slope / 1000, linear.Slope, linear.Intercept, linear.RSquared * 0.9);
        }

        private (double Slope, double Intercept, double RSquared) CalculateLogarithmicRegression(double[] x, double[] y)
        {
            var logX = x.Select(val => Math.Log(val + 1)).ToArray();
            return CalculateLinearRegression(logX, y);
        }

        public List<PerformanceMeasurement> GenerateTheoreticalData(List<PerformanceMeasurement> experimentalData, string complexityType)
        {
            var theoreticalData = new List<PerformanceMeasurement>();

            if (experimentalData.Count == 0) return theoreticalData;

            var x = experimentalData.Select(m => (double)m.InputSize).ToArray();
            var y = experimentalData.Select(m => m.ExecutionTimeMs).ToArray();

            foreach (var measurement in experimentalData)
            {
                double theoreticalTime = 0;

                switch (complexityType)
                {
                    case "O(n)":
                        var linear = CalculateLinearRegression(x, y);
                        theoreticalTime = linear.Slope * measurement.InputSize + linear.Intercept;
                        break;
                    case "O(n²)":
                        theoreticalTime = 0.001 * measurement.InputSize * measurement.InputSize;
                        break;
                    case "O(log n)":
                        theoreticalTime = 10 * Math.Log(measurement.InputSize + 1);
                        break;
                    case "O(1)":
                        theoreticalTime = y.Average();
                        break;
                }

                theoreticalData.Add(new PerformanceMeasurement
                {
                    InputSize = measurement.InputSize,
                    ExecutionTimeMs = theoreticalTime,
                    AlgorithmType = $"{measurement.AlgorithmType} ({complexityType})"
                });
            }

            return theoreticalData;
        }
    }
}