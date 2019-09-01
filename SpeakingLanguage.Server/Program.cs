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
                default_usercount = int.Parse(ConfigurationManager.AppSettings["default_usercount"]),
                default_dummycount = int.Parse(ConfigurationManager.AppSettings["default_dummycount"]),
                default_scenecount = int.Parse(ConfigurationManager.AppSettings["default_scenecount"]),
                default_objectcount = int.Parse(ConfigurationManager.AppSettings["default_objectcount"]),
                default_frameRate = int.Parse(ConfigurationManager.AppSettings["default_frameRate"]),
            };

            var worldManager = WorldManager.Instance;
            worldManager.Install(ref startInfo);

            var eventManager = Logic.EventManager.Instance;
            eventManager.Install(ref startInfo);

            // process

            var packet = new Network.PacketProcessor();
            packet.Run(startInfo.port);

            // uninstall
            
            packet.Dispose();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
