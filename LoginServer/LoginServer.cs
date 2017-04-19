using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;
using ExitGames.Logging;
using log4net;
using System.IO;
using log4net.Config;
using ExitGames.Logging.Log4Net;

namespace LoginServer
{

    public class LoginServer : ApplicationBase
    {
        private static ILogger log = ExitGames.Logging.LogManager.GetCurrentClassLogger();

        public static void Log(string str)
        {
            log.Info(str.ToString());
        }

        protected virtual void InitLogging()
        {
            ExitGames.Logging.LogManager.SetLoggerFactory(Log4NetLoggerFactory.Instance);
            GlobalContext.Properties["Photon:ApplicationLogPath"] =
            Path.Combine(this.ApplicationRootPath, "log");
            GlobalContext.Properties["LogFileName"] = "My" + this.ApplicationName;
            XmlConfigurator.ConfigureAndWatch(new
            FileInfo(Path.Combine(this.BinaryPath, "log4net.config")));
        }

        protected override PeerBase CreatePeer(InitRequest initRequest)
        {
            return new LoginPeer(initRequest);
        }

        protected override void Setup()
        {
            InitLogging();

            Log("Setup ok.");
        }

        protected override void TearDown()
        {
            Log("TearDown ok.");
        }
    }
}
