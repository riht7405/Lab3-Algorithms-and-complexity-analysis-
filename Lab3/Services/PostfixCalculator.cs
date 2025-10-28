using System;
using System.Collections.Generic;
using System.Globalization;
using Lab3.Structures;

namespace Lab3.Services
{
    public class PostfixCalculator
    {
        private readonly Dictionary<string, Func<double, double, double>> _binaryOperations;
        private readonly Dictionary<string, Func<double, double>> _unaryOperations;

        public PostfixCalculator()
        {
            // Бинарные операции
            _binaryOperations = new Dictionary<string, Func<double, double, double>>
            {
                {"+", (x, y) => x + y},
                {"-", (x, y) => x - y},
                {"*", (x, y) => x * y},
                {"/", (x, y) => x / y},
                {":", (x, y) => x / y},
                {"^", (x, y) => Math.Pow(x, y)}
            };

            // Унарные операции
            _unaryOperations = new Dictionary<string, Func<double, double>>
            {
                {"ln", x => Math.Log(x)},
                {"cos", x => Math.Cos(x)},
                {"sin", x => Math.Sin(x)},
                {"sqrt", x => Math.Sqrt(x)}
            };
        }

        public double Evaluate(string expression)
        {
            Structures.Stack<double> stack = new Structures.Stack<double>();
            string[] tokens = expression.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            foreach (string token in tokens)
            {
                if (IsNumber(token))
                {
                    double number = ParseNumber(token);
                    stack.Push(number);
                }
                else if (_binaryOperations.ContainsKey(token))
                {
                    if (stack.Count < 2)
                        throw new InvalidOperationException($"Недостаточно операндов для операции '{token}'");

                    double right = stack.Pop();
                    double left = stack.Pop();
                    double result = _binaryOperations[token](left, right);
                    stack.Push(result);

                    Console.WriteLine($"  {left} {token} {right} = {result}");
                }
                else if (_unaryOperations.ContainsKey(token))
                {
                    if (stack.IsEmpty())
                        throw new InvalidOperationException($"Недостаточно операндов для операции '{token}'");

                    double operand = stack.Pop();
                    double result = _unaryOperations[token](operand);
                    stack.Push(result);

                    Console.WriteLine($"  {token}({operand}) = {result}");
                }
                else
                {
                    throw new ArgumentException($"Неизвестный оператор: '{token}'");
                }
            }

            if (stack.Count != 1)
                throw new InvalidOperationException("Некорректное выражение");

            return stack.Pop();
        }

        private bool IsNumber(string token)
        {
            return double.TryParse(token, NumberStyles.Any, CultureInfo.InvariantCulture, out _);
        }

        private double ParseNumber(string token)
        {
            if (double.TryParse(token, NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
            {
                return result;
            }
            throw new ArgumentException($"Неверный формат числа: '{token}'");
        }
    }
}