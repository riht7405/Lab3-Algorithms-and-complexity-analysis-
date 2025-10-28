using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using Lab3.Structures;

namespace Lab3.Services
{
    public class StackOperationService
    {
        private readonly Structures.Stack<object> _stack;

        public StackOperationService()
        {
            _stack = new Structures.Stack<object>();
        }

        public void ProcessOperationsFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Файл {filePath} не найден!");
                return;
            }

            string fileContent = File.ReadAllText(filePath).Trim();
            string[] operations = fileContent.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            Console.WriteLine($"Найдено операций: {operations.Length}");

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            foreach (string operation in operations)
            {
                ProcessSingleOperation(operation);
            }

            stopwatch.Stop();
            Console.WriteLine($"\nВремя выполнения всех операций: {stopwatch.Elapsed.TotalMilliseconds:F4} мс");
        }

        private void ProcessSingleOperation(string operation)
        {
            try
            {
                switch (operation)
                {
                    case "2": // Pop
                        ExecutePop();
                        break;

                    case "3": // Top
                        ExecuteTop();
                        break;

                    case "4": // IsEmpty
                        ExecuteIsEmpty();
                        break;

                    case "5": // Print
                        ExecutePrint();
                        break;

                    default:
                        if (operation.StartsWith("1,"))
                        {
                            ExecutePush(operation);
                        }
                        else
                        {
                            Console.WriteLine($"Неизвестная операция: {operation}");
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при выполнении операции '{operation}': {ex.Message}");
            }
        }

        private void ExecutePush(string operation)
        {
            string valueStr = operation.Substring(2);
            object value = ParseValue(valueStr);
            _stack.Push(value);
            Console.WriteLine($"Push: {value} ({value.GetType().Name})");
        }

        private void ExecutePop()
        {
            try
            {
                object result = _stack.Pop();
                Console.WriteLine($"Pop: {result}");
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("Pop: Стек пуст");
            }
        }

        private void ExecuteTop()
        {
            try
            {
                object result = _stack.Top();
                Console.WriteLine($"Top: {result}");
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("Top: Стек пуст");
            }
        }

        private void ExecuteIsEmpty()
        {
            bool isEmpty = _stack.IsEmpty();
            Console.WriteLine($"IsEmpty: {isEmpty}");
        }

        private void ExecutePrint()
        {
            Console.Write("Print: ");
            _stack.Print();
        }

        private object ParseValue(string valueStr)
        {
            // Пробуем разные форматы чисел
            if (int.TryParse(valueStr, NumberStyles.Any, CultureInfo.InvariantCulture, out int intValue))
                return intValue;
            if (double.TryParse(valueStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double doubleValue))
                return doubleValue;
            return valueStr; // Если не число, возвращаем как строку
        }
    }
}