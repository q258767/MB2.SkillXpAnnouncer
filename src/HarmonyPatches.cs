using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace SkillXpAnnouncer
{
    internal static class HarmonyPatches
    {
        internal sealed class XpState
        {
            public Hero Hero;
            public SkillObject Skill;
            public float BeforeSkillXp;
            public int BeforeTotalXp;
            public bool Valid;
        }

        private enum ReportScopeKind
        {
            PlayerOnly,
            PartyOnly,
            All
        }

        private static readonly Dictionary<Hero, Dictionary<SkillObject, float>> PendingSkillXp = new Dictionary<Hero, Dictionary<SkillObject, float>>();
        private static readonly Dictionary<Hero, int> PendingCharXp = new Dictionary<Hero, int>();
        private static DateTime _windowStart = DateTime.MinValue;

        private static readonly Color SkillXpColor = Color.FromUint(0xFFE8C56B);

        internal static void Prefix(HeroDeveloper __instance, SkillObject skill, bool shouldNotify, out XpState __state)
        {
            __state = new XpState();
            if (!shouldNotify || __instance == null || skill == null)
            {
                return;
            }
            try
            {
                __state.Hero = __instance.Hero;
                __state.Skill = skill;
                __state.BeforeSkillXp = __instance.GetPropertyValue(skill);
                __state.BeforeTotalXp = __instance.TotalXp;
                __state.Valid = __state.Hero != null;
            }
            catch
            {
                __state.Valid = false;
            }
        }

        internal static void Postfix(HeroDeveloper __instance, ref XpState __state)
        {
            if (__state == null || !__state.Valid || __instance == null || !IsEnabled())
            {
                return;
            }
            try
            {
                Hero hero = __state.Hero;
                if (hero == null || !ShouldReport(hero))
                {
                    return;
                }
                float skillDelta = __instance.GetPropertyValue(__state.Skill) - __state.BeforeSkillXp;
                int charDelta = __instance.TotalXp - __state.BeforeTotalXp;
                if (!IsSkillXpEnabled())
                {
                    skillDelta = 0f;
                }
                if (!IsCharXpEnabled())
                {
                    charDelta = 0;
                }
                if (skillDelta == 0f && charDelta == 0)
                {
                    return;
                }

                if (IsInMission())
                {
                    // In battle/mission: merge into the window to avoid spam.
                    DateTime now = DateTime.UtcNow;
                    if (_windowStart != DateTime.MinValue && (now - _windowStart).TotalMilliseconds >= GetMergeIntervalMs() && HasPending())
                    {
                        Flush();
                    }
                    if (_windowStart == DateTime.MinValue)
                    {
                        _windowStart = now;
                    }
                    Accumulate(hero, __state.Skill, skillDelta, charDelta);
                }
                else
                {
                    // Outside battle: show immediately, no merging.
                    Flush();
                    DisplayGains(hero, __state.Skill, skillDelta, charDelta);
                }
            }
            catch
            {
            }
        }

        private static bool IsInMission()
        {
            try
            {
                return Mission.Current != null;
            }
            catch
            {
                return false;
            }
        }

        private static void Accumulate(Hero hero, SkillObject skill, float skillDelta, int charDelta)
        {
            if (skillDelta != 0f)
            {
                if (!PendingSkillXp.TryGetValue(hero, out Dictionary<SkillObject, float> dict))
                {
                    dict = new Dictionary<SkillObject, float>();
                    PendingSkillXp[hero] = dict;
                }
                dict[skill] = dict.TryGetValue(skill, out float v) ? v + skillDelta : skillDelta;
            }
            if (charDelta != 0)
            {
                PendingCharXp[hero] = PendingCharXp.TryGetValue(hero, out int cv) ? cv + charDelta : charDelta;
            }
        }

        private static void DisplayGains(Hero hero, SkillObject skill, float skillDelta, int charDelta)
        {
            try
            {
                if (skillDelta == 0f && charDelta == 0)
                {
                    return;
                }
                List<(string Name, double Amount, int Remaining)> skills = new List<(string Name, double Amount, int Remaining)>();
                if (skillDelta != 0f)
                {
                    skills.Add((skill.Name.ToString(), Math.Round(skillDelta), GetRemainingToNextLevel(hero, skill)));
                }
                InformationManager.DisplayMessage(new InformationMessage(BuildMessage(hero, skills, charDelta, GetCharRemainingToNextLevel(hero)), SkillXpColor));
            }
            catch
            {
            }
        }

        private static bool IsEnabled()
        {
            try
            {
                MCMSettings settings = MCMSettings.Instance;
                return settings == null || settings.Enabled;
            }
            catch
            {
                return true;
            }
        }

        private static bool IsSkillXpEnabled()
        {
            try
            {
                MCMSettings settings = MCMSettings.Instance;
                return settings == null || settings.ReportSkillXp;
            }
            catch
            {
                return true;
            }
        }

        private static bool IsCharXpEnabled()
        {
            try
            {
                MCMSettings settings = MCMSettings.Instance;
                return settings == null || settings.ReportCharXp;
            }
            catch
            {
                return true;
            }
        }

        private static bool IsShowSkillRemainingEnabled()
        {
            try
            {
                MCMSettings settings = MCMSettings.Instance;
                return settings == null || settings.ShowSkillXpRemaining;
            }
            catch
            {
                return true;
            }
        }

        private static bool IsShowCharRemainingEnabled()
        {
            try
            {
                MCMSettings settings = MCMSettings.Instance;
                return settings == null || settings.ShowCharXpRemaining;
            }
            catch
            {
                return true;
            }
        }

        private static int GetCharRemainingToNextLevel(Hero hero)
        {
            try
            {
                if (hero == null || hero.HeroDeveloper == null)
                {
                    return -1;
                }
                int requiredNext = hero.HeroDeveloper.GetXpRequiredForLevel(hero.Level + 1);
                int remaining = requiredNext - hero.HeroDeveloper.TotalXp;
                return remaining < 0 ? 0 : remaining;
            }
            catch
            {
                return -1;
            }
        }

        private static int GetRemainingToNextLevel(Hero hero, SkillObject skill)
        {
            try
            {
                if (hero == null || skill == null || hero.HeroDeveloper == null || Campaign.Current == null || Campaign.Current.Models == null)
                {
                    return -1;
                }
                var model = Campaign.Current.Models.CharacterDevelopmentModel;
                int level = hero.GetSkillValue(skill);
                int requiredCurrent = model.GetXpRequiredForSkillLevel(level);
                int requiredNext = model.GetXpRequiredForSkillLevel(level + 1);
                int progress = hero.HeroDeveloper.GetSkillXpProgress(skill);
                int remaining = requiredNext - requiredCurrent - progress;
                return remaining < 0 ? 0 : remaining;
            }
            catch
            {
                return -1;
            }
        }

        private static double GetMergeIntervalMs()
        {
            try
            {
                MCMSettings settings = MCMSettings.Instance;
                if (settings != null && settings.MergeIntervalSeconds > 0f)
                {
                    return settings.MergeIntervalSeconds * 1000.0;
                }
            }
            catch
            {
            }
            return 1000.0;
        }

        private static ReportScopeKind GetScope()
        {
            try
            {
                MCMSettings settings = MCMSettings.Instance;
                if (settings != null && settings.ReportScope != null)
                {
                    int index = settings.ReportScope.SelectedIndex;
                    if (index == 0)
                    {
                        return ReportScopeKind.PlayerOnly;
                    }
                    if (index == 1)
                    {
                        return ReportScopeKind.PartyOnly;
                    }
                }
            }
            catch
            {
            }
            return ReportScopeKind.All;
        }

        private static bool IsInPlayerParty(Hero hero)
        {
            try
            {
                if (hero.PartyBelongedTo == MobileParty.MainParty)
                {
                    return true;
                }
                Army army = MobileParty.MainParty != null ? MobileParty.MainParty.Army : null;
                return army != null && hero.PartyBelongedTo != null && army.Parties.Contains(hero.PartyBelongedTo);
            }
            catch
            {
                return false;
            }
        }

        private static bool ShouldReport(Hero hero)
        {
            switch (GetScope())
            {
                case ReportScopeKind.PlayerOnly:
                    return hero == Hero.MainHero;
                case ReportScopeKind.PartyOnly:
                    return hero != Hero.MainHero && IsInPlayerParty(hero);
                default:
                    return hero == Hero.MainHero || IsInPlayerParty(hero);
            }
        }

        private static bool HasPending()
        {
            return PendingSkillXp.Count > 0 || PendingCharXp.Count > 0;
        }

        internal static void ClearPending()
        {
            PendingSkillXp.Clear();
            PendingCharXp.Clear();
            _windowStart = DateTime.MinValue;
        }

        internal static void FlushIfWindowElapsed()
        {
            try
            {
                if (_windowStart == DateTime.MinValue || !HasPending())
                {
                    return;
                }
                if ((DateTime.UtcNow - _windowStart).TotalMilliseconds >= GetMergeIntervalMs())
                {
                    Flush();
                }
            }
            catch
            {
            }
        }

        private static void Flush()
        {
            try
            {
                HashSet<Hero> heroes = new HashSet<Hero>();
                foreach (KeyValuePair<Hero, Dictionary<SkillObject, float>> kvp in PendingSkillXp)
                {
                    heroes.Add(kvp.Key);
                }
                foreach (KeyValuePair<Hero, int> kvp in PendingCharXp)
                {
                    heroes.Add(kvp.Key);
                }
                foreach (Hero hero in heroes)
                {
                    if (hero == null)
                    {
                        continue;
                    }
                    List<(string Name, double Amount, int Remaining)> skills = new List<(string Name, double Amount, int Remaining)>();
                    if (PendingSkillXp.TryGetValue(hero, out Dictionary<SkillObject, float> dict))
                    {
                        foreach (KeyValuePair<SkillObject, float> s in dict)
                        {
                            skills.Add((s.Key.Name.ToString(), Math.Round(s.Value), GetRemainingToNextLevel(hero, s.Key)));
                        }
                    }
                    int charXp = PendingCharXp.TryGetValue(hero, out int cv) ? cv : 0;
                    if (skills.Count == 0 && charXp == 0)
                    {
                        continue;
                    }
                    InformationManager.DisplayMessage(new InformationMessage(BuildMessage(hero, skills, charXp, GetCharRemainingToNextLevel(hero)), SkillXpColor));
                }
                PendingSkillXp.Clear();
                PendingCharXp.Clear();
            }
            catch
            {
            }
            _windowStart = DateTime.MinValue;
        }

        private static string BuildMessage(Hero hero, List<(string Name, double Amount, int Remaining)> skills, int charXp, int charRemaining)
        {
            string sep = new TextObject("{=sxa_sep}，").ToString();
            TextObject charLabel = new TextObject("{=sxa_char_label}角色经验");
            StringBuilder parts = new StringBuilder();
            foreach ((string Name, double Amount, int Remaining) s in skills)
            {
                if (parts.Length > 0)
                {
                    parts.Append(sep);
                }
                parts.Append(s.Name);
                parts.Append(s.Amount >= 0.0 ? "+" : string.Empty);
                parts.Append(s.Amount.ToString("0"));
                if (s.Remaining >= 0 && IsShowSkillRemainingEnabled())
                {
                    parts.Append("(").Append(s.Remaining).Append(")");
                }
            }
            if (charXp != 0)
            {
                if (parts.Length > 0)
                {
                    parts.Append(sep);
                }
                parts.Append(charLabel.ToString());
                parts.Append(charXp >= 0 ? "+" : string.Empty);
                parts.Append(charXp.ToString("0"));
                if (charRemaining >= 0 && IsShowCharRemainingEnabled())
                {
                    parts.Append("(").Append(charRemaining).Append(")");
                }
            }
            TextObject msg = new TextObject("{=sxa_gains}{HERO}的{PARTS}。");
            msg.SetTextVariable("HERO", hero.Name.ToString());
            msg.SetTextVariable("PARTS", parts.ToString());
            return msg.ToString();
        }
    }
}
