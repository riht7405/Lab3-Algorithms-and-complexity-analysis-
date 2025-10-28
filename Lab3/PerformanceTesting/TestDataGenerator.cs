using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lab3.PerformanceTesting
{
    public class TestDataGenerator
    {
        private readonly Random _random;

        public TestDataGenerator()
        {
            _random = new Random(42);
        }

        public List<string> GeneratePostfixExpressions(int minLength, int maxLength, int step, int expressionsPerStep)
        {
            var expressions = new List<string>();

            // Гарантируем, что размеры корректны
            minLength = Math.Max(minLength, 3); // Минимум 3 токена для валидного выражения
            maxLength = Math.Min(maxLength, 1000); // Ограничиваем максимальный размер

            for (int length = minLength; length <= maxLength; length += step)
            {
                for (int i = 0; i < expressionsPerStep; i++)
                {
                    // Генерируем гарантированно корректное выражение
                    expressions.Add(GenerateValidPostfixExpression(length));
                }

                // Ограничиваем общее количество выражений
                if (expressions.Count >= 15)
                    break;
            }

            return expressions;
        }

        private string GenerateValidPostfixExpression(int tokenCount)
        {
            // Гарантируем нечетное количество токенов для корректного выражения
            if (tokenCount % 2 == 0) tokenCount++;

            var tokens = new List<string>();
            int numberCount = 0;
            int operatorCount = 0;

            // Алгоритм, гарантирующий корректность постфиксной записи
            for (int i = 0; i < tokenCount; i++)
            {
                // Всегда можем добавить число, если нужно больше чисел чем операторов
                bool canAddNumber = numberCount <= operatorCount;
                bool canAddOperator = numberCount >= operatorCount + 2;

                if (canAddNumber && (!canAddOperator || _random.NextDouble() < 0.6))
                {
                    tokens.Add(GenerateSafeNumber());
                    numberCount++;
                }
                else if (canAddOperator)
                {
                    tokens.Add(GenerateSafeOperator());
                    operatorCount++;
                }
                else
                {
                    // Если нельзя добавить оператор, добавляем число
                    tokens.Add(GenerateSafeNumber());
                    numberCount++;
                }
            }

            // Гарантируем, что выражение заканчивается оператором
            while (operatorCount < numberCount - 1)
            {
                tokens.Add(GenerateSafeOperator());
                operatorCount++;
            }

            return string.Join(" ", tokens);
        }

        private string GenerateSafeNumber()
        {
            // Генерируем только положительные числа
            return _random.Next(1, 100).ToString();
        }

        private string GenerateSafeOperator()
        {
            // Используем только базовые операторы, которые точно работают
            string[] safeOperators = { "+", "-", "*" };
            return safeOperators[_random.Next(safeOperators.Length)];
        }

        // Генерация операций для стека
        public List<string> GenerateStackOperations(int operationCount)
        {
            var operations = new List<string>();

            // Гарантируем, что стек не опустошается полностью
            int stackSize = 0;

            for (int i = 0; i < operationCount; i++)
            {
                // Выбираем операцию в зависимости от текущего размера стека
                int operationType;

                if (stackSize == 0)
                {
                    // Если стек пуст, можно только Push
                    operationType = 1;
                }
                else if (stackSize < 3)
                {
                    // Если стек маленький, чаще Push, реже другие операции
                    operationType = _random.Next(1, 6) <= 3 ? 1 : _random.Next(2, 6);
                }
                else
                {
                    // Если стек большой, более разнообразные операции
                    operationType = _random.Next(1, 6);
                }

                switch (operationType)
                {
                    case 1: // Push
                        operations.Add($"1,{GenerateStackValue()}");
                        stackSize++;
                        break;
                    case 2: // Pop
                        operations.Add("2");
                        if (stackSize > 0) stackSize--;
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
            if (_random.NextDouble() < 0.8)
            {
                return _random.Next(1, 100).ToString();
            }
            else
            {
                string[] words = { "cat", "dog", "test", "hello", "world" };
                return words[_random.Next(words.Length)];
            }
        }
    }
}