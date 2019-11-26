using System;
using System.Threading;
using BeamBackend;

namespace BeamCli
{
    class Program
    {
        static void Main(string[] args)
        {
            CliDriver drv = new CliDriver();
            drv.Run();
        }
    }


    class CliDriver
    {
        public long targetFrameMs {get; private set;} = 60;

        public BeamGameInstance gameInst = null;
        public BeamCliFrontend fe = null;

        public void Run() {
            UnityEngine.Debug.Log("Starting");
            Init();
            LoopUntilDone();
        }

        protected void Init()
        {
            fe = new BeamCliFrontend();
            gameInst = new BeamGameInstance(fe,
                new BeamGameInstance.BeamGameConfig(){startMode= BeamModeFactory.kConnect});
            fe.SetBackendWeakRef(gameInst);
            gameInst.Start();
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
            fe.Loop(frameSecs);
            return gameInst.Loop(frameSecs);
        }

        private long _TimeMs() =>  DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

    }
}

