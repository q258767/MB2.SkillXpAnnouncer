using System;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace SkillXpAnnouncer
{
    public class BattleStatsMissionView : MissionView
    {
        private GauntletLayer _layer;
        private GauntletMovie _movie;
        private BattleStatsVM _vm;
        private bool _initialized;
        private float _appliedOpacity = -1f;

        public override void OnMissionScreenInitialize()
        {
            base.OnMissionScreenInitialize();
            try
            {
                if (MissionScreen == null)
                {
                    return;
                }
                MCMSettings cfg = MCMSettings.Instance;
                if (cfg == null || !cfg.ShowBattleStats)
                {
                    return;
                }
                HarmonyPatches.ResetBattleStats();
                _vm = new BattleStatsVM();
                _vm.Refresh(cfg);
                _layer = new GauntletLayer(100, "BattleStats", false);
                _movie = _layer.LoadMovie("BattleStats", _vm) as GauntletMovie;
                MissionScreen.AddLayer(_layer);
                _initialized = true;
            }
            catch
            {
            }
        }

        public override void OnMissionScreenTick(float dt)
        {
            base.OnMissionScreenTick(dt);
            if (!_initialized || _vm == null)
            {
                return;
            }
            try
            {
                MCMSettings cfg = MCMSettings.Instance;
                _vm.Refresh(cfg);
                float opacity = 0.7f;
                if (cfg != null)
                {
                    opacity = Math.Max(0.1f, cfg.BattleStatsOpacity / 100f);
                }
                if (Math.Abs(opacity - _appliedOpacity) > 0.001f)
                {
                    _appliedOpacity = opacity;
                    if (_movie != null && _movie.RootWidget != null)
                    {
                        TaleWorlds.GauntletUI.GauntletExtensions.SetGlobalAlphaRecursively(_movie.RootWidget, opacity);
                    }
                }
            }
            catch
            {
            }
        }

        public override void OnMissionScreenFinalize()
        {
            base.OnMissionScreenFinalize();
            try
            {
                if (_layer != null && MissionScreen != null)
                {
                    if (_movie != null)
                    {
                        _layer.ReleaseMovie(_movie);
                    }
                    MissionScreen.RemoveLayer(_layer);
                }
                _layer = null;
                _movie = null;
                _vm = null;
                _initialized = false;
                HarmonyPatches.ResetBattleStats();
            }
            catch
            {
            }
        }
    }
}
