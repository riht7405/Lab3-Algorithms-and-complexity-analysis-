using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab3.PerformanceTesting
{
    public class TestDataGenerator
    {
        private readonly Random _random;

        public TestDataGenerator()
        {
            _random = new Random(42); // Фиксируем seed для воспроизводимости
        }

        public List<string> GeneratePostfixExpressions(int minLength, int maxLength, int step, int expressionsPerStep)
        {
            var expressions = new List<string>();

            for (int length = minLength; length <= maxLength; length += step)
            {
                for (int i = 0; i < expressionsPerStep; i++)
                {
                    try
                    {
                        string expression = GeneratePostfixExpression(length);
                        expressions.Add(expression);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка при генерации выражения длины {length}: {ex.Message}");
                    }
                }
            }

            return expressions;
        }

        private string GeneratePostfixExpression(int tokenCount)
        {
            // Упрощенная генерация - только простые выражения
            var tokens = new List<string>();
            int numbersNeeded = 1; // Начинаем с одного числа

            for (int i = 0; i < tokenCount; i++)
            {
                if (numbersNeeded > 0)
                {
                    // Нужно добавить число
                    tokens.Add(GenerateNumber());
                    numbersNeeded--;
                }
                else
                {
                    // Можем добавить оператор
                    tokens.Add(GenerateSimpleOperator());
                    numbersNeeded += 1; // Бинарный оператор требует два операнда, но один уже есть в стеке
                }

                // Если это последний токен и нам еще нужны числа, добавляем числа
                if (i == tokenCount - 1 && numbersNeeded > 0)
                {
                    for (int j = 0; j < numbersNeeded; j++)
                    {
                        tokens.Add(GenerateNumber());
                    }
                }
            }

            return string.Join(" ", tokens);
        }

        private string GenerateNumber()
        {
            return _random.Next(1, 100).ToString();
        }

        private string GenerateSimpleOperator()
        {
            string[] operators = { "+", "-", "*" };
            return operators[_random.Next(operators.Length)];
        }

        public List<string> GenerateStackOperations(int operationCount)
        {
            var operations = new List<string>();
            int stackSize = 0;

            for (int i = 0; i < operationCount; i++)
            {
                // Определяем доступные операции на основе текущего размера стека
                List<int> availableOperations = new List<int> { 1, 4 }; // Push и IsEmpty всегда доступны

                if (stackSize > 0)
                {
                    availableOperations.Add(2); // Pop
                    availableOperations.Add(3); // Top
                    availableOperations.Add(5); // Print
                }

                int operationType = availableOperations[_random.Next(availableOperations.Count)];

                switch (operationType)
                {
                    case 1: // Push
                        operations.Add($"1,{GenerateStackValue()}");
                        stackSize++;
                        break;
                    case 2: // Pop
                        operations.Add("2");
                        stackSize--;
                        break;
                    case 3: // Top
                        operations.Add("3");
                        break;
                    case 4: // IsEmpty
                        operations.Add("4");
                        break;
                    case 5: // Print
                        operations.Add("5");
                        break;
                }
            }

            return operations;
        }

        private string GenerateStackValue()
        {
            return _random.Next(1, 100).ToString();
        }
    }
}