using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeakingLanguage.Logic
{
    public static class PropertyHelper
    {
        public static T GetReadonly<T>(int handle) where T : unmanaged
        {
            var pt = Locator.Instance.PropertyTable;
            if (!pt.TryGetRow(handle, out PropertyTable.Row row))
                return default;

            return GetReadonly<T>(row);
        }

        public static T GetReadonly<T>(PropertyTable.Row row) where T : unmanaged
        {
            var ofs = row.type.Properties[typeof(T)];
            unsafe
            {
                var ret = *(T*)(row.head + ofs);
                return ret;
            }
        }

        public static unsafe T* Get<T>(PropertyTable.Row row) where T : unmanaged
        {
            var ofs = row.type.Properties[typeof(T)];
            return (T*)(row.head + ofs);
        }

        public static unsafe T* Get<T>(ref PropertyTable.Row row) where T : unmanaged
        {
            var ofs = row.type.Properties[typeof(T)];
            return (T*)(row.head + ofs);
        }
    }

    public static class Construct
    {
        private class LawBundle
        {
            private readonly List<Law> _forSubjects = new List<Law>();
            private readonly List<Law> _forTargets = new List<Law>();

            public void AddSubject(Law law)
            {
                _forSubjects.Add(law);
            }

            public void AddTarget(Law law)
            {
                _forTargets.Add(law);
            }

            public void CopyToSubjectList(HashSet<Law> outList)
            {
                var iter = _forSubjects.GetEnumerator();
                while (iter.MoveNext())
                {
                    outList.Add(iter.Current);
                }
            }

            public void CopyToTargetList(HashSet<Law> outList)
            {
                var iter = _forTargets.GetEnumerator();
                while (iter.MoveNext())
                {
                    outList.Add(iter.Current);
                }
            }
        }

        public static void Execute()
        {
            var lawCol = Locator.Instance.LawCollection;
            var typeCol = Locator.Instance.ArchetypeCollection;
            var lawMed = Locator.Instance.LawMediator;

            var prop2LawBundleDic = new Dictionary<Type, LawBundle>();

            var lawIter = lawCol.GetEnumerator();
            while (lawIter.MoveNext())
            {
                var law = lawIter.Current;
                var lawType = law.GetType();
                var subjectAttr = Attribute.GetCustomAttribute(typeof(SubjectConditionAttribute), lawType) as SubjectConditionAttribute;
                var targetAttr = Attribute.GetCustomAttribute(typeof(TargetConditionAttribute), lawType) as TargetConditionAttribute;
                if (null == subjectAttr) continue;

                var subjectTypes = subjectAttr.ParamTypes;
                var subjectTypesLen = subjectTypes.Length;
                for (int i = 0; i != subjectTypesLen; i++)
                {
                    var subjectType = subjectTypes[i];
                    if (!prop2LawBundleDic.TryGetValue(subjectType, out LawBundle lawBundle))
                    {
                        lawBundle = new LawBundle();
                        prop2LawBundleDic.Add(subjectType, lawBundle);
                    }

                    lawBundle.AddSubject(law);
                }

                if (null != targetAttr)
                {
                    var targetTypes = targetAttr.ParamTypes;
                    var targetTypesLen = targetTypes.Length;
                    for (int i = 0; i != targetTypesLen; i++)
                    {
                        var targetType = subjectTypes[i];
                        if (!prop2LawBundleDic.TryGetValue(targetType, out LawBundle lawBundle))
                        {
                            lawBundle = new LawBundle();
                            prop2LawBundleDic.Add(targetType, lawBundle);
                        }

                        lawBundle.AddTarget(law);
                    }
                }
            }

            var subject2LawDic = new Dictionary<Archetype, HashSet<Law>>();
            var subjectAtIter = typeCol.GetEnumerator();
            while (subjectAtIter.MoveNext())
            {
                var subjectAt = subjectAtIter.Current;
                var subjectLawList = new HashSet<Law>();

                var subjectPropIter = subjectAt.Properties.GetEnumerator();
                while (subjectPropIter.MoveNext())
                {
                    var propType = subjectPropIter.Current;
                    if (!prop2LawBundleDic.TryGetValue(propType, out LawBundle lawBundle))
                        continue;

                    lawBundle.CopyToSubjectList(subjectLawList);
                }

                subject2LawDic.Add(subjectAt, subjectLawList);
            }

            var target2LawDic = new Dictionary<Archetype, HashSet<Law>>();
            var targetAtIter = typeCol.GetEnumerator();
            while (targetAtIter.MoveNext())
            {
                var targetAt = targetAtIter.Current;
                var targetLawList = new HashSet<Law>();

                var targetPropIter = targetAt.Properties.GetEnumerator();
                while (targetPropIter.MoveNext())
                {
                    var propType = targetPropIter.Current;
                    if (!prop2LawBundleDic.TryGetValue(propType, out LawBundle lawBundle))
                        continue;

                    lawBundle.CopyToTargetList(targetLawList);
                }

                target2LawDic.Add(targetAt, targetLawList);
            }

            // cross match
            subjectAtIter = typeCol.GetEnumerator();
            while (subjectAtIter.MoveNext())
            {
                var subjectAt = subjectAtIter.Current;
                var subjectSet = subject2LawDic[subjectAt];
                var subjectLawIter = subjectSet.GetEnumerator();
                while (subjectLawIter.MoveNext())
                {
                    var subjectLaw = subjectLawIter.Current;

                    targetAtIter = typeCol.GetEnumerator();
                    while (targetAtIter.MoveNext())
                    {
                        var targetAt = targetAtIter.Current;
                        var targetSet = target2LawDic[targetAt];
                        if (!targetSet.Contains(subjectLaw))
                            continue;

                        lawMed.Insert(subjectAt, targetAt, subjectLaw);
                    }
                }
            }
        }
    }

    public static class Factory
    {
        public static PropertyTable.Row CreateNew(int handle, Archetype type)
        {
            var pt = Locator.Instance.PropertyTable;
            pt.Insert(handle, type, out PropertyTable.Row outRow);
            return outRow;
        }
    }

    public static class Synchronization
    {
        public static int Deserialize(ref Library.Reader reader)
        {
            reader.ReadInt(out int handle);
            reader.ReadInt(out int typeIdx);

            var typeCol = Locator.Instance.ArchetypeCollection;
            if (!typeCol.TryGetArchetype(typeIdx, out Archetype type))
                return -1;

            var pt = Locator.Instance.PropertyTable;
            pt.Insert(handle, type, out PropertyTable.Row outRow);
            unsafe
            {
                reader.ReadMemory((void*)outRow.head, type.Capacity);
            }
            return handle;
        }

        public static bool Serialize(int handle, ref Library.Writer writer)
        {
            var pt = Locator.Instance.PropertyTable;
            if (!pt.TryGetRow(handle, out PropertyTable.Row row))
                return false;

            var type = row.type;
            var length = sizeof(int) * 2 + type.Capacity;
            if (writer.Remained <= length)
                writer.Expand();

            writer.WriteInt(handle);
            writer.WriteInt(type.Index);
            unsafe
            {
                writer.WriteMemory((void*)row.head, type.Capacity);
            }
            return true;
        }

        public static byte[] Serialize(int handle)
        {
            var pt = Locator.Instance.PropertyTable;
            if (!pt.TryGetRow(handle, out PropertyTable.Row row))
                return null;

            var type = row.type;
            var writer = new Library.Writer(sizeof(int) * 2 + type.Capacity);
            writer.WriteInt(handle);
            writer.WriteInt(type.Index);
            unsafe
            {
                writer.WriteMemory((void*)row.head, type.Capacity);
            }
            return writer.GetResizedBuffer();
        }
    }

    public static class Receiver
    {
        public static void Interaction(int lhs, int rhs)
        {
            var graph = Locator.Instance.Graph;
            graph.Insert(new slInteraction(lhs, rhs));
        }

        public static void Control(int handle, bool press, int key)
        {
            var tower = Locator.Instance.ControlTower;
            tower.Insert(new slControl(handle, press, key));
        }
    }

    public static class Inject
    {
        public static void Execute()
        {
            var graph = Locator.Instance.Graph;
            var tower = Locator.Instance.ControlTower;
            var pt = Locator.Instance.PropertyTable;
            var lawMed = Locator.Instance.LawMediator;

            var iter = graph.GetEnumerator();
            while (iter.MoveNext())
            {
                var itr = iter.Current;
                if (!pt.TryGetRow(iter.Current.subject, out PropertyTable.Row subjectRow))
                    continue;
                if (!pt.TryGetRow(iter.Current.target, out PropertyTable.Row targetRow))
                    continue;
                if (!lawMed.TryGetLaws(subjectRow.type, targetRow.type, out List<Law> laws))
                    continue;

                var lawIter = laws.GetEnumerator();
                while (lawIter.MoveNext())
                {
                    lawIter.Current.Insert(subjectRow, targetRow);
                }
            }
            graph.Reset();

            var ctrIter = tower.GetEnumerator();
            while (ctrIter.MoveNext())
            {
                var pair = ctrIter.Current;
                if (!pt.TryGetRow(pair.Key, out PropertyTable.Row row))
                    continue;

                unsafe
                {
                    var controller = PropertyHelper.Get<Property.Controller>(row);
                    if (pair.Value.changed)
                        pair.Value.CopyTo(controller);
                }
            }
        }
    }

    public static class Process
    {
        public static void Execute()
        {
            var lawCol = Locator.Instance.LawCollection;
            var iter = lawCol.GetEnumerator();
            while (iter.MoveNext())
            {
                var law = iter.Current;
                law.Execute();
                law.Reset();
            }
        }
    }
}
