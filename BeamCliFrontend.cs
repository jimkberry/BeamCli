using System.ComponentModel;
using System;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BeamBackend;
using BikeControl;
using UniLog;

namespace BeamCli 
{
    
    public class BeamCliFrontend : IBeamFrontend
    {
        public  Dictionary<string, FrontendBike> feBikes; 
        public IBeamBackend backend;
        public BeamGameInstance gameInst;
        protected BeamCliModeHelper _feModeHelper;
        protected BeamUserSettings userSettings;
        public UniLogger logger;

        // Start is called before the first frame update
        public BeamCliFrontend(BeamUserSettings startupSettings)
        {
            _feModeHelper = new BeamCliModeHelper(this);
            feBikes = new Dictionary<string, FrontendBike>();
            userSettings = startupSettings;
            logger = UniLogger.GetLogger("Frontend");
        }

        public void SetBackend(IBeamBackend back)
        {
            backend = back;
            gameInst = back as BeamGameInstance;
            back.PeerJoinedEvt += OnPeerJoinedEvt;
            back.PeerLeftEvt += OnPeerLeftEvt;            
            back.PeersClearedEvt += OnPeersClearedEvt;   
            back.NewBikeEvt += OnNewBikeEvt;   
            back.BikeRemovedEvt += OnBikeRemovedEvt;   
            back.BikesClearedEvt +=OnBikesClearedEvt;   
            back.PlaceClaimedEvt += OnPlaceClaimedEvt;
            back.PlaceHitEvt += OnPlaceHitEvt;

            back.ReadyToPlayEvt += OnReadyToPlay;

            // TODO: Maybe move these to IBackend and add a Raise[Foo]Evt method?
            back.GetGround().PlaceFreedEvt += OnPlaceFreedEvt;
            back.GetGround().PlacesClearedEvt += OnPlacesClearedEvt; 
            back.GetGround().SetupPlaceMarkerEvt += OnSetupPlaceMarkerEvt;                         
        }
    
        public virtual void Loop(float frameSecs)
        {
            foreach( FrontendBike bike in feBikes.Values)
            {
                bike.Loop(frameSecs);   
            }
        }

        //
        // IBeamFrontend API
        //   
        public BeamUserSettings GetUserSettings() => userSettings;

        // Backend game modes
        public void OnStartMode(int modeId, object param) =>  _feModeHelper.OnStartMode(modeId, param);
        public void OnEndMode(int modeId, object param) => _feModeHelper.OnEndMode(modeId, param);
        
        // Players
        public void OnPeerJoinedEvt(object sender, BeamPeer p)
        {
            logger.Info($"OnPeerJoinedEvt() name: {p.Name}, Id: {p.PeerId}");
        }

        public void OnPeerLeftEvt(object sender, string p2pId) 
        {
            logger.Info($"OnPeerLeftEvt(): {p2pId}");            
        }

        public void OnPeersClearedEvt(object sender, EventArgs e)
        {
            // Probably never will do anything
            logger.Verbose("OnClearPeers() currently does nothing");
        }

        // Bikes
        public void OnNewBikeEvt(object sender, IBike ib)
        {
            logger.Info($"OnNewBikeEvt(). Id: {ib.bikeId}, Local: {ib.peerId == gameInst.LocalPeerId}, AI: {ib.ctrlType == BikeFactory.AiCtrl}"); 
            FrontendBike b = FeBikeFactory.Create(ib);
            b.Setup(ib, backend);
            feBikes[ib.bikeId] = b;
        }
        public void OnBikeRemovedEvt(object sender, BikeRemovedData rData)
        {
            logger.Info(string.Format("OnBikeRemovedEvt({0}). Id: {1}", rData.doExplode ? "Boom!" : "", rData.bikeId));   
            feBikes.Remove(rData.bikeId);                        
        }  
        public void OnBikesClearedEvt(object sender, EventArgs e)
        {
            logger.Verbose(string.Format("OnBikesClearedEvt()"));      
		    feBikes.Clear();              
        }    


        // Places
        
        public void OnPlaceHitEvt(object sender, PlaceHitArgs args)        
        {        
            logger.Info($"OnPlaceHitEvt. Place: ({args.p.xIdx}, {args.p.zIdx})  Bike: {args.ib.bikeId}");        
        }    

        public void OnPlaceClaimedEvt(object sender, Ground.Place p)        
        {         
            logger.Verbose($"OnPlaceClaimedEvt. Pos: ({p.xIdx}, {p.zIdx})  Bike: {p.bike.bikeId}");     
        }

        // Ground
        public void OnSetupPlaceMarkerEvt(object sender, Ground.Place p)
        {
            logger.Debug($"OnSetupPlaceMarkerEvt({p.xIdx}, {p.zIdx})"); 
        }

        public void OnPlaceFreedEvt(object sender, Ground.Place p)
        {
            logger.Debug($"OnFreePlace({p.xIdx}, {p.zIdx})");                  
        }   

        public void OnPlacesClearedEvt(object sender, EventArgs e)
        {
           logger.Debug($"OnClearPlaces()");
        }

        public void OnReadyToPlay(object sender, EventArgs e)
        {
           logger.Info($"OnReadyToPlay()");    
           backend.OnSwitchModeReq(BeamModeFactory.kPlay, null);        
        }

    }

    public class IntBeamCliFrontend : BeamCliFrontend
    {
        public IntBeamCliFrontend(BeamUserSettings startupSettings) : base(startupSettings)
        {

        }

        public override void Loop(float frameSecs)
        {
            base.Loop(frameSecs);
        }

    }

}
