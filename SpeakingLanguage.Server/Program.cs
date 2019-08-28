using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;

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

            var startInfo = new Logic.StartInfo()
            {
                port = int.Parse(ConfigurationManager.AppSettings["port"]),
                default_agentcount = int.Parse(ConfigurationManager.AppSettings["default_agentcount"]),
                default_scenecount = int.Parse(ConfigurationManager.AppSettings["default_scenecount"]),
            };

            var worldManager = WorldManager.Locator;
            worldManager.Install(ref startInfo);
            worldManager.Executor.Run(ref worldManager.Service);

            var processor = new PacketProcessor();
            processor.Run(ref startInfo);
        }
    }
}
