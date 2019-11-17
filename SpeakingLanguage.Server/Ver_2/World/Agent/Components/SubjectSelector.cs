using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Server
{
    internal interface IComponent
    {
        void OnDeserialize(ref Library.Reader reader);
        void OnSerialize(ref Library.Writer writer);
    }

    internal sealed class SubjectSelector : ISerializable
    {
        private readonly Dictionary<Logic.slObjectHandle, long> _dicSubject;
        
        public Logic.slObjectHandle Current { get; private set; }

        public SubjectSelector()
        {
            _dicSubject = new Dictionary<Logic.slObjectHandle, long>(8);
        }

        public void OnDeserialize(ref Library.Reader reader)
        {
            var read = reader.ReadInt(out int length);
            if (!read) Library.ThrowHelper.ThrowFailToConvert($"[SubjectSelector::OnDeserialize] name: length, position:{reader.Position.ToString()}");

            for (int i = 0; i != length; i++)
            {
                read &= reader.ReadLong(out long objUid);
                var eRet = Logic.EventManager.Instance.DeserializeObject(ref reader);
                if (!eRet.Success) Library.ThrowHelper.ThrowFailToConvert($"[SubjectSelector::OnDeserialize] name: objUid, position:{reader.Position.ToString()}");

                var objHandle = eRet.result;
                _dicSubject.Add(objHandle, objUid);
            }
        }

        public void OnSerialize(ref Library.Writer writer)
        {
            writer.WriteInt(_dicSubject.Count);

            var iter = _dicSubject.GetEnumerator();
            while (iter.MoveNext())
            {
                var pair = iter.Current;
                writer.WriteLong(pair.Value);

                var eRet = Logic.EventManager.Instance.SerializeObject(pair.Key.value, ref writer);
                if (!eRet.Success) Library.ThrowHelper.ThrowFailToConvert("[SubjectSelector::OnSerialize] ");
            }
        }

        public bool ContainsSubject(Logic.slObjectHandle handle)
        {
            return _dicSubject.ContainsKey(handle);
        }

        public void CaptureSubject(Logic.slObjectHandle handle)
        {
            Current = handle;
        }

        public bool TryCaptureSubject(Logic.slObjectHandle handle, out long uid)
        {
            if (!_dicSubject.TryGetValue(handle, out uid))
                return false;

            Current = handle;
            return true;
        }

        public void InsertSubject(long objUid, Logic.slObjectHandle handle)
        {
            _dicSubject.Add(handle, objUid);
        }

        public bool RemoveSubject(Logic.slObjectHandle handle)
        {
            return _dicSubject.Remove(handle);
        }
    }
}
