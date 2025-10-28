namespace Lab3.Models
{
    public class Node<T>
    {
        public T Data { get; set; }
        public Node<T>? Next { get; set; }  // Добавлен nullable

        public Node(T data)
        {
            Data = data;
            Next = null;
        }
    }
}