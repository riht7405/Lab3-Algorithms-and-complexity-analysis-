using System;
using System.Diagnostics;
using Lab3.Services;
using Lab3.PerformanceTesting;
using Lab3.Visualization;

namespace Lab3
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("=== АНАЛИЗ ВРЕМЕННОЙ СЛОЖНОСТИ ПОСТФИКСНЫХ ВЫЧИСЛЕНИЙ ===");

                var overallStopwatch = Stopwatch.StartNew();

                // Тестирование базовой функциональности
                Console.WriteLine("\n--- Тестирование базовой функциональности ---");
                TestBasicFunctionality();

                // Тестирование производительности
                Console.WriteLine("\n--- Тестирование производительности ---");
                var measurer = new PerformanceMeasurer();

                Console.WriteLine("\n=== ИЗМЕРЕНИЕ ПОСТФИКСНЫХ ВЫЧИСЛЕНИЙ ===");
                var postfixMeasurements = measurer.MeasurePostfixEvaluationPerformance();

                Console.WriteLine("\n=== ИЗМЕРЕНИЕ ОПЕРАЦИЙ СО СТЕКОМ ===");
                var stackMeasurements = measurer.MeasureStackOperationsPerformance();

                overallStopwatch.Stop();
                Console.WriteLine($"\n⏱️ Общее время тестирования: {overallStopwatch.Elapsed.TotalSeconds:F2} секунд");

                // Создание отчетов с графиками
                if (postfixMeasurements.Count >= 3 && stackMeasurements.Count >= 3)
                {
                    Console.WriteLine("\n=== СОЗДАНИЕ ОТЧЕТОВ И ГРАФИКОВ ===");
                    var plotter = new GraphPlotter();
                    plotter.CreateComplexityReport(postfixMeasurements, stackMeasurements);
                    Console.WriteLine("✅ Отчеты успешно созданы в папке PerformanceResults/");
                }
                else
                {
                    Console.WriteLine("\n⚠️ Недостаточно данных для создания графиков");
                    Console.WriteLine($"   Постфиксные измерения: {postfixMeasurements.Count} (нужно >= 3)");
                    Console.WriteLine($"   Измерения стека: {stackMeasurements.Count} (нужно >= 3)");
                }

                // Демонстрация работы
                Console.WriteLine("\n--- Демонстрация основного функционала ---");
                DemonstrateOriginalFunctionality();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Критическая ошибка: {ex.Message}");
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

                string[] testExpressions = {
                    "3 4 +",           // 7
                    "5 2 * 3 +",       // 13
                    "10 5 - 2 *",      // 10
                    "15 3 / 2 +",      // 7
                    "2 3 4 * +",       // 14
                    "7 8 + 3 2 + /"    // 3
                };

                foreach (string expr in testExpressions)
                {
                    try
                    {
                        double result = calculator.Evaluate(expr);
                        Console.WriteLine($"  ✅ {expr} = {result}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"  ❌ Ошибка в '{expr}': {ex.Message}");
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
            try
            {
                var stackService = new StackOperationService();

                // Создаем тестовый файл
                System.IO.File.WriteAllText("input.txt", "1,10 1,20 3 2 5 4");
                Console.WriteLine("Создан тестовый файл input.txt");

                stackService.ProcessOperationsFromFile("input.txt");

                // Демонстрация конвертации инфиксной записи
                Console.WriteLine("\n--- Конвертация инфиксной записи ---");
                var converter = new InfixToPostfixConverter();
                string infixExpr = "( 2 + 3 ) * 5";
                string postfixExpr = converter.Convert(infixExpr);
                Console.WriteLine($"  Инфиксная: {infixExpr}");
                Console.WriteLine($"  Постфиксная: {postfixExpr}");

                var calculator = new PostfixCalculator();
                double result = calculator.Evaluate(postfixExpr);
                Console.WriteLine($"  Результат: {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при демонстрации: {ex.Message}");
            }
        }
    }
}