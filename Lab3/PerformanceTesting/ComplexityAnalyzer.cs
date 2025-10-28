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
            int n = x.Length;

            // Создаем матрицу для системы уравнений
            double[,] matrix = new double[3, 4];

            // Заполняем матрицу
            for (int i = 0; i < n; i++)
            {
                double xi = x[i];
                double xi2 = xi * xi;
                double xi3 = xi2 * xi;
                double xi4 = xi3 * xi;

                matrix[0, 0] += xi4;
                matrix[0, 1] += xi3;
                matrix[0, 2] += xi2;
                matrix[0, 3] += xi2 * y[i];

                matrix[1, 0] += xi3;
                matrix[1, 1] += xi2;
                matrix[1, 2] += xi;
                matrix[1, 3] += xi * y[i];

                matrix[2, 0] += xi2;
                matrix[2, 1] += xi;
                matrix[2, 2] += 1;
                matrix[2, 3] += y[i];
            }

            // Решаем систему методом Гаусса (упрощенно)
            double A = matrix[0, 3] / matrix[0, 0];
            double B = (matrix[1, 3] - matrix[1, 0] * A) / matrix[1, 1];
            double C = (matrix[2, 3] - matrix[2, 0] * A - matrix[2, 1] * B) / matrix[2, 2];

            // Вычисляем R²
            double yMean = y.Average();
            double ssTot = y.Sum(val => Math.Pow(val - yMean, 2));
            double ssRes = x.Zip(y, (xi, yi) => Math.Pow(yi - (A * xi * xi + B * xi + C), 2)).Sum();
            double rSquared = 1 - (ssRes / ssTot);

            return (A, B, C, rSquared);
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