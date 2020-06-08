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
        public IBeamGameInstance backend;
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

        public void SetGameInstance(IBeamGameInstance back)
        {
            backend = back;
            if (back == null)
                return;

 ///           back.PeerJoinedGameEvt += OnPeerJoinedGameEvt;
///            back.PeerLeftGameEvt += OnPeerLeftGameEvt;
            back.PlayersClearedEvt += OnPlayersClearedEvt;
            back.NewBikeEvt += OnNewBikeEvt;
            back.BikeRemovedEvt += OnBikeRemovedEvt;
            back.BikesClearedEvt +=OnBikesClearedEvt;
            back.PlaceClaimedEvt += OnPlaceClaimedEvt;
            back.PlaceHitEvt += OnPlaceHitEvt;

            back.ReadyToPlayEvt += OnReadyToPlay;

            // TODO: Maybe move these to IBackend and add a Raise[Foo]Evt method?
            back.GameData.PlaceFreedEvt += OnPlaceFreedEvt;
            back.GameData.PlacesClearedEvt += OnPlacesClearedEvt;
            back.GameData.SetupPlaceMarkerEvt += OnSetupPlaceMarkerEvt;
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

        public void OnPeerJoinedGameEvt(object sender, PeerJoinedGameArgs args)
        {
        ///     BeamGroupMember p = args.peer;
        ///     logger.Info($"OnPeerJoinedEvt() name: {p.Name}, Id: {p.PeerId}");
        }

        public void OnPeerLeftGameEvt(object sender, PeerLeftGameArgs args)
        {
            logger.Info($"OnPeerLeftEvt(): {args.p2pId}");
        }

        public void OnPlayersClearedEvt(object sender, EventArgs e)
        {
            // Probably never will do anything
            logger.Verbose("OnClearPeers() currently does nothing");
        }

        // Bikes
        public void OnNewBikeEvt(object sender, IBike ib)
        {
            logger.Info($"OnNewBikeEvt(). Id: {ib.bikeId}, Local: {ib.peerId == backend.LocalPeerId}, AI: {ib.ctrlType == BikeFactory.AiCtrl}");
            FrontendBike b = FeBikeFactory.Create(ib, ib.peerId == backend.LocalPeerId);
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
            // This intentionally accesses linked data to show issues
            // IBike bike = args.ib;
            // string bikeOwner = bike.peerId;

            // BeamPlace place = args.p;
            // IBike createdBy = place.bike;  // TODO: CRASH happens here. When no longer useful make it bulletproof
            //                                // (would rather make it not happen - not sure if that's possible)
            // string placeOwner = createdBy.peerId;

            logger.Info($"OnPlaceHitEvt. Place: ({args.p?.xIdx}, {args.p?.zIdx})  Bike: {args.ib?.bikeId}");
        }

        public void OnPlaceClaimedEvt(object sender, BeamPlace p)
        {
            logger.Verbose($"OnPlaceClaimedEvt. Pos: ({p.xIdx}, {p.zIdx})  Bike: {p.bike.bikeId}");
        }

        // Ground
        public void OnSetupPlaceMarkerEvt(object sender, BeamPlace p)
        {
            logger.Debug($"OnSetupPlaceMarkerEvt({p.xIdx}, {p.zIdx})");
        }

        public void OnPlaceFreedEvt(object sender, BeamPlace p)
        {
            logger.Debug($"OnFreePlace({p.xIdx}, {p.zIdx})");
        }

        public void OnPlacesClearedEvt(object sender, EventArgs e)
        {
           logger.Debug($"OnClearPlaces()");
        }

        public void OnReadyToPlay(object sender, EventArgs e)
        {
            logger.Error($"OnReadyToPlay() - doesn't work anymore");
            //logger.Info($"OnReadyToPlay()");
            //&&& backend.OnSwitchModeReq(BeamModeFactory.kPlay, null);
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
