using System;
using UniLog;
using UnityEngine;
using BeamBackend;
using BikeControl;

namespace BeamCli
{
    public static class FeBikeFactory
    {
        public static FrontendBike Create(IBike ib)
        {
            FrontendBike feb = null;
            switch (ib.ctrlType)
            {
            case BikeFactory.LocalPlayerCtrl:
                feb = new PlayerBike();
                break;
            case BikeFactory.AiCtrl:
                feb = new AiBike();
                break;
            case BikeFactory.RemoteCtrl:
                feb = new RemoteBike();
                break;
            }
            return feb;
        }
    }

    public abstract class FrontendBike
    {
        public static readonly System.Type[] bikeClassTypes = {
            typeof(FrontendBike), // remote bike. Nothing to do here.
            typeof(AiBike),  // AiCtrl - AI - controlled local bike
            typeof(PlayerBike) // LocalPlayerCtrl - a human on this machine
        };

        public IBike bb = null;
        protected IBeamGameInstance be = null;
        protected IBikeControl control = null;
        protected abstract void CreateControl();

        public UniLogger Logger;

        public virtual void Setup(IBike beBike, IBeamGameInstance backend)
        {
            Logger = UniLogger.GetLogger("Frontend");  // use the FE logger
            be = backend;
            bb = beBike;
            CreateControl();
            control.Setup(beBike, backend);
        }

        public virtual void Loop(float frameSecs)
        {
            control.Loop(frameSecs);
        }

    }

    public class AiBike : FrontendBike
    {
        protected override void CreateControl()
        {
            Logger.Verbose($"AiBike.CreateControl()");
            control = new AiControl();
        }
    }
    public class PlayerBike : FrontendBike
    {
        protected override void CreateControl()
        {
            Logger.Verbose($"PlayerBike.CreateControl()");
            control = new PlayerControl();
        }
    }

    public class RemoteBike : FrontendBike
    {
        protected override void CreateControl()
        {
            Logger.Verbose($"PlayerBike.RemoteBike()");
            control = new RemoteControl();
        }
    }
}