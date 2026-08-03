using System;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.Screens;

namespace SkillXpAnnouncer
{
    public class SubModule : MBSubModuleBase
    {
        private bool _initialized;

        public override void OnMissionBehaviorInitialize(Mission mission)
        {
            base.OnMissionBehaviorInitialize(mission);
            try
            {
                if (Game.Current != null && Game.Current.GameStateManager != null && Game.Current.GameStateManager.ActiveState is MissionState missionState && missionState.Handler is MissionScreen missionScreen)
                {
                    missionScreen.AddMissionView(new BattleStatsMissionView());
                }
            }
            catch (Exception ex)
            {
                FileLog.Log("SkillXpAnnouncer: failed to add battle stats view. " + ex);
            }
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);
            try
            {
                HarmonyPatches.ClearPending();
            }
            catch (Exception ex)
            {
                FileLog.Log("SkillXpAnnouncer: failed to clear pending on game start. " + ex);
            }
        }

        protected override void OnApplicationTick(float dt)
        {
            base.OnApplicationTick(dt);
            if (Campaign.Current != null)
            {
                HarmonyPatches.FlushIfWindowElapsed();
            }
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();
            if (_initialized)
            {
                return;
            }
            _initialized = true;
            try
            {
                Harmony harmony = new Harmony("SkillXpAnnouncer");
                harmony.Patch(
                    AccessTools.Method(typeof(HeroDeveloper), "AddSkillXp", new[] { typeof(SkillObject), typeof(float), typeof(bool), typeof(bool) }),
                    prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.Prefix)),
                    postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.Postfix)));
            }
            catch (Exception ex)
            {
                FileLog.Log("SkillXpAnnouncer: Harmony patch failed. " + ex);
            }
        }
    }
}
