using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SpeakingLanguage.Library
{
    public static class AssemblyHelper
    {
        public static List<T> Collect<T>()
        {
            var collected = new List<T>();
            var types = AppDomain.CurrentDomain.GetAssemblies()
                       .SelectMany(asm => asm.GetTypes())
                       .Where(t => t.IsClass && t.IsSubclassOf(typeof(T)));
            foreach (var type in types)
            {
                var inst = (T)Activator.CreateInstance(type);
                if (collected.Contains(inst))
                    throw new ArgumentException($"dulicate type: {type.FullName}");

                collected.Add(inst);
            }

            return collected;
        }
        
        public static IEnumerable<Type> CollectType(Predicate<Type> predicator)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                       .SelectMany(asm => GetTypesSafely(asm))
                       .Where(t => predicator?.Invoke(t) ?? true);
            return types;
        }

        public static IEnumerable<Type> GetTypesSafely(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                return ex.Types.Where(x => x != null);
            }
        }

        public static void FindAssemblyAndLoad(string prefix, string suffix)
        {
            var directory = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var files = System.IO.Directory.GetFiles(directory);
            foreach (var file in files)
            {
                if (file.Contains(prefix) && file.Contains(suffix))
                {
                    Assembly.LoadFrom(file);
                }
            }
        }

        public static Delegate CreateDelegate(MethodInfo method)
        {
            var parameters = method.GetParameters()
                       .Select(p => Expression.Parameter(p.ParameterType, p.Name))
                        .ToArray();

            var call = Expression.Call(method, parameters);
            return Expression.Lambda(call, parameters).Compile();
        }

        public static Delegate CreateDelegate(object instance, MethodInfo method)
        {
            var parameters = method.GetParameters()
                       .Select(p => Expression.Parameter(p.ParameterType, p.Name))
                        .ToArray();

            var call = Expression.Call(Expression.Constant(instance), method, parameters);
            return Expression.Lambda(call, parameters).Compile();
        }
    }
}
