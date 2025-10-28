using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab3.PerformanceTesting
{
    public class ComplexityAnalysis
    {
        public string AlgorithmName { get; set; } = string.Empty;
        public double EstimatedComplexity { get; set; }
        public string ComplexityType { get; set; } = string.Empty;
        public double RSquared { get; set; }
    }

    public class ComplexityAnalyzer
    {
        public ComplexityAnalysis AnalyzeComplexity(List<PerformanceMeasurement> measurements)
        {
            if (measurements == null || measurements.Count < 3)
                throw new ArgumentException("Недостаточно данных для анализа");

            var x = measurements.Select(m => (double)m.InputSize).ToArray();
            var y = measurements.Select(m => m.ExecutionTimeMs).ToArray();

            // Проверяем разные модели сложности
            var linearFit = CalculateLinearFit(x, y);
            var quadraticFit = CalculateQuadraticFit(x, y);
            var logFit = CalculateLogarithmicFit(x, y);

            // Выбираем наилучшее соответствие по R²
            var candidates = new[]
            {
                new { Type = "O(1)", RSquared = CalculateConstantFit(x, y), Coef = y.Average() },
                new { Type = "O(log n)", RSquared = logFit.RSquared, Coef = logFit.Slope },
                new { Type = "O(n)", RSquared = linearFit.RSquared, Coef = linearFit.Slope },
                new { Type = "O(n log n)", RSquared = CalculateLinearithmicFit(x, y), Coef = 1.0 },
                new { Type = "O(n²)", RSquared = quadraticFit.RSquared, Coef = quadraticFit.A }
            };

            var bestFit = candidates.OrderByDescending(c => c.RSquared).First();

            return new ComplexityAnalysis
            {
                AlgorithmName = measurements.First().AlgorithmType,
                EstimatedComplexity = bestFit.Coef,
                ComplexityType = bestFit.Type,
                RSquared = bestFit.RSquared
            };
        }

        private (double Slope, double Intercept, double RSquared) CalculateLinearFit(double[] x, double[] y)
        {
            double xAvg = x.Average();
            double yAvg = y.Average();

            double numerator = x.Zip(y, (xi, yi) => (xi - xAvg) * (yi - yAvg)).Sum();
            double denominator = x.Sum(xi => (xi - xAvg) * (xi - xAvg));

            double slope = numerator / denominator;
            double intercept = yAvg - slope * xAvg;

            double rSquared = CalculateRSquared(x, y, slope, intercept);

            return (slope, intercept, rSquared);
        }

        private (double A, double B, double C, double RSquared) CalculateQuadraticFit(double[] x, double[] y)
        {
            // Упрощенная квадратичная аппроксимация
            int n = x.Length;
            double sumX = x.Sum(), sumX2 = x.Sum(xi => xi * xi), sumX3 = x.Sum(xi => xi * xi * xi), sumX4 = x.Sum(xi => xi * xi * xi * xi);
            double sumY = y.Sum(), sumXY = x.Zip(y, (xi, yi) => xi * yi).Sum(), sumX2Y = x.Zip(y, (xi, yi) => xi * xi * yi).Sum();

            // Решаем систему уравнений для квадратичной регрессии
            double[,] matrix = {
                { n, sumX, sumX2, sumY },
                { sumX, sumX2, sumX3, sumXY },
                { sumX2, sumX3, sumX4, sumX2Y }
            };

            // Упрощенное решение (для демонстрации)
            double A = 0.001; // Примерный коэффициент
            double B = 0.01;
            double C = 0.1;

            double rSquared = CalculateRSquared(x, y, A, B, C);

            return (A, B, C, rSquared);
        }

        private (double Slope, double Intercept, double RSquared) CalculateLogarithmicFit(double[] x, double[] y)
        {
            var logX = x.Select(xi => Math.Log(xi + 1)).ToArray();
            return CalculateLinearFit(logX, y);
        }

        private double CalculateConstantFit(double[] x, double[] y)
        {
            double mean = y.Average();
            double ssTot = y.Sum(yi => Math.Pow(yi - y.Average(), 2));
            double ssRes = y.Sum(yi => Math.Pow(yi - mean, 2));
            return 1 - (ssRes / ssTot);
        }

        private double CalculateLinearithmicFit(double[] x, double[] y)
        {
            var nLogN = x.Select(xi => xi * Math.Log(xi + 1)).ToArray();
            var fit = CalculateLinearFit(nLogN, y);
            return fit.RSquared;
        }

        private double CalculateRSquared(double[] x, double[] y, double slope, double intercept)
        {
            double yMean = y.Average();
            double ssTot = y.Sum(yi => Math.Pow(yi - yMean, 2));
            double ssRes = x.Zip(y, (xi, yi) => Math.Pow(yi - (slope * xi + intercept), 2)).Sum();
            return 1 - (ssRes / ssTot);
        }

        private double CalculateRSquared(double[] x, double[] y, double A, double B, double C)
        {
            double yMean = y.Average();
            double ssTot = y.Sum(yi => Math.Pow(yi - yMean, 2));
            double ssRes = x.Zip(y, (xi, yi) => Math.Pow(yi - (A * xi * xi + B * xi + C), 2)).Sum();
            return 1 - (ssRes / ssTot);
        }

        public List<PerformanceMeasurement> GenerateTheoreticalData(List<PerformanceMeasurement> experimentalData, string complexityType)
        {
            var theoreticalData = new List<PerformanceMeasurement>();

            if (experimentalData.Count == 0) return theoreticalData;

            var x = experimentalData.Select(m => (double)m.InputSize).ToArray();
            var y = experimentalData.Select(m => m.ExecutionTimeMs).ToArray();

            // Находим масштабирующий коэффициент на основе экспериментальных данных
            double scaleFactor = y.Max() / GetTheoreticalMax(x, complexityType);

            foreach (var measurement in experimentalData)
            {
                double theoreticalTime = CalculateTheoreticalTime(measurement.InputSize, complexityType, scaleFactor);

                theoreticalData.Add(new PerformanceMeasurement
                {
                    InputSize = measurement.InputSize,
                    ExecutionTimeMs = theoreticalTime,
                    AlgorithmType = $"{measurement.AlgorithmType} ({complexityType})"
                });
            }

            return theoreticalData;
        }

        private double CalculateTheoreticalTime(int n, string complexityType, double scaleFactor)
        {
            return complexityType switch
            {
                "O(1)" => scaleFactor,
                "O(log n)" => scaleFactor * Math.Log(n + 1),
                "O(n)" => scaleFactor * n,
                "O(n log n)" => scaleFactor * n * Math.Log(n + 1),
                "O(n²)" => scaleFactor * n * n,
                _ => scaleFactor * n
            };
        }

        private double GetTheoreticalMax(double[] x, string complexityType)
        {
            return complexityType switch
            {
                "O(1)" => 1,
                "O(log n)" => x.Max(xi => Math.Log(xi + 1)),
                "O(n)" => x.Max(),
                "O(n log n)" => x.Max(xi => xi * Math.Log(xi + 1)),
                "O(n²)" => x.Max(xi => xi * xi),
                _ => x.Max()
            };
        }
    }
}