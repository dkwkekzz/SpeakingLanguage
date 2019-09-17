using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    using MethodInvoker = Action;

    class TestEvent
    {
        public event EventHandler MyEvent;

        public static IEnumerable<int> GetCounter()
        {
            for (int count = 0; count < 10; count++)
            {
                yield return count;
            }
        }

        public static IEnumerator<int> GetCounter2()
        {
            for (int count = 0; count < 10; count++)
            {
                yield return count;
            }
        }

        static void Main()
        {
            var iter = GetCounter().GetEnumerator();
            Console.WriteLine(iter.Current);
            Console.WriteLine(GetCounter2().Current);


            Console.ReadLine();
        }

        void DoNothing(object sender, EventArgs e)
        {
            Console.WriteLine("DoNothing");
        }
    }
}
