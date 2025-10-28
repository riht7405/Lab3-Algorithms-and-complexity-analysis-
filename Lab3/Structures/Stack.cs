using Lab3.Models;

namespace Lab3.Structures
{
    public class Stack<T>
    {
        private readonly Models.LinkedList<T> _list;

        public Stack()
        {
            _list = new Models.LinkedList<T>();
        }

        public void Push(T element)
        {
            _list.PushFront(element);
        }

        public T Pop()
        {
            return _list.PopFront();
        }

        public T Top()
        {
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

        // Для тестирования и отладки
        public int Count
        {
            get
            {
                var elements = _list.ToList();
                return elements.Count;
            }
        }
    }
}