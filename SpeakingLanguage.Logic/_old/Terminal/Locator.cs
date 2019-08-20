using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    internal unsafe sealed class Locator : IDisposable
    {
        private readonly Library.umnMarshal _marshal;
        private readonly Library.umnHeap _largeHeap;

        public FrameManager FrameManager { get; }
        public FrontStack FrontStack { get; }
        public ActionDictionary ActionDictionary { get; }
        public CellDictionary CellDictionary { get; }
        public ObjectDictionary ObserverDictionary { get; }

        public Locator(ref StartInfo info)
        {
            _largeHeap = new Library.umnHeap(_marshal.Alloc(info.max_byte_terminal));

            FrameManager = new FrameManager(info.startFrame);
            FrontStack = new FrontStack(_largeHeap.Calloc(info.max_byte_frontstack));
            ActionDictionary = new ActionDictionary();
            CellDictionary = new CellDictionary(_largeHeap.Alloc(info.max_byte_celldictionary), info.default_count_cell);
            ObserverDictionary = new ObjectDictionary(_largeHeap.Alloc(info.max_byte_observerdictionary), info.default_count_observer);
        }

        public void Dispose()
        {
            _marshal.Dispose();
        }
    }
}
