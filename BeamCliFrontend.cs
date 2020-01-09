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
        public WeakReference backend;
        protected BeamCliModeHelper _feModeHelper;
        protected BeamUserSettings userSettings;
        public UniLogger logger;

        // Start is called before the first frame update
        public BeamCliFrontend(BeamUserSettings startupSettings)
        {
            _feModeHelper = new BeamCliModeHelper();
            feBikes = new Dictionary<string, FrontendBike>();
            userSettings = startupSettings;
            logger = UniLogger.GetLogger("Frontend");
        }

        public void SetBackendWeakRef(IBeamBackend back)
        {
            backend = new WeakReference(back);
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
        public void OnNewPeer(BeamPeer p, int modeId)
        {
            Console.WriteLine($"New Peer: {p.Name}, Id: {p.PeerId}");
        }

        public void OnPeerLeft(string p2pId, int modeId) 
        {
            logger.Info("Peer Left: {p2pId}");            
        }

        public void OnClearPeers(int modeId)
        {
            // Probably never will do anything
            logger.Info("OnClearPeers() currently does nothing");
        }

        // Bikes
        public void OnNewBike(IBike ib, int modeId)
        {
            logger.Info($"FE.OnNewBike(). Id: {ib.bikeId}, LocalPlayer: {ib.ctrlType == BikeFactory.LocalPlayerCtrl}"); 
            FrontendBike b = FeBikeFactory.Create(ib);
            b.Setup(ib, backend.Target as IBeamBackend);
            feBikes[ib.bikeId] = b;
        }
        public void OnBikeRemoved(string bikeId, bool doExplode, int modeId)
        {
            logger.Info(string.Format("FE.OnBikeRemoved({0}). Id: {1}", doExplode ? "Boom!" : "", bikeId));   
            feBikes.Remove(bikeId);                        
        }  
        public void OnClearBikes(int modeId)
        {
            logger.Info(string.Format("FE.OnClearBikes()"));      
		    feBikes.Clear();              
        }    

        public void OnBikeAtPlace(string bikeId, Ground.Place place, bool justClaimed, int modeId)
        {        
            if (place != null)
                logger.Debug(string.Format("FE.OnBikeAtPlace({0},{1})", place.xIdx, place.zIdx));        
        }    

        // Ground
        public void SetupPlaceMarker(Ground.Place p, int modeId)
        {         
            logger.Debug(string.Format("FE.SetupPlaceMarker({0},{1})", p.xIdx, p.zIdx));     
        }

        public void OnFreePlace(Ground.Place p, int modeId)
        {
            logger.Debug(string.Format("FE.OnFreePlace({0},{1})", p.xIdx, p.zIdx));                  
        }   

        public void OnClearPlaces(int modeId)
        {
           logger.Info($"OnClearPlaces()");
        }

    }

    public class IntBeamCliFrontend : BeamCliFrontend
    {
        public IntBeamCliFrontend(BeamUserSettings startupSettings) : base(startupSettings)
        {
            _feModeHelper = new BeamCliModeHelper();
            feBikes = new Dictionary<string, FrontendBike>();
            userSettings = startupSettings;
            logger = UniLogger.GetLogger("Frontend");
        }

        public override void Loop(float frameSecs)
        {
            base.Loop(frameSecs);
        }

    }

}
