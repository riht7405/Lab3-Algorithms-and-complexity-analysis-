using System;
using System.Collections.Generic;

namespace Lab3.Structures
{
    public class LinkedList<T>
    {
        private Node<T> _head;
        private int _count;

        public void PushFront(T data)
        {
            Node<T> newNode = new Node<T>(data);
            newNode.Next = _head;
            _head = newNode;
            _count++;
        }

        public T PopFront()
        {
            if (_head == null)
                throw new InvalidOperationException("Список пуст");

            T data = _head.Data;
            _head = _head.Next;
            _count--;
            return data;
        }

        public T Front()
        {
            if (_head == null)
                throw new InvalidOperationException("Список пуст");
            return _head.Data;
        }

        public bool IsEmpty()
        {
            return _head == null;
        }

        public void Print()
        {
            Node<T> current = _head;
            List<string> elements = new List<string>();

            while (current != null)
            {
                elements.Add(current.Data?.ToString() ?? "null");
                current = current.Next;
            }

            Console.WriteLine("Стек: " + string.Join(" → ", elements));
        }

        public int Count => _count;
    }

    public class Node<T>
    {
        public T Data { get; set; }
        public Node<T> Next { get; set; }

        public Node(T data)
        {
            Data = data;
            Next = null;
        }
    }
}