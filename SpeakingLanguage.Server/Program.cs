using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;

namespace SpeakingLanguage.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            //Test ntp
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

            int port = "port".ParseConfig();
            using (var processor = new Networks.PacketProcessor())
            {
                Console.WriteLine("=== SpeakingLanguage Server ===");
                processor.Run(port);
            }

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
