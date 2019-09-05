using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Test
{
    public class TLS
    {
        public TLS()
        {
            Console.WriteLine("Creating TLS Class");
        }

        public int value;
    }

    public class TLSFlied
    {
        [ThreadStatic]
        public static TLS Tdata;
        public static TLS Data { get { if (null == Tdata) Tdata = new TLS(); return Tdata; } }
    }

    //public class EntryPoint
    //{
    //    private static void ThreadFun()
    //    {
    //        Console.WriteLine("Thread {0} starting.....", Thread.CurrentThread.ManagedThreadId);
    //        Console.WriteLine("TData for this thread is \"{0}\"", TLSFlied.Tdata);
    //        Console.WriteLine("thread {0} exiting", Thread.CurrentThread.ManagedThreadId);
    //    }
    //    static void Main()
    //    {
    //        Thread t1 = new Thread(new ThreadStart(EntryPoint.ThreadFun));
    //        Thread t2 = new Thread(new ThreadStart(EntryPoint.ThreadFun));
    //        t1.Start();
    //        t2.Start();
    //        Console.Read();
    //    }
    //}

    class TestTLS
    {
        private static ConcurrentDictionary<int, TLS> _dicTLS = new ConcurrentDictionary<int, TLS>();

        private static void ThreadFun()
        {
            var tid = Thread.CurrentThread.ManagedThreadId;
            _dicTLS.TryAdd(tid, new TLS());

            Console.WriteLine("Thread {0} starting.....", tid);
            TLSFlied.Data.value = 0;

            var timer = new Stopwatch();
            timer.Start();
            for (int i = 0; i != 100000000; i++)
            {
                //TLSFlied.Data.value++;
                _dicTLS[tid].value++;
            }
            timer.Stop();
            Console.WriteLine($"elapsed: {timer.ElapsedMilliseconds.ToString()}");

            //Console.WriteLine("TData for this thread is \"{0}\"", TLSFlied.Tdata.value.ToString());
            Console.WriteLine("thread {0} exiting", tid);
        }

        static void Main()
        {
            Task.Factory.StartNew(TestTLS.ThreadFun);
    
            Task.Factory.StartNew(TestTLS.ThreadFun);
    
            Console.ReadLine();
        }
    }
}
