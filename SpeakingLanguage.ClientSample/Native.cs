using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SpeakingLanguage.ClientSample
{
    class Native
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct slObjectHandle
        {
		    public int value;
        }

        public enum LogicError
        {
            None = 0,
	    	FailToReadLength,
	    	FailToReadHandle,
	    	NullReferenceObject,
	    	NullReferenceControlState,
	    	SelfInteraction,
	    	OverflowObjectCapacity,
	    }

        [StructLayout(LayoutKind.Sequential)]
        public struct LogicResult
        {
            public LogicError error;
        }

        public struct FrameResult
        {
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void ProgressCallback(int value);

        [DllImport("SpeakingLanguage.Core.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static LogicResult Sample(int val);
        [DllImport("SpeakingLanguage.Core.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static LogicResult RegistAction(int key, ProgressCallback callback);
        [DllImport("SpeakingLanguage.Core.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static LogicResult CreateObject(out slObjectHandle outHandle);
        [DllImport("SpeakingLanguage.Core.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static LogicResult DestroyObject(slObjectHandle handle);
        [DllImport("SpeakingLanguage.Core.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static LogicResult DeserializeObject(byte[] buffer, out slObjectHandle outHandle);
        [DllImport("SpeakingLanguage.Core.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static LogicResult SerializeObject(slObjectHandle handle, byte[] outBuffer);
        [DllImport("SpeakingLanguage.Core.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static LogicResult InsertKeyboard(slObjectHandle handle, int key, int value);
        [DllImport("SpeakingLanguage.Core.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static LogicResult InsertTouch(slObjectHandle handle, int key, int value);
        [DllImport("SpeakingLanguage.Core.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static LogicResult InsertInteraction(slObjectHandle subject, slObjectHandle target);

        [DllImport("SpeakingLanguage.Core.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static  void EnterFrame();
        [DllImport("SpeakingLanguage.Core.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static FrameResult ExecuteFrame();
    }
}
