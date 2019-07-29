using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public unsafe interface IEntityManager
    {
        int Handle { get; }

        Dictionary<Type, IntPtr>.Enumerator GetEnumerator();

        void AddEntity<TEntity>() where TEntity : unmanaged;
        void RemoveEntity<TEntity>(TEntity e) where TEntity : unmanaged;
        void SetEntity<TEntity>(TEntity e) where TEntity : unmanaged;
        TEntity* GetEntity<TEntity>() where TEntity : unmanaged;
    }
}
