using System;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BeamBackend;
using BikeControl;

namespace BeamCli 
{
    
    public class BeamCliFrontend : IBeamFrontend
    {
        public  Dictionary<string, FrontendBike> feBikes;
        
        public WeakReference backend;
        protected BeamCliModeHelper feModeHelper;

        protected BeamUserSettings userSettings;


        // Start is called before the first frame update
        public BeamCliFrontend(BeamUserSettings startupSettings)
        {
            feModeHelper = new BeamCliModeHelper();
            feBikes = new Dictionary<string, FrontendBike>();
            userSettings = startupSettings;
        }

        public void SetBackendWeakRef(IBeamBackend back)
        {
            backend = new WeakReference(back);
        }
    
        public void Loop(float frameSecs)
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
        public IFrontendModeHelper ModeHelper() => (IFrontendModeHelper)feModeHelper;

        // Players
        public void OnNewPlayer(Player p)
        {
            UnityEngine.Debug.Log("FE.OnNewPlayer() currently does nothing");
        }

        public void OnClearPlayers()
        {
            UnityEngine.Debug.Log("FE.OnClearPlayers() currently does nothing");
        }

        // Bikes
        public void OnNewBike(IBike ib)
        {
            UnityEngine.Debug.Log(string.Format("FE.OnNewBike(). Id: {0}, Local: {1}", ib.bikeId, ib.player.IsLocal ? "Yes" : "No" )); 
            FrontendBike b = BikeFactory.Create(ib);
            b.Setup(ib, backend.Target as IBeamBackend);
            feBikes[ib.bikeId] = b;
        }
        public void OnBikeRemoved(string bikeId, bool doExplode)
        {
            UnityEngine.Debug.Log(string.Format("FE.OnBikeRemoved({0}). Id: {1}", doExplode ? "Boom!" : "", bikeId));   
            feBikes.Remove(bikeId);                        
        }  
        public void OnClearBikes()
        {
            UnityEngine.Debug.Log(string.Format("FE.OnClearBikes()"));      
		    feBikes.Clear();              
        }    

        public void OnBikeAtPlace(string bikeId, Ground.Place place, bool justClaimed)
        {        
            //if (place != null)
            //    UnityEngine.Debug.Log(string.Format("FE.OnBikeAtPlace({0},{1})", place.xIdx, place.zIdx));        
        }    

        // Ground
        public void SetupPlaceMarker(Ground.Place p)
        {         
            //UnityEngine.Debug.Log(string.Format("FE.SetupPlaceMarker({0},{1})", p.xIdx, p.zIdx));     
        }
        public void OnFreePlace(Ground.Place p)
        {
            //UnityEngine.Debug.Log(string.Format("FE.OnFreePlace({0},{1})", p.xIdx, p.zIdx));                  
        }        
        public void OnClearPlaces()
        {
            UnityEngine.Debug.Log(string.Format("FE.OnClearPlaces()"));
        }

    }

}
