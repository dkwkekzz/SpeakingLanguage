using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteNetLib.Utils;

namespace SpeakingLanguage.ClientWpf
{
    static class Bootstrap
    {
        public static void Start()
        {
            NtpRequest ntpRequest = null;
            ntpRequest = NtpRequest.Create("pool.ntp.org", ntpPacket =>
            {
                ntpRequest.Close();
                if (ntpPacket != null)
                    Console.WriteLine("[MAIN] NTP time test offset: " + ntpPacket.CorrectionOffset);
                else
                    Console.WriteLine("[MAIN] NTP time error");
            });
            ntpRequest.Send();

            var startInfo = new Logic.StartInfo()
            {
                default_frameRate = "default_frameRate".ParseConfigOrDefault(60),
                default_objectcount = "default_objectcount".ParseConfigOrDefault(10),
                default_interactcount = "default_interactcount".ParseConfigOrDefault(100),
                default_workercount = "default_workercount".ParseConfigOrDefault(1),
                default_jobchunklength = "default_jobchunklength".ParseConfigOrDefault(16),
            };

            var processor = new PacketProcessor();
            processor.Run("port".ParseConfig());
        }
    }
}
