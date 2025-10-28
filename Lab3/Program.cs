using System;
using Lab3.PerformanceTesting;
using Lab3.Visualization;
using Lab3.Services;
using Lab3.PerformanceTesting;
using Lab3.Services;
using Lab3.Visualization;

namespace Lab3
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("=== ТЕСТИРОВАНИЕ ПРОИЗВОДИТЕЛЬНОСТИ И АНАЛИЗ СЛОЖНОСТИ ===");

                // Тестирование производительности
                var measurer = new PerformanceMeasurer();

                Console.WriteLine("\n--- Тестирование постфиксных вычислений ---");
                var postfixMeasurements = measurer.MeasurePostfixEvaluationPerformance();

                Console.WriteLine("\n--- Тестирование операций со стеком ---");
                var stackMeasurements = measurer.MeasureStackOperationsPerformance();

                // Создание отчета с графиками
                Console.WriteLine("\n--- Создание отчетов и графиков ---");
                var plotter = new GraphPlotter();
                plotter.CreateComplexityReport(postfixMeasurements, stackMeasurements);

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