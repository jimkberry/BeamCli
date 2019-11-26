using System;
using System.Collections.Generic;
using UnityEngine;
using BeamBackend;

namespace BeamCli
{
    public class BeamCliModeHelper : IFrontendModeHelper
    {

        protected abstract class ModeFuncs
        {
            public abstract void OnStart(object parms);
            public abstract void OnEnd(object parms);
            public void HandleCmd(int cmdId, object parms) => _cmdDispatch[cmdId](parms);
            protected Dictionary<int,dynamic> _cmdDispatch;

            public ModeFuncs()
            {
                _cmdDispatch = new Dictionary<int, dynamic>();
            }
        }



        protected Dictionary<int, ModeFuncs> _modeFuncs;

        public BeamCliModeHelper()
        {
            _modeFuncs = new Dictionary<int, ModeFuncs>()
            {
                { BeamModeFactory.kStartup, new StartupModeFuncs()},
                { BeamModeFactory.kConnect, new ConnectModeFuncs()},
                { BeamModeFactory.kSplash, new SplashModeFuncs()},
                { BeamModeFactory.kPlay, new PlayModeFuncs()}
            };
        }

        public void OnStartMode(int modeId, object parms=null)
        {
            _modeFuncs[modeId].OnStart(parms);
        }
        public void DispatchCmd(int modeId, int cmdId, object parms=null)
        {
            _modeFuncs[modeId].HandleCmd(cmdId, parms);
        }
        public void OnEndMode(int modeId, object parms=null)
        {
            _modeFuncs[modeId].OnEnd(parms);
        }

        // Implementations
        class StartupModeFuncs : ModeFuncs
        {
            public StartupModeFuncs() : base() {}
            public override void OnStart(object parms=null) {}
            public override void OnEnd(object parms=null) {}
        }
        class ConnectModeFuncs : ModeFuncs
        {
            public ConnectModeFuncs() : base() {}
            public override void OnStart(object parms=null) {}
            public override void OnEnd(object parms=null) {}
        }
        class SplashModeFuncs : ModeFuncs
        {
            public SplashModeFuncs() : base()
            {
    //           _cmdDispatch[ModeSplash.kCmdTargetCamera] = new Action<object>(o => TargetCamera(o));
            }

            // protected void TargetCamera(ModeSplash.TargetIdParams parm)
            // {

            // }

            public override void OnStart(object parms=null)
            {
                TargetIdParams p = (TargetIdParams)parms;
            }

            public override void OnEnd(object parms=null)
            {

            }


        }

        class PlayModeFuncs : ModeFuncs
        {
            public PlayModeFuncs() : base()
            {
    //           _cmdDispatch[ModeSplash.kCmdTargetCamera] = new Action<object>(o => TargetCamera(o));
            }

            // protected void TargetCamera(ModeSplash.TargetIdParams parm)
            // {

            // }

            public override void OnStart(object parms=null)
            {
                TargetIdParams p = (TargetIdParams)parms;
            }

            public override void OnEnd(object parms=null)
            {

            }

        }

    }
}
