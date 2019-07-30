using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    unsafe class TestHeap
    {
        struct Vector3
        {
            public float x;
            public float y;
            public float z;

            public override string ToString()
            {
                return $"({x.ToString()}, {y.ToString()}, {z.ToString()})";
            }
        }

        static void Main()
        {
            var marshal = new SpeakingLanguage.Library.umnMarshal();
            var heap = new SpeakingLanguage.Library.umnHeap(marshal.Alloc(4000));
            var narr = SpeakingLanguage.Library.umnNativeArray.Allocate_umnNativeArray<SpeakingLanguage.Library.umnHeap, Vector3>(&heap, 100);
            var vec = new Vector3 { x = 1, y = 2, z = 3 };
            narr[3] = &vec;
            Console.WriteLine((*((Vector3*)narr[4])).ToString());

            Console.ReadLine();
        }
    }
}
