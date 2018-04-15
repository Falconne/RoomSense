using HugsLib.Settings;
using HugsLib.Utils;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace RoomSense
{
    [DefOf]
    public static class RoomSenseKeyBingings
    {
        public static KeyBindingDef ToggleRoomSenseOverlay;
    }

    public class Main : HugsLib.ModBase
    {

        internal new ModLogger Logger => base.Logger;

        internal static Main Instance { get; private set; }

        public override string ModIdentifier => "RoomSense";

        public bool ShowOverlay = false;

        private SettingHandle<int> _graphOpacity;

        private SettingHandle<int> _heatMapOpacity;

        private SettingHandle<bool> _showStatMeters;

        private SettingHandle<bool> _showHeatMap;

        private SettingHandle<bool> _showRoomLabels;

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
            if (!ShowOverlay || (!_showStatMeters && !_showHeatMap))
            {
                _infoCollector.Reset();
                _heatMap?.Reset();
                return;
            }

            _infoCollector.Update(_updateDelay);
            if (!_showHeatMap)
                return;

            if (_heatMap == null)
                _heatMap = new HeatMap(_infoCollector);
            _heatMap.Update();
        }

        public override void OnGUI()
        {
            if (Current.ProgramState != ProgramState.Playing || Find.VisibleMap == null
                || WorldRendererUtility.WorldRenderedNow)
            {
                return;
            }

            if (Event.current.type == EventType.KeyDown && Event.current.keyCode != KeyCode.None)
            {

                if (RoomSenseKeyBingings.ToggleRoomSenseOverlay.JustPressed)
                {
                    if (WorldRendererUtility.WorldRenderedNow)
                    {
                        return;
                    }

                    ShowOverlay = !ShowOverlay;
                }
            }

            if (!ShowOverlay)
                return;

            if (!_showStatMeters)
                return;

            if (_firstRun)
            {
                _firstRun = false;
                _graphOpacityAsFloat = _graphOpacity / 100f;
            }
            _graphOverlay.OnGUI(_infoCollector, _graphOpacityAsFloat, _showRoomLabels);
        }

        public override void WorldLoaded()
        {
            _infoCollector.Reset();
        }

        public override void DefsLoaded()
        {
            _showStatMeters = Settings.GetHandle(
                "showStatMeters", "FALCRS.ShowStatMeters".Translate(),
                "FALCRS.ShowStatMetersDesc".Translate(), true);

            _showHeatMap = Settings.GetHandle(
                "showRSHeatMap", "FALCRS.ShowHeatMap".Translate(),
                "FALCRS.ShowHeatMapDesc".Translate(), true);

            _graphOpacity = Settings.GetHandle(
                "graphOpacity", "FALCRS.GraphOpacity".Translate(),
                "FALCRS.GraphOpacityDesc".Translate(), 100,
                Validators.IntRangeValidator(1, 100));

            _graphOpacity.OnValueChanged = val => _graphOpacityAsFloat = _graphOpacity / 100f;

            _heatMapOpacity = Settings.GetHandle(
                "rsHeatMapOpacity", "FALCRS.HeatMapOpacity".Translate(),
                "FALCRS.HeatMapOpacityDesc".Translate(), 44,
                Validators.IntRangeValidator(1, 100));

            _heatMapOpacity.OnValueChanged = val => _heatMap.Reset();

            _updateDelay = Settings.GetHandle("updateDelay", "FALCRS.UpdateDelay".Translate(),
                "FALCRS.UpdateDelayDesc".Translate(),
                100, Validators.IntRangeValidator(1, 9999));

            _showRoomLabels = Settings.GetHandle(
                "showRoomLabels", "FALCRS.ShowRoomLabels".Translate(),
                "FALCRS.ShowRoomLabelsDesc".Translate(), true);

            _updateDelay.OnValueChanged = val => _infoCollector.Reset();
        }

        public float GetHeatMapOpacity()
        {
            return _heatMapOpacity / 100f;
        }
    }
}
