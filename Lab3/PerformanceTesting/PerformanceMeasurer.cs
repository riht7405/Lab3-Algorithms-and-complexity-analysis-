using System;
using System.Collections.Generic;
using System.Diagnostics;
using Lab3.PerformanceTesting;
using Lab3.Services;

namespace Lab3.PerformanceTesting
{
    public class PerformanceMeasurement
    {
        public int InputSize { get; set; }
        public double ExecutionTimeMs { get; set; }
        public string AlgorithmType { get; set; } = string.Empty;  // Инициализация по умолчанию
    }

    public class PerformanceMeasurer
    {
        private readonly PostfixCalculator _calculator;
        private readonly StackOperationService _stackService;
        private readonly TestDataGenerator _generator;

        public PerformanceMeasurer()
        {
            _calculator = new PostfixCalculator();
            _stackService = new StackOperationService();
            _generator = new TestDataGenerator();
        }

        public List<PerformanceMeasurement> MeasurePostfixEvaluationPerformance()
        {
            var measurements = new List<PerformanceMeasurement>();

            // Тестируем на различных размерах входных данных
            int[] inputSizes = { 10, 50, 100, 500, 1000, 5000, 10000 };
            int expressionsPerSize = 5;

            foreach (int size in inputSizes)
            {
                var expressions = _generator.GeneratePostfixExpressions(size, size, 1, expressionsPerSize);
                double totalTime = 0;

                foreach (string expression in expressions)
                {
                    totalTime += MeasurePostfixEvaluationTime(expression);
                }

                double averageTime = totalTime / expressionsPerSize;
                measurements.Add(new PerformanceMeasurement
                {
                    InputSize = size,
                    ExecutionTimeMs = averageTime,
                    AlgorithmType = "Postfix Evaluation"
                });

                Console.WriteLine($"Postfix Evaluation - Size: {size}, Time: {averageTime:F4}ms");
            }

            return measurements;
        }

        public List<PerformanceMeasurement> MeasureStackOperationsPerformance()
        {
            var measurements = new List<PerformanceMeasurement>();

            int[] operationCounts = { 10, 50, 100, 500, 1000, 5000, 10000 };

            foreach (int count in operationCounts)
            {
                double totalTime = 0;
                int repetitions = 3;

                for (int i = 0; i < repetitions; i++)
                {
                    var operations = _generator.GenerateStackOperations(count);
                    totalTime += MeasureStackOperationsTime(operations);
                }

                double averageTime = totalTime / repetitions;
                measurements.Add(new PerformanceMeasurement
                {
                    InputSize = count,
                    ExecutionTimeMs = averageTime,
                    AlgorithmType = "Stack Operations"
                });

                Console.WriteLine($"Stack Operations - Count: {count}, Time: {averageTime:F4}ms");
            }

            return measurements;
        }

        private double MeasurePostfixEvaluationTime(string expression)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                _calculator.Evaluate(expression);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при вычислении выражения: {ex.Message}");
                return 0;
            }

            stopwatch.Stop();
            return stopwatch.Elapsed.TotalMilliseconds;
        }

        private double MeasureStackOperationsTime(List<string> operations)
        {
            // Временное сохранение операций в файл для тестирования
            string tempFile = System.IO.Path.GetTempFileName();
            System.IO.File.WriteAllText(tempFile, string.Join(" ", operations));

            var stopwatch = Stopwatch.StartNew();

            try
            {
                // Вызываем метод, который обрабатывает операции из файла
                // Для этого нужно немного изменить StackOperationService
                _stackService.ProcessOperationsFromFile(tempFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при выполнении операций стека: {ex.Message}");
            }
            finally
            {
                // Удаляем временный файл
                System.IO.File.Delete(tempFile);
            }

            stopwatch.Stop();
            return stopwatch.Elapsed.TotalMilliseconds;
        }
    }
}