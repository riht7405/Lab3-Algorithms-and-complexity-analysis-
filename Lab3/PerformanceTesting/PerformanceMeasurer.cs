using System;
using System.Collections.Generic;
using System.Diagnostics;
using Lab3.Services;

namespace Lab3.PerformanceTesting
{
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

            int expressionsPerSize = 5;
            int max = 300000;
            int step = 100;

            for(int size=1; size < max;size+=step)
            {
                Console.WriteLine($"\n--- Тестирование размера {size} ---");
                var expressions = _generator.GeneratePostfixExpressions(size, size, 1, expressionsPerSize);
                double totalTime = 0;
                int successfulExpressions = 0;

                foreach (string expression in expressions)
                {
                    try
                    {
                        // Показываем выражение для отладки
                        if (successfulExpressions == 0)
                        {
                            Console.WriteLine($"  Выражение: {expression}");
                        }

                        double time = MeasurePostfixEvaluationTime(expression);
                        if (time > 0)
                        {
                            totalTime += time;
                            successfulExpressions++;
                            Console.WriteLine($"  ✅ Успешно: {time:F6}мс");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"  ❌ Ошибка вычисления: {ex.Message}");
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
                    Console.WriteLine($"📊 Среднее время: {averageTime:F6}мс (успешных: {successfulExpressions})");
                }
                else
                {
                    Console.WriteLine($"⚠️ Нет успешных измерений для размера {size}");
                }
            }

            Console.WriteLine($"\n📊 ИТОГО: Собрано измерений постфиксных вычислений: {measurements.Count}");
            return measurements;
        }

        public List<PerformanceMeasurement> MeasureStackOperationsPerformance()
        {
            var measurements = new List<PerformanceMeasurement>();

            int[] operationCounts = { 10, 20, 50, 100, 200, 500, 1000, 1500, 2000, 3000, 4000, 5000 };

            foreach (int count in operationCounts)
            {
                Console.WriteLine($"\n--- Тестирование {count} операций ---");
                double totalTime = 0;
                int repetitions = 5;
                int successfulRuns = 0;

                for (int i = 0; i < repetitions; i++)
                {
                    try
                    {
                        var operations = _generator.GenerateStackOperations(count);
                        double time = MeasureStackOperationsTime(operations);
                        totalTime += time;
                        successfulRuns++;
                        Console.WriteLine($"  ✅ Попытка {i + 1}: {time:F4}мс");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"  ❌ Попытка {i + 1}: {ex.Message}");
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
                    Console.WriteLine($"📊 Среднее время: {averageTime:F4}мс");
                }
            }

            Console.WriteLine($"\n📊 ИТОГО: Собрано измерений операций стека: {measurements.Count}");
            return measurements;
        }

        private double MeasurePostfixEvaluationTime(string expression)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                // АДАПТИВНЫЕ ПОВТОРЕНИЯ ДЛЯ БОЛЬШИХ ВЫРАЖЕНИЙ
                int repetitions = CalculateRepetitions(expression.Length);

                for (int i = 0; i < repetitions; i++)
                {
                    _calculator.Evaluate(expression);
                }

                stopwatch.Stop();
                double timePerIteration = stopwatch.Elapsed.TotalMilliseconds / repetitions;

                // Для очень больших выражений логируем информацию
                if (expression.Length > 1000)
                {
                    Console.WriteLine($"  📏 Длина: {expression.Length} токенов, повторений: {repetitions}, время: {timePerIteration:F6}мс");
                }

                return timePerIteration;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                throw new InvalidOperationException($"Ошибка вычисления (длина: {expression.Length}): {ex.Message}");
            }
        }


        private int CalculateRepetitions(int expressionLength)
        {
            // АДАПТИВНАЯ ЛОГИКА ДЛЯ БОЛЬШИХ ВЫРАЖЕНИЙ
            if (expressionLength <= 10) return 10000;
            if (expressionLength <= 50) return 5000;
            if (expressionLength <= 100) return 1000;
            if (expressionLength <= 500) return 100;
            if (expressionLength <= 1000) return 10;
            if (expressionLength <= 5000) return 5;
            if (expressionLength <= 10000) return 2;
            return 1; // Для очень больших выражений - 1 повторение
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
            finally
            {
                System.IO.File.Delete(tempFile);
            }

            stopwatch.Stop();
            return stopwatch.Elapsed.TotalMilliseconds;
        }
    }

    public class PerformanceMeasurement
    {
        public int InputSize { get; set; }
        public double ExecutionTimeMs { get; set; }
        public string AlgorithmType { get; set; } = string.Empty;
    }
}