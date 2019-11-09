using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    internal struct StateSync : IComparable<StateSync>
    {
        public int sync01;
        public int sync02;
        public int sync03;
        public int sync04;

        public int CompareTo(StateSync other)
        {
            var ret = sync04.CompareTo(other.sync04);
            if (ret != 0)
                return ret;

            ret = sync03.CompareTo(other.sync03);
            if (ret != 0)
                return ret;

            ret = sync02.CompareTo(other.sync02);
            if (ret != 0)
                return ret;

            ret = sync01.CompareTo(other.sync01);
            return ret;
        }

        public bool Contains(ref StateSync other)
        {
            return (other.sync01 & sync01) == sync01
                && (other.sync02 & sync02) == sync02
                && (other.sync03 & sync03) == sync03
                && (other.sync04 & sync04) == sync04;
        }
        
        public void Insert(int idx)
        {
            if (idx < 32)
            {
                sync01 |= (1 << idx);
            }
            else if (idx < 64)
            {
                sync02 |= (1 << (idx - 32));
            }
            else if (idx < 96)
            {
                sync03 |= (1 << (idx - 64));
            }
            else if (idx < 128)
            {
                sync04 |= (1 << (idx - 96));
            }
        }

        public void Remove(int idx)
        {
            if (idx < 32)
            {
                sync01 &= ~(1 << idx);
            }
            else if (idx < 64)
            {
                sync02 &= ~(1 << (idx - 32));
            }
            else if (idx < 96)
            {
                sync03 &= ~(1 << (idx - 64));
            }
            else if (idx < 128)
            {
                sync04 &= ~(1 << (idx - 96));
            }
        }

        public void Clear()
        {
            sync01 = 0;
            sync02 = 0;
            sync03 = 0;
            sync04 = 0;
        }

        public static StateSync Create<TEnumerator>(TEnumerator iter)
            where TEnumerator : IEnumerator<int>
        {
            return Create(ref iter);
        }

        public static StateSync Create<TEnumerator>(ref TEnumerator iter)
            where TEnumerator : IEnumerator<int>
        {
            var key = new StateSync();

            while (true)
            {
                if (!iter.MoveNext())
                    return key;

                var idx = iter.Current;
                if (idx > 31)
                    break;

                var flag = 1 << idx;
                key.sync01 |= flag;
            }

            while (true)
            {
                if (!iter.MoveNext())
                    return key;

                var idx = iter.Current - 32;
                if (idx > 31)
                    break;

                var flag = 1 << idx;
                key.sync02 |= flag;
            }

            while (true)
            {
                if (!iter.MoveNext())
                    return key;

                var idx = iter.Current - 64;
                if (idx > 31)
                    break;

                var flag = 1 << idx;
                key.sync03 |= flag;
            }

            while (true)
            {
                if (!iter.MoveNext())
                    return key;

                var idx = iter.Current - 96;
                if (idx > 31)
                    Library.ThrowHelper.ThrowStateOverflow("in StateFlag::Create");

                var flag = 1 << idx;
                key.sync04 |= flag;
            }
        }
    }

}
