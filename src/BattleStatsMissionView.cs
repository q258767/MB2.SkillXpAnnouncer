using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
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
        private ListPanel _list;
        private readonly List<TextWidget> _rows = new List<TextWidget>();
        private Brush _defaultBrush;
        private Brush _playerBrush;
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
                _layer = new GauntletLayer(100, "BattleStats", false);
                _movie = _layer.LoadMovie("BattleStats", _vm) as GauntletMovie;
                _list = FindListPanel(_movie != null ? _movie.RootWidget : null);
                if (_list != null)
                {
                    _defaultBrush = _layer.UIContext.BrushFactory.GetBrush("BattleStatsBrush");
                    _playerBrush = _layer.UIContext.BrushFactory.GetBrush("BattleStatsPlayerBrush");
                }
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
            if (!_initialized || _list == null)
            {
                return;
            }
            try
            {
                MCMSettings cfg = MCMSettings.Instance;
                if (cfg == null)
                {
                    return;
                }
                _list.PositionXOffset = cfg.BattleStatsX;
                _list.PositionYOffset = cfg.BattleStatsY;
                UpdateRows(cfg);
                float opacity = Math.Max(0.1f, cfg.BattleStatsOpacity / 100f);
                if (Math.Abs(opacity - _appliedOpacity) > 0.001f)
                {
                    _appliedOpacity = opacity;
                    GauntletExtensions.SetGlobalAlphaRecursively(_list, opacity);
                }
            }
            catch
            {
            }
        }

        private void UpdateRows(MCMSettings cfg)
        {
            System.Collections.Generic.List<(Hero Hero, string Line)> data = HarmonyPatches.BuildBattleRows();
            int count = data.Count;
            while (_rows.Count < count)
            {
                TextWidget row = new TextWidget(_layer.UIContext);
                row.WidthSizePolicy = SizePolicy.Fixed;
                row.HeightSizePolicy = SizePolicy.CoverChildren;
                _list.AddChild(row);
                _rows.Add(row);
            }
            int fontSize = (int)cfg.BattleStatsFontSize;
            float width = cfg.BattleStatsWidth;
            for (int i = 0; i < _rows.Count; i++)
            {
                if (i < count)
                {
                    _rows[i].IsVisible = true;
                    _rows[i].Brush = HarmonyPatches.IsMainHero(data[i].Hero) ? _playerBrush : _defaultBrush;
                    if (_rows[i].Brush != null)
                    {
                        _rows[i].Brush.FontSize = fontSize;
                    }
                    _rows[i].ScaledSuggestedWidth = width;
                    _rows[i].Text = data[i].Line;
                }
                else
                {
                    _rows[i].IsVisible = false;
                }
            }
        }

        private static ListPanel FindListPanel(Widget root)
        {
            try
            {
                if (root == null)
                {
                    return null;
                }
                if (root is ListPanel)
                {
                    return (ListPanel)root;
                }
                foreach (Widget child in root.AllChildren)
                {
                    if (child is ListPanel)
                    {
                        return (ListPanel)child;
                    }
                }
            }
            catch
            {
            }
            return null;
        }

        public override void OnAgentHit(Agent affectedAgent, Agent affectorAgent, in MissionWeapon affectorWeapon, in Blow blow, in AttackCollisionData collisionData)
        {
            base.OnAgentHit(affectedAgent, affectorAgent, affectorWeapon, blow, collisionData);
            try
            {
                Hero hero = GetHero(affectorAgent);
                if (hero != null && blow.InflictedDamage > 0)
                {
                    HarmonyPatches.AccumulateBattleDamage(hero, blow.InflictedDamage);
                }
            }
            catch
            {
            }
        }

        private static Hero GetHero(Agent agent)
        {
            try
            {
                if (agent != null && agent.Character is CharacterObject characterObject && characterObject.HeroObject != null)
                {
                    return characterObject.HeroObject;
                }
            }
            catch
            {
            }
            return null;
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
                _list = null;
                _rows.Clear();
                _initialized = false;
                HarmonyPatches.ResetBattleStats();
            }
            catch
            {
            }
        }
    }
}
