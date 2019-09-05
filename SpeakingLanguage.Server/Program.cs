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

            var worldInfo = new World.StartInfo()
            {
                port = "port".ParseConfig(),
                default_usercount = "port".ParseConfigOrDefault(10),
                default_dummycount = "default_dummycount".ParseConfigOrDefault(100),
                default_scenecount = "default_scenecount".ParseConfigOrDefault(100),
            };
            var worldManager = WorldManager.Instance;
            worldManager.Install(ref worldInfo);

            var logicInfo = new Logic.StartInfo()
            {
                default_frameRate = "default_frameRate".ParseConfigOrDefault(60),
                default_objectcount = "default_objectcount".ParseConfigOrDefault(10),
                default_interactcount = "default_interactcount".ParseConfigOrDefault(100),
                default_workercount = "default_workercount".ParseConfigOrDefault(1),
                default_jobchunklength = "default_jobchunklength".ParseConfigOrDefault(16),
            };
            var eventManager = Logic.EventManager.Instance;
            eventManager.Install(ref logicInfo);

            // process

            var processor = new Network.PacketProcessor();
            processor.Run(worldInfo.port);

            // uninstall
            
            processor.Dispose();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
