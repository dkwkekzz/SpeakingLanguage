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

            // install

            var startInfo = new Logic.StartInfo()
            {
                port = int.Parse(ConfigurationManager.AppSettings["port"]),
                default_agentcount = int.Parse(ConfigurationManager.AppSettings["default_agentcount"]),
                default_scenecount = int.Parse(ConfigurationManager.AppSettings["default_scenecount"]),
            };

            var worldManager = WorldManager.Locator;
            worldManager.Install(ref startInfo);

            // process

            Console.WriteLine("=== SpeakingLanguage Server ===");

            var packet = new Network.PacketProcessor(ref startInfo);
            var logic = new LogicProcessor();

            ref var service = ref worldManager.Service;
            while (!Console.KeyAvailable)
            {
                service.Begin();

                packet.Update();
                logic.Update(ref service);

                var ret = service.End();
                if (ret.leg > 0)
                    continue;

                Thread.Sleep(ret.leg);
            }

            // uninstall

            packet.Dispose();
            logic.Dispose();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
