using System;
using System.Collections.Generic;

namespace Lab3.Structures
{
    public class Stack<T>
    {
        private readonly LinkedList<T> _list;

        public Stack()
        {
            _list = new LinkedList<T>();
        }

        public void Push(T element)
        {
            _list.PushFront(element);
        }

        public T Pop()
        {
            if (IsEmpty())
                throw new InvalidOperationException("Стек пуст");
            return _list.PopFront();
        }

        public T Top()
        {
            if (IsEmpty())
                throw new InvalidOperationException("Стек пуст");
            return _list.Front();
        }

        public bool IsEmpty()
        {
            return _list.IsEmpty();
        }

        public void Print()
        {
            _list.Print();
        }

        public int Count => _list.Count;
    }
}