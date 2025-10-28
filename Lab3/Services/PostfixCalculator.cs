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
                {"^", (x, y) =>
                    {
                        if (x < 0 && Math.Abs(y - (int)y) > 1e-10)
                            throw new ArgumentException("Возведение отрицательного числа в дробную степень");
                        return Math.Pow(x, y);
                    }
                }
            };

            _unaryOperations = new Dictionary<string, Func<double, double>>
            {
                {"ln", x =>
                    {
                        if (x <= 0)
                            throw new ArgumentException("Логарифм не определен для неположительных чисел");
                        return Math.Log(x);
                    }
                },
                {"cos", x => Math.Cos(x)},
                {"sin", x => Math.Sin(x)},
                {"sqrt", x =>
                    {
                        if (x < 0)
                            throw new ArgumentException("Квадратный корень не определен для отрицательных чисел");
                        return Math.Sqrt(x);
                    }
                }
            };
        }

        public double Evaluate(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
                throw new ArgumentException("Выражение не может быть пустым");

            Structures.Stack<double> stack = new Structures.Stack<double>();
            string[] tokens = expression.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // Логирование для отладки
            Console.WriteLine($"Вычисление выражения: {expression}");
            Console.WriteLine($"Количество токенов: {tokens.Length}");

            foreach (string token in tokens)
            {
                try
                {
                    if (IsNumber(token))
                    {
                        double number = ParseNumber(token);
                        stack.Push(number);
                        Console.WriteLine($"  Добавлено число: {number}");
                    }
                    else if (_binaryOperations.ContainsKey(token))
                    {
                        if (stack.Count < 2)
                            throw new InvalidOperationException($"Недостаточно операндов для бинарной операции '{token}'. Нужно 2, есть {stack.Count}");

                        double right = stack.Pop();
                        double left = stack.Pop();
                        double result = _binaryOperations[token](left, right);
                        stack.Push(result);

                        Console.WriteLine($"  {left} {token} {right} = {result}");
                    }
                    else if (_unaryOperations.ContainsKey(token))
                    {
                        if (stack.IsEmpty())
                            throw new InvalidOperationException($"Недостаточно операндов для унарной операции '{token}'");

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
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Ошибка при обработке токена '{token}': {ex.Message}", ex);
                }
            }

            if (stack.Count != 1)
                throw new InvalidOperationException($"Некорректное выражение. В стеке осталось {stack.Count} элементов вместо 1");

            double finalResult = stack.Pop();
            Console.WriteLine($"Результат: {finalResult}");
            return finalResult;
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