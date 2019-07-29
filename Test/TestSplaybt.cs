using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class TestSplaybt
    {
        public const int BLOCK = 100;
        public const int TEST_CASE = 1;

        struct Vector3
        {
            public float x;
            public float y;
            public float z;

            public Vector3(int x)
            {
                this.x = x;
                this.y = 0;
                this.z = 0;
            }

            public Vector3(float x, float y, float z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }

            public static Vector3 operator *(Vector3 vec, float time)
            {
                return new Vector3 { x = vec.x * time, y = vec.y * time, z = vec.z * time };
            }

            public static Vector3 operator +(Vector3 left, Vector3 right)
            {
                return new Vector3(right.x + left.x, right.y + left.y, right.z + left.z);
            }

            public static implicit operator Vector3(int a)
            {
                return new Vector3(a);
            }

            public override string ToString()
            {
                return $"({x}, {y}, {z})";
            }
        }

        class VecComparer : IComparer<Vector3>
        {
            public int Compare(Vector3 x, Vector3 y)
            {
                return x.x.CompareTo(y.x);
            }
        }

        static unsafe void Test_umnsbtPerformance()
        {
            var heap = new SpeakingLanguage.Logic.UnmanagedHeap(4000);
            var fac = new SpeakingLanguage.Library.umnFactory<SpeakingLanguage.Library.sbtNode>(heap, 100);
            var tree = new SpeakingLanguage.Library.umnSplayBT<Vector3>(fac, new VecComparer());

            Vector3 v1 = new Vector3(456);
            tree.Add(&v1);
            Vector3 v2 = new Vector3(34222);
            tree.Add(&v2);
            Vector3 v3 = new Vector3(33);
            tree.Add(&v3);
            Vector3 v4 = new Vector3(123566);
            tree.Add(&v4);
            Vector3 v5 = new Vector3(123);
            tree.Add(&v5);
            Console.WriteLine(tree.ToString());
            Vector3 v6 = new Vector3(33);
            tree.Add(&v6);
            Vector3 v7 = new Vector3(33);
            tree.Add(&v7);
            Console.WriteLine(tree.ToString());

            Console.WriteLine("===iterate===");
            var iter = tree.GetEnumerator();
            while (iter.MoveNext())
            {
                Console.WriteLine(((*(iter.Current)).ToString()));
            }

            Console.WriteLine("===backiterate===");
            while (iter.MovePrev())
            {
                Console.WriteLine(((*(iter.Current)).ToString()));
            }
            Console.WriteLine("===randomiterate===");
            if (iter.Advance(4))
                Console.WriteLine(((*(iter.Current)).ToString()));
            else
                Console.WriteLine("fail to advance...");

            Vector3 v8 = new Vector3(992);
            tree.Add(&v8);
            Console.WriteLine("===BidirectEnumerator===");
            var bidIter = tree.GetEnumerator(33);
            while (bidIter.MoveNext())
            {
                Console.WriteLine(((*(bidIter.Current)).ToString()));
            }
            Console.WriteLine("=============");

            if (tree.ContainsKey(123566))
            {
                Console.WriteLine("found: 123566");
            }
            Console.WriteLine(tree.ToString());
            
            Console.WriteLine("remove: 34222");
            tree.Remove(34222);
            Console.WriteLine(tree.ToString());

            Console.WriteLine("remove: 33");
            tree.Remove(33);
            Console.WriteLine(tree.ToString());

            SpeakingLanguage.Library.sbtNode** p = stackalloc SpeakingLanguage.Library.sbtNode*[10];
            var fiter = tree.GetFastEnumerator(p);
            while (fiter.MoveNext())
            {
                Console.WriteLine(((*(fiter.Current)).ToString()));
            }

        }

        static void Test_SbtPerformance(Random rd)
        {
            var sbt = new SpeakingLanguage.Library.SplayBT<int, int>();
            for (int ti = 0; ti != TEST_CASE; ti++)
            {
                for (int i = 0; i != BLOCK; i++)
                    sbt.Add(i, rd.Next());
                //for (int i = 0; i != BLOCK / 10; i++)
                //    sbt.Remove(i);
                //for (int i = BLOCK; i != BLOCK * 2; i++)
                //    sbt.Add(i, rd.Next());
                //for (int i = BLOCK / 10; i != BLOCK / 5; i++)
                //    sbt.Remove(i);
                //for (int i = 0; i != BLOCK; i++)
                //    sbt.TryGetValue(i, out int v);
                sbt.Clear();
            }
        }

        static void Test_DicPerformance(Random rd)
        {
            var dic = new SortedDictionary<int, int>();
            for (int ti = 0; ti != TEST_CASE; ti++)
            {
                for (int i = 0; i != BLOCK; i++)
                    dic.Add(i, rd.Next());
                //for (int i = 0; i != BLOCK / 10; i++)
                //    dic.Remove(i);
                //for (int i = BLOCK; i != BLOCK * 2; i++)
                //    dic.Add(i, rd.Next());
                //for (int i = BLOCK / 10; i != BLOCK / 5; i++)
                //    dic.Remove(i);
                //for (int i = 0; i != BLOCK; i++)
                //    dic.TryGetValue(i, out int v);
                dic.Clear();
            }
        }

        static void func1()
        {
            var _head = Marshal.AllocHGlobal(2222);
            Console.WriteLine($"allocated in fund1: {_head.ToInt64().ToString()}");
        }

        static void func2()
        {
            var _head = Marshal.AllocHGlobal(3333);
            Console.WriteLine($"allocated in fund2: {_head.ToInt64().ToString()}");
        }

        static void func3()
        {
            var _head = Marshal.AllocHGlobal(4444);
            Console.WriteLine($"allocated in fund3: {_head.ToInt64().ToString()}");
        }

        static void Main(string[] args)
        {
            Console.WriteLine("============= test space =============");

            var rd = new Random(); 
            var sw = new Stopwatch();

            sw.Start();

            Test_umnsbtPerformance();
            //func1();
            //func2();
            //func3();
            //Test_umnsbtPerformance();
            //SpeakingLanguage.Library.SplayBT<int, int>.Test();
            //Test_SbtPerformance(rd);
            //Test_DicPerformance(rd);

            sw.Stop();
            Console.WriteLine($"elapsed: {sw.ElapsedMilliseconds.ToString()}");

            //SpeakingLanguage.Library.SplayBT<int, int>.Test();

            Console.ReadLine();
        }
    }
}
