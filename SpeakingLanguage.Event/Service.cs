using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Event
{
    public unsafe interface IComponent
    {
        Logic.slObjectHandle* GetKey { get; }
        void Allocate(Logic.slObjectHandle hd);
    }

    internal struct MessageBox
    {
        public Logic.slObjectHandle handle;

        unsafe Logic.slObjectHandle* GetKey { get { fixed (Logic.slObjectHandle* ph = &handle) return ph; } }

        public void Allocate(int hd)
        {
            handle = hd;
        }
    }

    internal unsafe struct EventCollection
    {
        private readonly Library.umnHeap _heap;
        private readonly Library.umnArray<ChangeView> _arrChangeView;
        private readonly Library.umnArray<Controller> _arrController;
        private readonly Library.umnArray<Interaction> _arrInteraction;

        public EventCollection(Library.umnChunk* chk)
        {
            _heap = new Library.umnHeap(chk);
            _arrChangeView = Library.umnArray<ChangeView>.CreateNew(ref _heap, 100);
            _arrController = Library.umnArray<Controller>.CreateNew(ref _heap, 100);
            _arrInteraction = Library.umnArray<Interaction>.CreateNew(ref _heap, 100);
        }
        
        public void Insert(ChangeView st)
        {

        }

        public void Insert(Controller st)
        {

        }

        public void Insert(Interaction st)
        {

        }

        public void Reset()
        {

        }
    }
    
    public sealed class Service
    {
        private readonly Library.umnMarshal _umnAllocator;



    }
}
