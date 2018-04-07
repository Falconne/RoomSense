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

        public bool ShowGraphOverlay = true;

        private SettingHandle<int> _graphOpacity;

        private float _graphOpacityAsFloat;

        private bool firstRun = true;

        private SettingHandle<int> _updateDelay;

        private readonly InfoCollector _infoCollector = new InfoCollector();

        private readonly GraphOverlay _graphOverlay = new GraphOverlay();

        public Main()
        {
            Instance = this;
        }

        public void UpdateOverlays()
        {
            if (!ShowGraphOverlay)
            {
                _infoCollector.Reset();
                return;
            }
            _infoCollector.Update(_updateDelay);
        }

        public override void OnGUI()
        {
            if (!ShowGraphOverlay)
                return;

            if (Current.ProgramState != ProgramState.Playing || Find.VisibleMap == null
                || WorldRendererUtility.WorldRenderedNow)
            {
                return;
            }

            if (firstRun)
            {
                firstRun = false;
                _graphOpacityAsFloat = _graphOpacity / 100f;
            }
            _graphOverlay.OnGUI(_infoCollector, _graphOpacityAsFloat);
        }

        public override void WorldLoaded()
        {
            _infoCollector.Reset();
        }

        public override void DefsLoaded()
        {
            _graphOpacity = Settings.GetHandle(
                "graphOpacity", "FALCRS.GraphOpacity".Translate(),
                "FALCRS.GraphOpacityDesc".Translate(), 100,
                Validators.IntRangeValidator(1, 100));

            _graphOpacity.OnValueChanged = val => { _graphOpacityAsFloat = _graphOpacity / 100f; };

            _updateDelay = Settings.GetHandle("updateDelay", "FALCRS.UpdateDelay".Translate(),
                "FALCRS.UpdateDelayDesc".Translate(),
                100, Validators.IntRangeValidator(1, 9999));

            _updateDelay.OnValueChanged = val => { _infoCollector.Reset(); };
        }
    }
}
