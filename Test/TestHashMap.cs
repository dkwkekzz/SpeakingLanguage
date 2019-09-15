using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    static unsafe class TestHashMap
    {
        struct ecmp : SpeakingLanguage.Library.IumnEqualityComparer<int>
        {
            public bool Equals(int* x, int* y)
            {
                return *x == *y;
            }

            public int GetHashCode(int* obj)
            {
                return *obj;
            }
        }

        struct Vector3
        {
            public int x;
            public int y;
            public int z;

            public Vector3(int x, int y, int z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }

            public override string ToString()
            {
                return $"({x.ToString()}, {y.ToString()}, {z.ToString()})";
            }

            public static Vector3 operator *(Vector3 vec, int time)
            {
                return new Vector3 { x = vec.x * time, y = vec.y * time, z = vec.z * time };
            }

            public static Vector3 operator +(Vector3 left, Vector3 right)
            {
                return new Vector3(right.x + left.x, right.y + left.y, right.z + left.z);
            }
        }

        static void umn()
        {

        }

        static void mng()
        {

        }

        static void Main()
        {
            var timer = new Stopwatch();
            var allocator = new SpeakingLanguage.Library.umnMarshal();

            timer.Start();
            var map = SpeakingLanguage.Library.umnHashMap<ecmp, int, Vector3>.CreateNew(ref allocator, 1000);

            int count = 1000;
            Vector3* arrv = stackalloc Vector3[count];
            for (int i = 0; i != count; i++)
                arrv[i] = new Vector3(i, i + 1, i + 2);

            for (int i = 0; i != count; i++)
            {
                map.Add(& arrv[i].x, &arrv[i]);
            }

            for (int i = 0; i < count; i += 10)
            {
                map.Remove(&i);
            }

            Console.WriteLine($"res1: {map[5]->ToString()}");
            Console.WriteLine($"res1: {map[13]->ToString()}");
            Console.WriteLine($"res1: {map[22]->ToString()}");

            int k = 100;
            if (!map.TryGetValue(&k, out Vector3* v))
            {
                Console.WriteLine($"success remove!");
            }
            else
            {
                Console.WriteLine($"fail to remove...");
            }

            Console.WriteLine($"elapsed: {timer.ElapsedTicks.ToString()}");

            Console.ReadLine();
        }
    }
}
