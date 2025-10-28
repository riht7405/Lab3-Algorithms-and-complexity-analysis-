using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Lab3.Structures;

namespace Lab3.Services
{
    public class InfixToPostfixConverter
    {
        private readonly Dictionary<string, int> _precedence;
        private readonly Dictionary<string, string> _associativity;

        public InfixToPostfixConverter()
        {
            _precedence = new Dictionary<string, int>
            {
                {"+", 1}, {"-", 1},
                {"*", 2}, {"/", 2}, {":", 2},
                {"^", 3},
                {"ln", 4}, {"cos", 4}, {"sin", 4}, {"sqrt", 4}
            };

            _associativity = new Dictionary<string, string>
            {
                {"+", "L"}, {"-", "L"},
                {"*", "L"}, {"/", "L"}, {":", "L"},
                {"^", "R"},
                {"ln", "R"}, {"cos", "R"}, {"sin", "R"}, {"sqrt", "R"}
            };
        }

        public string Convert(string infixExpression)
        {
            Structures.Stack<string> stack = new Structures.Stack<string>();
            List<string> output = new List<string>();

            string[] tokens = Tokenize(infixExpression);

            foreach (string token in tokens)
            {
                if (IsNumber(token))
                {
                    output.Add(token);
                }
                else if (IsFunction(token))
                {
                    stack.Push(token);
                }
                else if (IsOperator(token))
                {
                    while (!stack.IsEmpty() && stack.Top() != "(" &&
                           HasHigherPrecedence(stack.Top(), token))
                    {
                        output.Add(stack.Pop());
                    }
                    stack.Push(token);
                }
                else if (token == "(")
                {
                    stack.Push(token);
                }
                else if (token == ")")
                {
                    while (!stack.IsEmpty() && stack.Top() != "(")
                    {
                        output.Add(stack.Pop());
                    }

                    if (stack.IsEmpty())
                        throw new ArgumentException("Несбалансированные скобки");

                    stack.Pop(); // Убираем "("

                    // Если на вершине стека функция, добавляем её в вывод
                    if (!stack.IsEmpty() && IsFunction(stack.Top()))
                    {
                        output.Add(stack.Pop());
                    }
                }
                else
                {
                    throw new ArgumentException($"Неизвестный токен: {token}");
                }
            }

            // Выталкиваем оставшиеся операторы из стека
            while (!stack.IsEmpty())
            {
                if (stack.Top() == "(")
                    throw new ArgumentException("Несбалансированные скобки");

                output.Add(stack.Pop());
            }

            return string.Join(" ", output);
        }

        private string[] Tokenize(string expression)
        {
            // Добавляем пробелы вокруг операторов и скобок, затем разбиваем
            return expression
                .Replace("(", " ( ")
                .Replace(")", " ) ")
                .Replace("+", " + ")
                .Replace("-", " - ")
                .Replace("*", " * ")
                .Replace("/", " / ")
                .Replace(":", " : ")
                .Replace("^", " ^ ")
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);
        }

        private bool IsNumber(string token)
        {
            return double.TryParse(token, NumberStyles.Any, CultureInfo.InvariantCulture, out _);
        }

        private bool IsOperator(string token)
        {
            return _precedence.ContainsKey(token);
        }

        private bool IsFunction(string token)
        {
            return token == "ln" || token == "cos" || token == "sin" || token == "sqrt";
        }

        private bool HasHigherPrecedence(string operator1, string operator2)
        {
            if (!_precedence.ContainsKey(operator1) || !_precedence.ContainsKey(operator2))
                return false;

            int prec1 = _precedence[operator1];
            int prec2 = _precedence[operator2];

            if (prec1 > prec2)
                return true;
            if (prec1 == prec2 && _associativity[operator2] == "L")
                return true;

            return false;
        }
    }
}