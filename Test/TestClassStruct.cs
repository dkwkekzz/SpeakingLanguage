using System;
using System.Collections.Generic;
using System.Linq;
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

        class ProjectileClass
        {
            public Vector3 Position;
            public Vector3 Velocity;
        }

        static void Main()
        {
            const int count = 10000000;
            ProjectileStruct[] projectileStructs = new ProjectileStruct[count];
            ProjectileClass[] projectileClasses = new ProjectileClass[count];
            for (int i = 0; i < count; ++i)
            {
                projectileClasses[i] = new ProjectileClass();
            }
            Shuffle(projectileStructs);
            Shuffle(projectileClasses);

            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
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
                "Struct,{0}\n" +
                "Class,{1}\n",
                structTime,
                classTime
            );
            Console.WriteLine(report);
            Console.ReadLine();
        }

        static void UpdateProjectile(ref ProjectileStruct projectile, float time)
        {
            projectile.Position += projectile.Velocity * time;
        }

        static void UpdateProjectile(ProjectileClass projectile, float time)
        {
            projectile.Position += projectile.Velocity * time;
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
