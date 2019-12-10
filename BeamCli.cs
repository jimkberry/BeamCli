using System;
using System.Diagnostics;
using System.Threading;
using CommandLine;
using BeamBackend;
using GameNet;
using P2pNet;

namespace BeamCli 
{
    class Program
    {
        public class CliOptions
        {
            [Option(
	            Default = null,
	            HelpText = "If set, join this game, if null, create a game")]            
            public string GameId {get; set;}
        }

        protected static BeamUserSettings GetSettings(string[] args)
        {
            BeamUserSettings settings = BeamUserSettings.CreateDefault();

            Parser.Default.ParseArguments<CliOptions>(args)
                   .WithParsed<CliOptions>(o =>
                   {
                       if (o.GameId != null)
                        settings.gameId = o.GameId;    
                   });

            return settings;
        }

        static void Main(string[] args)
        {
            P2pNetTrace.InitInCode(TraceLevel.Verbose);
            GameNetTrace.InitInCode(TraceLevel.Verbose);

            CliDriver drv = new CliDriver();
            drv.Run(GetSettings(args));
        }
    }


    class CliDriver
    { 
        public long targetFrameMs {get; private set;} = 60;

        public BeamGameInstance gameInst = null;
        public BeamCliFrontend fe = null;
        public BeamGameNet bgn = null;

        public void Run(BeamUserSettings settings) {
            UnityEngine.Debug.Log("Starting");
            Init(settings);
            LoopUntilDone();
        }

        protected void Init(BeamUserSettings settings)
        {
            fe = new BeamCliFrontend(settings);
            bgn = new BeamGameNet(); // TODO: config/settings?            
            gameInst = new BeamGameInstance(fe, bgn);
            bgn.Init(gameInst); // weakref
            fe.SetBackendWeakRef(gameInst);
            gameInst.Start(BeamModeFactory.kConnect);
        }

        protected void LoopUntilDone()
        {
            bool keepRunning = true;
            long frameStartMs = _TimeMs() - targetFrameMs;;              
            while (keepRunning)
            {
                long prevFrameStartMs = frameStartMs;
                frameStartMs = _TimeMs();

                // call loop
                keepRunning = Loop((int)(frameStartMs - prevFrameStartMs));
                long elapsedMs = _TimeMs() - frameStartMs;

                // wait to maintain desired rate
                int waitMs = (int)(targetFrameMs - elapsedMs);
                //UnityEngine.Debug.Log(string.Format("Elapsed ms: {0}, Wait ms: {1}",elapsedMs, waitMs));
                if (waitMs <= 0)
                    waitMs = 1;
                Thread.Sleep(waitMs);
            }
        }

        protected bool Loop(int frameMs)
        {
            // first dispatch incoming messages
            // while not self.netCmdQueue.empty():
            //     cmd = self.netCmdQueue.get(block=False)
            //     if cmd:
            //         self._dispatch_net_cmd(cmd)
            //         ge_sleep(0)  # yield         


            // while not self.feMsgQueue.empty():
            //     cmd = self.feMsgQueue.get(block=False)
            //     if cmd:
            //         self._dispatch_fe_cmd(cmd)
            //         ge_sleep(0)  # yield                           

            // then update the game

            float frameSecs = (float)frameMs / 1000f;    
            bgn.Loop();
            fe.Loop(frameSecs);
            return gameInst.Loop(frameSecs);
        }

        private long _TimeMs() =>  DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

    }
}

