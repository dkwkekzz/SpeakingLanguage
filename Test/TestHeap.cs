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
            var narr = SpeakingLanguage.Library.umnNativeArray.AllocateNew<SpeakingLanguage.Library.umnHeap, Vector3>(&heap, 100);
            var vec = new Vector3 { x = 1, y = 2, z = 3 };
            narr[3] = &vec;
            Console.WriteLine((*((Vector3*)narr[4])).ToString());

            var sz = sizeof(Vector3);
            var dynarr = SpeakingLanguage.Library.umnDynamicArray.AllocateNew(&heap, sz * 10);
            dynarr.PushChunk(0);

            Vector3* pVec = stackalloc Vector3[10];
            for (int i = 0; i != 10; i++)
            {
                dynarr.PushBack(pVec, sz);
                pVec++;
            }

            while (!dynarr.IsHead)
            {
                var pv = dynarr.PopBack(sz);
                Console.WriteLine((*((Vector3*)pv)).ToString());
            }

            Console.ReadLine();
        }
    }
}
