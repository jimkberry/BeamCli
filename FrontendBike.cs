using System;
using UnityEngine;
using BeamBackend;
using BikeControl;

namespace BeamCli
{
    public static class BikeFactory
    {
        public static FrontendBike Create(IBike ib)
        {
            System.Type bType = FrontendBike.bikeClassTypes[ib.ctrlType];
            return (FrontendBike)Activator.CreateInstance(bType);
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
        protected IBeamBackend be = null; 
        protected IBikeControl control = null;
        protected abstract void CreateControl();      

        public virtual void Setup(IBike beBike, IBeamBackend backend)
        {           
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
            Debug.Log(string.Format("AiBike.CreateControl()"));        
            control = new AiControl();
        }        
    }
    public class PlayerBike : FrontendBike
    {
        protected override void CreateControl()
        {
            Debug.Log(string.Format("PlayerBike.CreateControl()"));        
            control = new PlayerControl();
        }        
    }    
}