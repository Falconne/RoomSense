using System.Collections.Generic;
using HugsLib.Settings;
using HugsLib.Utils;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace RoomSense
{
    public class Main : HugsLib.ModBase
    {

        internal new ModLogger Logger => base.Logger;

        internal static Main Instance { get; private set; }

        public override string ModIdentifier => "RoomSense";

        private SettingHandle<int> _opacity;

        private SettingHandle<int> _updateDelay;

        private readonly GraphOverlay _graphOverlay = new GraphOverlay();

        public Main()
        {
            Instance = this;
        }

        public void UpdateOverlays()
        {
            //_graphOverlay.Update(_updateDelay);
        }

        public override void OnGUI()
        {
            if (Current.ProgramState != ProgramState.Playing || Find.VisibleMap == null
                || WorldRendererUtility.WorldRenderedNow)
            {
                return;
            }

            _graphOverlay.Update(_updateDelay);
        }

        public override void WorldLoaded()
        {
            //Reset();
        }

        public override void DefsLoaded()
        {
            _opacity = Settings.GetHandle(
                "opacity", "FALCHM.OverlayOpacity".Translate(),
                "FALCHM.OverlayOpacityDesc".Translate(), 30,
                Validators.IntRangeValidator(1, 100));

            // _opacity.OnValueChanged = val => { Reset(); };

            _updateDelay = Settings.GetHandle("updateDelay", "FALCRS.UpdateDelay".Translate(),
                "FALCRS.UpdateDelayDesc".Translate(),
                100, Validators.IntRangeValidator(1, 9999));

        }

        public float GetConfiguredOpacity()
        {
            return _opacity / 100f;
        }
    }
}
