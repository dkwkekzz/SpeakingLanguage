﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    struct Archer
    {
        public int power;
        public int health;
        public int skill;
        public int mana;
    }

    struct Monster
    {
        public int power;
        public int health;
    }
    
    class MyWorld
    {
        [InteractionAttribute(typeof(Archer))]
        public unsafe static void SimpleInteract(Archer* a, Observer ob)
        {
            Console.WriteLine($"battle! {a->power.ToString()} vs. {ob.power.ToString()}");
        }
    }

    public class InteractionAttribute : Attribute
    {
        public Type SrcType { get; }

        public InteractionAttribute(Type src)
        {
            SrcType = src;
        }
    }

    class Observer
    {
        public int power;
        public int health;
    }
    
    unsafe interface IAction
    {
        void Take(MethodInfo mth);
        void Invoke(void* a, Observer ob);
    }
    
    unsafe class InteractAction<T> : IAction where T : unmanaged
    {
        private delegate void SimpleDel(T* pa, Observer ob);
        private SimpleDel _del;
        
        public void Take(MethodInfo mth)
        {
            _del = (SimpleDel)mth.CreateDelegate(typeof(SimpleDel));
        }

        public void Invoke(void* a, Observer ob)
        {
            _del((T*)a, ob);
        }
    }

    unsafe class SimpleAction : IAction
    {
        private delegate void SimpleDel(void* pa, Observer ob);
        private SimpleDel _del;

        public void Take(MethodInfo mth)
        {
            _del = (SimpleDel)mth.CreateDelegate(typeof(SimpleDel));
        }

        public void Invoke(void* a, Observer ob)
        {
            _del(a, ob);
        }
    }


    static unsafe class TestAttribute
    {
        private readonly static Type TInteractionAttribute = typeof(InteractionAttribute);

        private readonly static Dictionary<Type, IAction> _dicActions = new Dictionary<Type, IAction>();

        public static void Collect()
        {
            var types = SpeakingLanguage.Library.AssemblyHelper.CollectType(null);
            foreach (var type in types)
            {
                var mths = type.GetMethods();
                for (int i = 0; i != mths.Length; i++)
                {
                    var mth = mths[i];

                    var interAttr = Attribute.GetCustomAttribute(mth, TInteractionAttribute) as InteractionAttribute;
                    if (interAttr != null)
                    {
                        var paramInfos = mth.GetParameters();
                        var paramTypes = new Type[paramInfos.Length];
                        for (int j = 0; j != paramInfos.Length; j++)
                            paramTypes[j] = paramInfos[j].ParameterType;

                        var action = new SimpleAction();
                        action.Take(mth);
                        _dicActions.Add(paramTypes[0], action);

                        //Type actionType = typeof(InteractAction<>);
                        //Type[] typeArgs = { interAttr.SrcType };
                        //Type constructed = actionType.MakeGenericType(typeArgs);
                        //
                        //var action = Activator.CreateInstance(constructed) as IAction;
                        //action.Take(mth);
                        //
                        //_dicActions.Add(interAttr.SrcType, action);


                    }
                }
            }
        }

        static Type at = typeof(Archer*);

        static void Execute()
        {
            var a = new Archer();
            var ob = new Observer();
            var pa = &a;
            _dicActions[at].Invoke(pa, ob);

        }

        static void Main()
        {
            Collect();
            Execute();

            Console.ReadLine();
        }
    }
}