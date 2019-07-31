using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public unsafe struct WorldActor
    {
        private readonly Interact.sObserver* _ob;

        internal WorldActor(Interact.sObserver* ob)
        {
            _ob = ob;
        }

        public StateValue* GetState(Define.Controller type) => _ob->GetState(type);

        public TEntity* AddEntity<TEntity>() where TEntity : unmanaged => _ob->AddEntity<TEntity>();
        public void AddEntity<TEntity>(TEntity e) where TEntity : unmanaged => _ob->AddEntity<TEntity>(e);
        public bool RemoveEntity<TEntity>() => _ob->RemoveEntity<TEntity>();
        public void SetEntity<TEntity>(TEntity* src, TEntity value) where TEntity : unmanaged => _ob->SetEntity<TEntity>(src, value);
        public TEntity* GetEntity<TEntity>() where TEntity : unmanaged => _ob->GetEntity<TEntity>();
    }
}
