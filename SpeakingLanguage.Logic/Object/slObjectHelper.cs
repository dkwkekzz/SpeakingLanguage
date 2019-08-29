using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    internal static unsafe class slObjectHelper
    {
        public static slObject* GetNext(slObject* curObj)
        {
            var nextChkPtr = (IntPtr)curObj + sizeof(slObject) + curObj->capacity;
            if (((Library.umnChunk*)nextChkPtr)->length == 0)
                return null;

            var nextObjPtr = nextChkPtr + sizeof(Library.umnChunk);
            return (slObject*)nextObjPtr;
        }

        public static Default* GetDefaultState(slObject* obj)
        {
            var ptr = (IntPtr)obj + sizeof(slObject) + sizeof(Library.umnChunk);
            return (Default*)ptr.ToPointer();
        }

        public static Control* GetControlState(slObject* obj)
        {
            var iter = (Library.umnChunk*)((IntPtr)obj + sizeof(slObject) + sizeof(Library.umnChunk) + sizeof(Default));
            while (null != iter)
            {
                if (iter->typeHandle == TypeManager.SHControlState.key)
                    return Library.umnChunk.GetPtr<Control>(iter);

                iter = Library.umnChunk.GetNext(iter);
            }

            return null;
        }
    }
}
