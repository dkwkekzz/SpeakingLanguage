using System;
using System.Diagnostics;
using System.IO;

namespace SpeakingLanguage.Library
{
    public class TextTraceListner : TraceListener
    {
		private DateTime createLogDateTime;
        private string logFilePath;
        private string prefixFileName;
        private string extension;
        DefaultTraceListener defaultListener;
		private int keepArchivedLogDays = 14;

		public TextTraceListner(string logFilePath, string prefixFileName = "Event", string extension = "txt")
        {
            this.logFilePath = logFilePath;
            this.prefixFileName = prefixFileName;
            this.extension = extension;

            DirectoryInfo directoryInfo = new DirectoryInfo(this.logFilePath);
            if (directoryInfo.Exists == false)
            {
                try
                {
                    Directory.CreateDirectory(directoryInfo.FullName);
                }
                catch (Exception exception)
                {
                    Trace.Assert(false, exception.Message);
                }
            }

			{
				FileInfo[] files = directoryInfo.GetFiles($"*.{this.extension}", SearchOption.AllDirectories);

				foreach (FileInfo file in files)
				{
					DateTime deleteDateTime = file.CreationTime.AddDays(this.keepArchivedLogDays);
					if (deleteDateTime > DateTime.Now)
					{
						continue;
					}
					file.Attributes = FileAttributes.Normal;
					file.Delete();
				}
			}

			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(OnUnhandledException);

			DateTime now = DateTime.Now;
            defaultListener = new DefaultTraceListener();
            defaultListener.LogFileName = String.Format($"{this.logFilePath}{this.prefixFileName}_{now.ToString("yyMMdd_HH")}.{this.extension}");
			defaultListener.AssertUiEnabled = false;

			this.createLogDateTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0);
		}
		private void update()
        {
            DateTime now = DateTime.Now;
			TimeSpan interval = now - createLogDateTime;
			if (interval.Hours < 1)
            {
                return;
            }

            this.defaultListener.LogFileName = String.Format($"{this.logFilePath}{this.prefixFileName}_{now.ToString("yyMMdd_HH")}.{this.extension}");
			this.createLogDateTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0);
		}
		private string time
        {
            get
            {
                return DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff");
            }
        }

        public override void Fail(string message)
        {
            this.WriteLine("");
            defaultListener.Fail(message);
            Environment.Exit(0);
        }
        public override void Fail(string message, string description)
        {
            this.WriteLine("");
            defaultListener.Fail(message, description);
            Environment.Exit(0);
        }
        public override void Write(string message)
        {
            this.update();
            defaultListener.Write(message);
        }

        public override void WriteLine(string message)
        {
            this.update();
            defaultListener.WriteLine(this.time + "\t" + message);
        }

        public static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Trace.WriteLine(e.ToString() + "\r\n" + e.ExceptionObject.ToString());
            Environment.Exit(0);
        }
    }
}
