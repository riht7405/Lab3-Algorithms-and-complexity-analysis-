using Lab3.Models;
using System;
using System.Collections.Generic;

namespace Lab3.Models
{
    public class LinkedList<T>
    {
        private Node<T>? _head;  // Добавлен nullable

        public void PushFront(T data)
        {
            Node<T> newNode = new Node<T>(data);
            newNode.Next = _head;
            _head = newNode;
        }

        public T PopFront()
        {
            if (_head == null)
                throw new InvalidOperationException("List is empty");

            T data = _head.Data;
            _head = _head.Next;
            return data;
        }

        public T Front()
        {
            if (_head == null)
                throw new InvalidOperationException("List is empty");

            return _head.Data;
        }

        public bool IsEmpty()
        {
            return _head == null;
        }

        public void Print()
        {
            Node<T>? current = _head;
            List<string> elements = new List<string>();

            while (current != null)
            {
                elements.Add(current.Data?.ToString() ?? "null");
                current = current.Next;
            }

            Console.WriteLine(string.Join(" ", elements));
        }

        // Для отладки - получить все элементы как список
        public List<T> ToList()
        {
            List<T> result = new List<T>();
            Node<T>? current = _head;

            while (current != null)
            {
                result.Add(current.Data);
                current = current.Next;
            }

            return result;
        }
    }
}