using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestAnna
{
    class Node
    {
        public string Data { get; set; }
        public Node Next { get; set; }
    }

    class MyList
    {
        private int _count;
        private Node _head;
        private Node _tail;

        public void Add(Node node)
        {
            if (_head == null)
            {
                _head = node;
                _tail = node;
            }
            else
            {
               _tail.Next = node;
                _tail = node;
            }
            _count++;
        }

        public void Print()
        {
            Node current = _head;
            while (current != null)
            {
                Console.WriteLine(current.Data);
                current = current.Next;
            }
        }
    }
}
