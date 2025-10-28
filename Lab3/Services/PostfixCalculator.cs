using System;
using System.Collections.Generic;
using System.Globalization;
using Lab3.Structures;

namespace Lab3.Services
{
    public class PostfixCalculator
    {
        private readonly Dictionary<string, Func<double, double, double>> _binaryOperations;

        public PostfixCalculator()
        {
            _binaryOperations = new Dictionary<string, Func<double, double, double>>
            {
                {"+", (x, y) => x + y},
                {"-", (x, y) => x - y},
                {"*", (x, y) => x * y},
                {"/", (x, y) =>
                    {
                        if (Math.Abs(y) < 1e-10)
                            throw new DivideByZeroException("Деление на ноль");
                        return x / y;
                    }
                },
                {":", (x, y) =>
                    {
                        if (Math.Abs(y) < 1e-10)
                            throw new DivideByZeroException("Деление на ноль");
                        return x / y;
                    }
                },
                {"^", (x, y) => Math.Pow(x, y)}
            };
        }

        public double Evaluate(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
                throw new ArgumentException("Выражение не может быть пустым");

            Structures.Stack<double> stack = new Structures.Stack<double>();
            string[] tokens = expression.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            foreach (string token in tokens)
            {
                if (double.TryParse(token, NumberStyles.Any, CultureInfo.InvariantCulture, out double number))
                {
                    stack.Push(number);
                }
                else if (_binaryOperations.ContainsKey(token))
                {
                    if (stack.Count < 2)
                        throw new InvalidOperationException($"Недостаточно операндов для операции '{token}'. Нужно 2, есть {stack.Count}");

                    double right = stack.Pop();
                    double left = stack.Pop();
                    double result = _binaryOperations[token](left, right);
                    stack.Push(result);
                }
                else
                {
                    throw new ArgumentException($"Неизвестный оператор: '{token}'");
                }
            }

            if (stack.Count != 1)
                throw new InvalidOperationException($"Некорректное выражение. В стеке осталось {stack.Count} элементов вместо 1");

            return stack.Pop();
        }
    }
}