using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("============= test space =============");

            SpeakingLanguage.Library.SplayBT<int, int>.Test();

            Console.ReadLine();
        }
    }
}
