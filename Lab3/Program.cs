using System;
using Lab3.PerformanceTesting;
using Lab3.Visualization;
using Lab3.Services;
using System.Diagnostics;

namespace Lab3
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("=== ТЕСТИРОВАНИЕ ПРОИЗВОДИТЕЛЬНОСТИ И АНАЛИЗ СЛОЖНОСТИ ===");

                var overallStopwatch = Stopwatch.StartNew();

                // Сначала протестируем базовую функциональность
                Console.WriteLine("\n--- Тестирование базовой функциональности ---");
                TestBasicFunctionality();

                // Тестирование производительности
                var measurer = new PerformanceMeasurer();

                Console.WriteLine("\n--- Тестирование постфиксных вычислений ---");
                var postfixMeasurements = measurer.MeasurePostfixEvaluationPerformance();

                Console.WriteLine("\n--- Тестирование операций со стеком ---");
                var stackMeasurements = measurer.MeasureStackOperationsPerformance();

                overallStopwatch.Stop();
                Console.WriteLine($"\nОбщее время выполнения: {overallStopwatch.Elapsed.TotalMinutes:F2} минут");

                // Создание отчета с графиками
                if (postfixMeasurements.Count >= 2 || stackMeasurements.Count >= 2)
                {
                    Console.WriteLine("\n--- Создание отчетов и графиков ---");
                    var plotter = new GraphPlotter();
                    plotter.CreateComplexityReport(postfixMeasurements, stackMeasurements);
                }
                else
                {
                    Console.WriteLine("\nНедостаточно данных для создания графиков");
                }

                // Демонстрация работы стека и вычислений
                DemonstrateOriginalFunctionality();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Критическая ошибка в приложении: {ex.Message}");
                Console.WriteLine($"Стек вызовов: {ex.StackTrace}");
            }

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }

        static void TestBasicFunctionality()
        {
            try
            {
                var calculator = new PostfixCalculator();

                // Простые тестовые выражения
                string[] testExpressions = {
            "3 4 +",
            "5 2 * 3 +",
            "10 5 - 2 *",
            "15 3 / 2 +"
        };

                foreach (string expr in testExpressions)
                {
                    try
                    {
                        double result = calculator.Evaluate(expr);
                        Console.WriteLine($"  {expr} = {result}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"  Ошибка в '{expr}': {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка тестирования: {ex.Message}");
            }
        }

        static void DemonstrateOriginalFunctionality()
        {
            Console.WriteLine("\n--- Демонстрация основного функционала ---");

            try
            {
                var stackService = new StackOperationService();
                if (System.IO.File.Exists("input.txt"))
                {
                    stackService.ProcessOperationsFromFile("input.txt");
                }
                else
                {
                    Console.WriteLine("Файл input.txt не найден. Создаем тестовые данные...");
                    // Создаем простой тестовый файл
                    System.IO.File.WriteAllText("input.txt", "1,10 1,20 3 2 5 4");
                    stackService.ProcessOperationsFromFile("input.txt");
                }

                var calculator = new PostfixCalculator();
                string postfixExpr = "7 8 + 3 2 + /";
                double result = calculator.Evaluate(postfixExpr);
                Console.WriteLine($"\nПример вычисления: {postfixExpr} = {result:F4}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при демонстрации: {ex.Message}");
            }
        }
    }
}