using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SpeakingLanguage.Library
{
    public static class TaskLogger
    {
        public enum TaskLogLevel { None, Pending };
        public static TaskLogLevel LogLevel { get; set; }
        
        public sealed class TaskLogEntry
        {
            public Task Task { get; internal set; }
            public string Tag { get; internal set; }
            public DateTime LogTime;
            public string CallerMemberName;
            public string CallerFilePath;
            public int CallerLineMember;
            public override string ToString()
            {
                return $"LogTime={LogTime}, Tag={Tag ?? "(none)"}, Member={CallerMemberName}, File={CallerFilePath}/{CallerLineMember}";
            }
        }

        private static readonly ConcurrentDictionary<Task, TaskLogEntry> s_log = new ConcurrentDictionary<Task, TaskLogEntry>();
        public static IEnumerable<TaskLogEntry> GetLogEntries() { return s_log.Values; }

        public static Task<TResult> Log<TResult>(this Task<TResult> task, string tag = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = -1)
        {
            return (Task<TResult>)Log((Task)task, tag, callerMemberName, callerFilePath, callerLineNumber);
        }

        public static Task Log(this Task task, string tag = null,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = -1)
        {
            if (LogLevel == TaskLogLevel.None) return task;
            var logEntry = new TaskLogEntry
            {
                Task = task,
                LogTime = DateTime.Now,
                Tag = tag,
                CallerMemberName = callerMemberName,
                CallerFilePath = callerFilePath,
                CallerLineMember = callerLineNumber
            };
            s_log[task] = logEntry;
            task.ContinueWith(t => { TaskLogEntry entry; s_log.TryRemove(t, out entry); },
                TaskContinuationOptions.ExecuteSynchronously);
            return task;
        }
    }
}
