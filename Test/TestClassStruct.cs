using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    static class TestClassStruct
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

            public static Vector3 operator * (Vector3 vec, float time)
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

        class ProjectileClass
        {
            public CVector3 Position = new CVector3();
            public CVector3 Velocity = new CVector3();
        }

        static unsafe void Main()
        {
            const int count = 10000000;
            ProjectileStruct* projectileUmns = (ProjectileStruct*)Marshal.AllocHGlobal(sizeof(ProjectileStruct) * count);
            ProjectileStruct[] projectileStructs = new ProjectileStruct[count];
            ProjectileClass[] projectileClasses = new ProjectileClass[count];
            for (int i = 0; i < count; ++i)
            {
                projectileClasses[i] = new ProjectileClass();
            }
            Shuffle(projectileUmns, count);
            Shuffle(projectileStructs);
            Shuffle(projectileClasses);

            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < count; ++i)
            {
                UpdateProjectile(projectileUmns + i, 0.5f);
            }
            long umnsTime = sw.ElapsedMilliseconds;

            sw.Reset();
            sw.Start();
            for (int i = 0; i < count; ++i)
            {
                UpdateProjectile(ref projectileStructs[i], 0.5f);
            }
            long structTime = sw.ElapsedMilliseconds;

            sw.Reset();
            sw.Start();
            for (int i = 0; i < count; ++i)
            {
                UpdateProjectile(projectileClasses[i], 0.5f);
            }
            long classTime = sw.ElapsedMilliseconds;

            string report = string.Format(
                "Type,Time\n" +
                "Umns,{0}\n" +
                "Struct,{1}\n" +
                "Class,{2}\n",
                umnsTime,
                structTime,
                classTime
            );
            Console.WriteLine(report);
            Console.ReadLine();
        }

        static unsafe void UpdateProjectile(ProjectileStruct* projectile, float time)
        {
            projectile->Position += projectile->Velocity * time;
        }

        static void UpdateProjectile(ref ProjectileStruct projectile, float time)
        {
            projectile.Position += projectile.Velocity * time;
        }

        static void UpdateProjectile(ProjectileClass projectile, float time)
        {
            projectile.Position += projectile.Velocity * time;
        }

        public static unsafe void Shuffle<T>(T* list, int count) where T : unmanaged
        {
            System.Random random = new System.Random();
            for (int n = count; n > 1;)
            {
                n--;
                int k = random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static void Shuffle<T>(T[] list)
        {
            System.Random random = new System.Random();
            for (int n = list.Length; n > 1;)
            {
                n--;
                int k = random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
