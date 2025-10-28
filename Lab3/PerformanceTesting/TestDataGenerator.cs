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
            _random = new Random(42);
        }

        public List<string> GeneratePostfixExpressions(int minLength, int maxLength, int step, int expressionsPerStep)
        {
            var expressions = new List<string>();

            for (int size = minLength; size <= maxLength; size += step)
            {
                // Для постфиксной записи количество токенов должно быть нечетным
                int actualSize = (size % 2 == 0) ? size + 1 : size;

                for (int i = 0; i < expressionsPerStep; i++)
                {
                    string expression = GenerateGuaranteedValidPostfix(actualSize);
                    if (IsValidPostfixExpression(expression))
                    {
                        expressions.Add(expression);
                    }
                    else
                    {
                        // Если выражение невалидно, создаем простое гарантированно рабочее выражение
                        expressions.Add(CreateSimpleValidExpression(actualSize));
                    }
                }
            }

            return expressions;
        }

        private string GenerateGuaranteedValidPostfix(int tokenCount)
        {
            if (tokenCount < 3) tokenCount = 3;

            // ГАРАНТИРОВАННО КОРРЕКТНЫЙ АЛГОРИТМ ГЕНЕРАЦИИ
            // Для постфиксной записи: numbers = operators + 1
            // total_tokens = numbers + operators = 2 * operators + 1
            int operatorCount = (tokenCount - 1) / 2;
            int numberCount = operatorCount + 1;

            // Проверяем и корректируем математику
            while (numberCount + operatorCount != tokenCount)
            {
                if (numberCount + operatorCount > tokenCount)
                {
                    operatorCount--;
                    numberCount = operatorCount + 1;
                }
                else
                {
                    operatorCount++;
                    numberCount = operatorCount + 1;
                }
            }

            var tokens = new List<string>();

            // 1. Добавляем ВСЕ числа сначала
            for (int i = 0; i < numberCount; i++)
            {
                // Используем безопасные числа (1-50 чтобы избежать переполнения)
                tokens.Add(_random.Next(1, 51).ToString());
            }

            // 2. Добавляем ВСЕ операторы после чисел
            string[] safeOperators = GetSafeOperatorsForSize(tokenCount);
            for (int i = 0; i < operatorCount; i++)
            {
                tokens.Add(safeOperators[_random.Next(safeOperators.Length)]);
            }

            return string.Join(" ", tokens);
        }

        private string[] GetSafeOperatorsForSize(int size)
        {
            // Для очень больших выражений используем ТОЛЬКО безопасные операторы
            if (size > 1000)
            {
                return new[] { "+", "*" }; // Только сложение и умножение
            }
            else if (size > 100)
            {
                return new[] { "+", "*", "+", "-", "*", "+" }; // В основном безопасные
            }
            else
            {
                return new[] { "+", "-", "*", "/" }; // Все базовые операторы
            }
        }

        private string CreateSimpleValidExpression(int tokenCount)
        {
            // Резервный метод: создаем максимально простое гарантированно рабочее выражение
            if (tokenCount < 3) tokenCount = 3;

            int operatorCount = (tokenCount - 1) / 2;
            int numberCount = operatorCount + 1;

            var tokens = new List<string>();

            // Добавляем числа
            for (int i = 0; i < numberCount; i++)
            {
                tokens.Add("2"); // Всегда используем 2 (безопасное число)
            }

            // Добавляем операторы (только сложение - самый безопасный)
            for (int i = 0; i < operatorCount; i++)
            {
                tokens.Add("+");
            }

            return string.Join(" ", tokens);
        }

        private bool IsValidPostfixExpression(string expression)
        {
            try
            {
                // Быстрая проверка без полного вычисления
                string[] tokens = expression.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                // Быстрая проверка структуры
                int numberCount = 0;
                int operatorCount = 0;

                foreach (string token in tokens)
                {
                    if (double.TryParse(token, out _))
                        numberCount++;
                    else
                        operatorCount++;
                }

                // В корректной постфиксной записи: numbers = operators + 1
                if (numberCount != operatorCount + 1)
                {
                    Console.WriteLine($"  ⚠️ Некорректная структура: {numberCount} чисел, {operatorCount} операторов");
                    return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<string> GenerateStackOperations(int operationCount)
        {
            var operations = new List<string>();
            int stackSize = 0;

            for (int i = 0; i < operationCount; i++)
            {
                // Вероятностное распределение с приоритетом Push
                double rand = _random.NextDouble();
                string operation;

                if (stackSize == 0 || rand < 0.6)
                {
                    operation = $"1,{_random.Next(1, 100)}";
                    stackSize++;
                }
                else if (rand < 0.8 && stackSize > 0)
                {
                    operation = "2";
                    stackSize--;
                }
                else if (rand < 0.9 && stackSize > 0)
                {
                    operation = "3";
                }
                else if (rand < 0.95)
                {
                    operation = "4";
                }
                else
                {
                    operation = "5";
                }

                operations.Add(operation);
            }

            return operations;
        }
    }
}