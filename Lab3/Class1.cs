using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

// Узел связного списка
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

// Связный список
public class LinkedList<T>
{
    private Node<T> head;

    public void PushFront(T data)
    {
        Node<T> newNode = new Node<T>(data);
        newNode.Next = head;
        head = newNode;
    }

    public T PopFront()
    {
        if (head == null)
            throw new InvalidOperationException("List is empty");

        T data = head.Data;
        head = head.Next;
        return data;
    }

    public T Front()
    {
        if (head == null)
            throw new InvalidOperationException("List is empty");

        return head.Data;
    }

    public bool IsEmpty()
    {
        return head == null;
    }

    public void Print()
    {
        Node<T> current = head;
        List<string> elements = new List<string>();

        while (current != null)
        {
            elements.Add(current.Data.ToString());
            current = current.Next;
        }

        Console.WriteLine(string.Join(" ", elements));
    }
}

// Стек на основе связного списка
public class Stack<T>
{
    private LinkedList<T> list;

    public Stack()
    {
        list = new LinkedList<T>();
    }

    public void Push(T elem)
    {
        list.PushFront(elem);
    }

    public T Pop()
    {
        return list.PopFront();
    }

    public T Top()
    {
        return list.Front();
    }

    public bool IsEmpty()
    {
        return list.IsEmpty();
    }

    public void Print()
    {
        list.Print();
    }
}