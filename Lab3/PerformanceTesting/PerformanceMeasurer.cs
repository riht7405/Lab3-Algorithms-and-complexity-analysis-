using System;
using System.Collections.Generic;
using System.Diagnostics;
using Lab3.Services;

namespace Lab3.PerformanceTesting
{
    public class PerformanceMeasurement
    {
        public int InputSize { get; set; }
        public double ExecutionTimeMs { get; set; }
        public string AlgorithmType { get; set; } = string.Empty;
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
            int maxSize = 10000;
            int step = 100;
           
            int expressionsPerSize = 1;

            for(int size = 0; size < maxSize;size+=step)
            {
                Console.WriteLine($"\n--- Тестирование размера {size} ---");
                var expressions = _generator.GeneratePostfixExpressions(size, size, 1, expressionsPerSize);
                double totalTime = 0;
                int successfulExpressions = 0;

                foreach (string expression in expressions)
                {
                    try
                    {
                        double time = MeasurePostfixEvaluationTime(expression);
                        if (time > 0)
                        {
                            totalTime += time;
                            successfulExpressions++;
                            Console.WriteLine($"  Успешно: {time:F4}мс");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"  Ошибка вычисления: {ex.Message}");
                    }
                }

                if (successfulExpressions > 0)
                {
                    double averageTime = totalTime / successfulExpressions;
                    measurements.Add(new PerformanceMeasurement
                    {
                        InputSize = size,
                        ExecutionTimeMs = averageTime,
                        AlgorithmType = "Postfix Evaluation"
                    });

                    Console.WriteLine($"Среднее время: {averageTime:F4}мс");
                }
                else
                {
                    Console.WriteLine($"Нет успешных измерений для размера {size}");
                }
            }

            Console.WriteLine($"\nВсего собрано измерений: {measurements.Count}");
            return measurements;
        }

        public List<PerformanceMeasurement> MeasureStackOperationsPerformance()
        {
            var measurements = new List<PerformanceMeasurement>();

            int[] operationCounts = { 10, 20, 30, 40, 50 };
            int max = 1000;
            int step = 10;

            for(int count=1; count<max; count+=step)
            {
                Console.WriteLine($"\n--- Тестирование {count} операций ---");
                double totalTime = 0;
                int repetitions = 2;
                int successfulRuns = 0;

                for (int i = 0; i < repetitions; i++)
                {
                    try
                    {
                        var operations = _generator.GenerateStackOperations(count);
                        double time = MeasureStackOperationsTime(operations);
                        totalTime += time;
                        successfulRuns++;
                        Console.WriteLine($"  Попытка {i + 1}: {time:F4}мс");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"  Ошибка выполнения операций: {ex.Message}");
                    }
                }

                if (successfulRuns > 0)
                {
                    double averageTime = totalTime / successfulRuns;
                    measurements.Add(new PerformanceMeasurement
                    {
                        InputSize = count,
                        ExecutionTimeMs = averageTime,
                        AlgorithmType = "Stack Operations"
                    });

                    Console.WriteLine($"Среднее время: {averageTime:F4}мс");
                }
            }

            return measurements;
        }

        private double MeasurePostfixEvaluationTime(string expression)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                // Выполняем несколько раз для более точного измерения
                int repetitions = Math.Max(1, 100 / (expression.Length + 1));
                for (int i = 0; i < repetitions; i++)
                {
                    _calculator.Evaluate(expression);
                }
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
            string tempFile = System.IO.Path.GetTempFileName();
            System.IO.File.WriteAllText(tempFile, string.Join(" ", operations));

            var stopwatch = Stopwatch.StartNew();

            try
            {
                _stackService.ProcessOperationsFromFile(tempFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при выполнении операций стека: {ex.Message}");
            }
            finally
            {
                System.IO.File.Delete(tempFile);
            }

            stopwatch.Stop();
            return stopwatch.Elapsed.TotalMilliseconds;
        }
    }
}