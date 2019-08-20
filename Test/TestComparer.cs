using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class TestComparer
    {
        class Comparer : IEqualityComparer<int>
        {
            public bool Equals(int x, int y)
            {
                Console.WriteLine($"[Equals] x:{x.ToString()} / y:{y.ToString()}");
                return x == y;
            }

            public int GetHashCode(int obj)
            {
                return obj;
            }
        }


        static void Main()
        {
            int actKey = 99;
            int sync = 101;
            Console.WriteLine($"{actKey.CompareTo(sync).ToString()}");
            
            Console.ReadLine();
        }
    }
}
