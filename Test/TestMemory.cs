using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public interface IComponent
    {
    }

    class Health : IComponent
    {
        public int value;
        public int value1;
        public int value2;
    }

    class Mana : IComponent
    {
        public int value;
        public int value1;
        public int value2;
    }

    class Power : IComponent
    {
        public int value;
        public int value1;
        public int value2;
        public int value3;
        public int value4;
    }

    class Property : IComponent
    {
        public int value;
        public int value1;
        public int value2;
        public int value3;
        public int value4;
        public int value5;
        public int value6;
        public int value7;
        public int value8;
    }

    class Archer
    {
        public int id;
        public Dictionary<Type, IComponent> lstCom = new Dictionary<Type, IComponent>(4);
    }

    class Heap
    {
        public Dictionary<int, Archer> list = new Dictionary<int, Archer>(100000);
        public Queue<Archer> factory = new Queue<Archer>();
        public Queue<Property> pfactory = new Queue<Property>();
    }

    class HeapLazy
    {
        public List<Archer> list = new List<Archer>(100000);
        public Queue<Archer> factory = new Queue<Archer>();
    }

    class TestMemory
    {
        static Heap heap = new Heap();

        static void executeHeap()
        {
            var timer = new Stopwatch();
            timer.Start();

            int idgenerator = 0;
            var ran = new Random();
            for (int i = 0; i != 100; i++)
            {
                for (int j = 0; j != 1000; j++)
                {
                    var a = new Archer();
                    a.id = idgenerator++;
                    heap.list.Add(a.id, a);

                    var val = ran.Next();
                    var comId = val % 4;
                    if (comId == 0)
                    {
                        a.lstCom.Add(typeof(Health), new )
                    }
                    else if (comId == 1)
                    {

                    }
                }

                for (int j = 0; j != 1000; j++)
                {
                    var val = ran.Next();
                    var comId = val % 4;
                    if (comId == 0)
                    {

                    }
                    else if (comId == 1)
                    {

                    }
                }

                for (int j = 0; j != 1000; j++)
                {
                    var val = ran.Next();
                    var id = val % idgenerator;
                    heap.list.Remove(id);
                }
            }

            timer.Stop();
            Console.WriteLine($"elapsed: {timer.ElapsedMilliseconds.ToString()}");

        }

        static void Main()
        {
            Console.WriteLine("test end.");
            Console.ReadLine();
        }

    }
}
