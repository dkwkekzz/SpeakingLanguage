using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Archer
    {
        public int Health { get; set; }
        public int Power { get; }

        public void Attack()
        {
            Console.WriteLine("Attack");
        }
    }

    class TestReflection
    {
        static void Main()
        {
            var mths = typeof(Archer).GetMethods();
            foreach (MethodInfo mth in mths)
            {
                Console.WriteLine($"mth: {mth.Name}");
            }
        }
    }
}
