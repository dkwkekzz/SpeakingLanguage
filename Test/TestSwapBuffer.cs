using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    unsafe class TestSwapBuffer
    {
        struct Vector3
        {
            public float x;
            public float y;
            public float z;

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
        }

        struct ProjectileStruct
        {
            public Vector3 Position;
            public Vector3 Velocity;
        }

        class ProjectileClass
        {
            public Vector3 Position = new Vector3();
            public Vector3 Velocity = new Vector3();
        }

        class CVector3
        {
            public float x;
            public float y;
            public float z;

            public CVector3()
            {
            }

            public CVector3(float x, float y, float z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }

            public static CVector3 operator *(CVector3 vec, float time)
            {
                return new CVector3(vec.x * time, vec.y * time, vec.z * time);
            }

            public static CVector3 operator +(CVector3 left, CVector3 right)
            {
                return new CVector3(right.x + left.x, right.y + left.y, right.z + left.z);
            }
        }

        unsafe class SwapBuffer : IDisposable
        {
            private SpeakingLanguage.Library.umnArray<IntPtr> _buffers;
            private int _bufIndex;

            public SwapBuffer(int count)
            {
                var allocator = new SpeakingLanguage.Library.umnMarshal();
                _buffers = SpeakingLanguage.Library.umnArray<IntPtr>.CreateNew(ref allocator, 2);
                *(_buffers[0]) = Marshal.AllocHGlobal(sizeof(ProjectileStruct) * count);
                *(_buffers[1]) = Marshal.AllocHGlobal(sizeof(ProjectileStruct) * count);
                _bufIndex = 0;
            }

            public void ExecuteNormal(int count)
            {
                var sz = sizeof(ProjectileStruct);
                var buffer = *(_buffers[_bufIndex]);
                for (int i = 0; i != count; i++)
                {
                    var st1 = (ProjectileStruct*)(buffer + i * sz);
                    UpdateProjectile(st1, 0.5f);
                }
            }

            public void ExecuteNormal2x(int count)
            {
                var sz = sizeof(ProjectileStruct);
                var buffer = *(_buffers[_bufIndex]);
                for (int i = 0; i != count; i += 2)
                {
                    var st1 = (ProjectileStruct*)(buffer + i * sz);
                    UpdateProjectile(st1, 0.5f);
                }
            }

            public void ExecuteSwap(int count)
            {
                var sz = sizeof(ProjectileStruct);
                var buffer = *(_buffers[_bufIndex]);
                var backBuffer = *(_buffers[(_bufIndex + 1) % 2]);
                for (int i = 0; i != count; i++)
                {
                    var st1 = (ProjectileStruct*)(buffer + i * sz);
                    UpdateProjectile(st1, 0.5f);
                    *(ProjectileStruct*)(backBuffer + i * sz) = *st1;
                }

                _bufIndex = (_bufIndex + 1) % 2;
            }

            public void ExecuteCopy(int count)
            {
                var sz = sizeof(ProjectileStruct);
                var buffer = *(_buffers[_bufIndex]);
                var backBuffer = *(_buffers[(_bufIndex + 1) % 2]);
                for (int i = 0; i != count; i++)
                {
                    var st1 = (ProjectileStruct*)(buffer + i * sz);
                    UpdateProjectile(st1, 0.5f);
                }

                for (int i = 0; i != count; i++)
                {
                    *(ProjectileStruct*)(buffer + i * sz) = *(ProjectileStruct*)(backBuffer + i * sz);
                }
            }

            public void Dispose()
            {
                foreach (IntPtr ptr in _buffers)
                    Marshal.FreeHGlobal(ptr);
            }

            static unsafe void UpdateProjectile(ProjectileStruct* projectile, float time)
            {
                projectile->Position += projectile->Velocity * time;
            }
        }


        static void Main()
        {
            int count = 10000;
            using (var buffer = new SwapBuffer(count))
            {
                var timer = new Stopwatch();
                int refCount = 0;
                var elapsed = 0L;
                while (++refCount > 0)
                {
                    timer.Restart();
                    //buffer.ExecuteNormal(count);
                    buffer.ExecuteNormal2x(count);
                    //buffer.ExecuteSwap(count);
                    //buffer.ExecuteCopy(count);
                    timer.Stop();

                    elapsed += timer.ElapsedTicks;
                    Console.WriteLine($"elapsed: {(elapsed / refCount).ToString()}");
                }
                
                Console.ReadLine();
            }
        }
    }
}
