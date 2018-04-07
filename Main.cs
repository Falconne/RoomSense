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

        public bool ShowOverlay = true;

        private SettingHandle<int> _graphOpacity;

        private SettingHandle<int> _heatMapOpacity;

        private float _graphOpacityAsFloat;

        private bool _firstRun = true;

        private SettingHandle<int> _updateDelay;

        private readonly InfoCollector _infoCollector = new InfoCollector();

        private readonly GraphOverlay _graphOverlay = new GraphOverlay();

        private HeatMap _heatMap;

        public Main()
        {
            Instance = this;
        }

        public void UpdateOverlays()
        {
            if (!ShowOverlay)
            {
                _infoCollector.Reset();
                _heatMap?.Reset();
                return;
            }
            _infoCollector.Update(_updateDelay);
            if (_heatMap == null)
                _heatMap = new HeatMap(_infoCollector);
            _heatMap.Update();
        }

        public override void OnGUI()
        {
            if (!ShowOverlay)
                return;

            if (Current.ProgramState != ProgramState.Playing || Find.VisibleMap == null
                || WorldRendererUtility.WorldRenderedNow)
            {
                return;
            }

            if (_firstRun)
            {
                _firstRun = false;
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

            _heatMapOpacity = Settings.GetHandle(
                "rsHeatMapOpacity", "FALCRS.HeatMapOpacity".Translate(),
                "FALCRS.HeatMapOpacityDesc".Translate(), 33,
                Validators.IntRangeValidator(1, 100));


            _updateDelay = Settings.GetHandle("updateDelay", "FALCRS.UpdateDelay".Translate(),
                "FALCRS.UpdateDelayDesc".Translate(),
                100, Validators.IntRangeValidator(1, 9999));

            _updateDelay.OnValueChanged = val => { _infoCollector.Reset(); };
        }

        public float GetHeatMapOpacity()
        {
            return _heatMapOpacity / 100f;
        }
    }
}
