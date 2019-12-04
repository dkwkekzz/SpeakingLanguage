using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SpeakingLanguage.Logic
{
    public struct slInteraction
    {
        public int subject;
        public int target;

        public slInteraction(int s, int t)
        {
            subject = s;
            target = t;
        }
    }

    public struct slControl
    {
        public int handle;
        public bool press;
        public int key;

        public slControl(int h, bool p, int k)
        {
            handle = h;
            press = p;
            key = k;
        }
    }

    public abstract class Archetype
    {
        public struct PropertySet
        {
            private readonly Dictionary<Type, int> _ofsDic;

            public int TotalSize { get; }

            public PropertySet(params Type[] types)
            {
                this.TotalSize = 0;

                var typeLen = types.Length;
                _ofsDic = new Dictionary<Type, int>(typeLen);

                for (int i = 0; i != typeLen; i++)
                {
                    var type = types[i];
                    _ofsDic.Add(type, this.TotalSize);

                    var typeSize = Marshal.SizeOf(type);
                    this.TotalSize += typeSize;
                }
            }

            public Dictionary<Type, int>.KeyCollection.Enumerator GetEnumerator()
            {
                return _ofsDic.Keys.GetEnumerator();
            }

            public int this[Type type] => _ofsDic[type];
        }

        private static int _indexGenerator;
        private PropertySet _property;

        public int Index { get; }
        public PropertySet Properties => _property;
        public int Capacity => _property.TotalSize;

        public Archetype()
        {
            OnConstruct(out _property);
            this.Index = _indexGenerator++;
        }

        protected abstract void OnConstruct(out PropertySet property);
    }

    public abstract class ArchetypeHolder<T> : Archetype where T : Archetype
    {
        public static T Value { get; private set; }

        public ArchetypeHolder()
        {
            ArchetypeHolder<T>.Value = this as T;
        }
    }

    public class Law
    {
        protected struct PropertyPair
        {
            public PropertyTable.Row subject;
            public PropertyTable.Row target;
        }

        private readonly List<PropertyPair> _pairs = new List<PropertyPair>();

        public void Insert(PropertyTable.Row subjectRow, PropertyTable.Row targetRow)
        {
            _pairs.Add(new PropertyPair { subject = subjectRow, target = targetRow });
        }

        public void Reset()
        {
            _pairs.Clear();
        }

        public void Execute()
        {
            var iter = _pairs.GetEnumerator();
            while (iter.MoveNext())
            {
                OnExecute(iter.Current);
            }
        }

        protected virtual void OnExecute(PropertyPair pair) { }
    }

    public sealed class DamageLaw : Law
    {
        protected override void OnExecute(PropertyPair pair)
        {
        }
    }

    internal class Controller
    {
        public bool changed;
        public int left;
        public int right;
        public int up;
        public int down;
        public int fire_a;
        public int fire_b;
        public int space;

        public unsafe void CopyTo(Property.Controller* dest)
        {
            dest->left = left;
            dest->right = right;
            dest->up = up;
            dest->down = down;
            dest->fire_a = fire_a;
            dest->fire_b = fire_b;
            dest->space = space;
            changed = false;
        }
    }

}
