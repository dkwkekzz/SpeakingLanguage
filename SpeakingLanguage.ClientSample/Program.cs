using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;

namespace SpeakingLanguage.ClientSample
{
    class Program
    {
        static void Main(string[] args)
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
                port = int.Parse(ConfigurationManager.AppSettings["port"]),
            };

            var processor = new PacketProcessor();
            processor.Run(ref startInfo);
        }
    }
}
