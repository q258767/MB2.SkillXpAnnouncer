using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;
using MCM.Common;

namespace SkillXpAnnouncer
{
    public class MCMSettings : AttributeGlobalSettings<MCMSettings>
    {
        public override string Id => "SkillXpAnnouncer";

        public override string DisplayName => "Skill XP Announcer";

        public override string FolderName => "SkillXpAnnouncer";

        public override string FormatType => "json";

        [SettingPropertyBool("{=sxa_enable}Enable Announcer", RequireRestart = false, HintText = "{=sxa_enable_hint}Master switch.", Order = 0, IsToggle = true)]
        [SettingPropertyGroup("{=sxa_group_main}Skill XP Announcer", GroupOrder = 1)]
        public bool Enabled { get; set; } = true;

        [SettingPropertyDropdown("{=sxa_scope}Report Scope", RequireRestart = false, HintText = "{=sxa_scope_hint}Player only, party members, or everyone.", Order = 1)]
        [SettingPropertyGroup("{=sxa_group_main}Skill XP Announcer", GroupOrder = 1)]
        public Dropdown<string> ReportScope { get; set; } = new Dropdown<string>(new[] { "{=sxa_scope_player}Player only", "{=sxa_scope_party}Party members only", "{=sxa_scope_all}All" }, 2);

        [SettingPropertyFloatingInteger("{=sxa_merge}Merge Interval (seconds)", 0.2f, 5f, "0.0", RequireRestart = false, HintText = "{=sxa_merge_hint}Merge gains of the same hero and skill within this interval into one message.", Order = 2)]
        [SettingPropertyGroup("{=sxa_group_main}Skill XP Announcer", GroupOrder = 1)]
        public float MergeIntervalSeconds { get; set; } = 1f;

        [SettingPropertyBool("{=sxa_skillxp}Report Skill XP", RequireRestart = false, HintText = "{=sxa_skillxp_hint}Announce skill proficiency XP.", Order = 3)]
        [SettingPropertyGroup("{=sxa_group_main}Skill XP Announcer", GroupOrder = 1)]
        public bool ReportSkillXp { get; set; } = true;

        [SettingPropertyBool("{=sxa_charxp}Report Character XP", RequireRestart = false, HintText = "{=sxa_charxp_hint}Announce character (total) XP.", Order = 4)]
        [SettingPropertyGroup("{=sxa_group_main}Skill XP Announcer", GroupOrder = 1)]
        public bool ReportCharXp { get; set; } = true;

        [SettingPropertyBool("{=sxa_skill_remain}Show Skill XP Remaining", RequireRestart = false, HintText = "{=sxa_skill_remain_hint}Show remaining XP to the next skill level, e.g. +10(990).", Order = 5)]
        [SettingPropertyGroup("{=sxa_group_main}Skill XP Announcer", GroupOrder = 1)]
        public bool ShowSkillXpRemaining { get; set; } = true;

        [SettingPropertyBool("{=sxa_char_remain}Show Character XP Remaining", RequireRestart = false, HintText = "{=sxa_char_remain_hint}Show remaining XP to the next level, e.g. +300(4500).", Order = 6)]
        [SettingPropertyGroup("{=sxa_group_main}Skill XP Announcer", GroupOrder = 1)]
        public bool ShowCharXpRemaining { get; set; } = true;

        [SettingPropertyBool("{=sxa_battle_stats}Show Battle XP Table", RequireRestart = false, HintText = "{=sxa_battle_stats_hint}Show total skill and character XP gained per hero during the current battle. Default: on.", Order = 0, IsToggle = true)]
        [SettingPropertyGroup("{=sxa_group_battle}Battle Stats", GroupOrder = 3)]
        public bool ShowBattleStats { get; set; } = true;

        [SettingPropertyBool("{=sxa_battle_party}Show Party Members", RequireRestart = false, HintText = "{=sxa_battle_party_hint}Include party members in the table; off shows the player only. Default: on.", Order = 1)]
        [SettingPropertyGroup("{=sxa_group_battle}Battle Stats", GroupOrder = 3)]
        public bool BattleStatsShowParty { get; set; } = true;

        [SettingPropertyFloatingInteger("{=sxa_battle_opacity}Battle Table Opacity (%)", 10f, 100f, "0", RequireRestart = false, HintText = "{=sxa_battle_opacity_hint}Opacity of the battle XP table. Default: 40.", Order = 11)]
        [SettingPropertyGroup("{=sxa_group_battle}Battle Stats", GroupOrder = 3)]
        public float BattleStatsOpacity { get; set; } = 40f;

        [SettingPropertyFloatingInteger("{=sxa_battle_font}Battle Table Font Size", 8f, 40f, "0", RequireRestart = false, HintText = "{=sxa_battle_font_hint}Font size of the table, kept small to avoid blocking the view. Default: 16.", Order = 10)]
        [SettingPropertyGroup("{=sxa_group_battle}Battle Stats", GroupOrder = 3)]
        public float BattleStatsFontSize { get; set; } = 16f;

        [SettingPropertyBool("{=sxa_battle_health}Show Health", RequireRestart = false, HintText = "{=sxa_battle_health_hint}Show the hero's current health after the name, e.g. XXX(100/300). Default: on.", Order = 2)]
        [SettingPropertyGroup("{=sxa_group_battle}Battle Stats", GroupOrder = 3)]
        public bool BattleStatsShowHealth { get; set; } = true;

        [SettingPropertyBool("{=sxa_battle_damage}Show Total Damage", RequireRestart = false, HintText = "{=sxa_battle_damage_hint}Show total damage dealt this battle after the name. Default: on.", Order = 3)]
        [SettingPropertyGroup("{=sxa_group_battle}Battle Stats", GroupOrder = 3)]
        public bool BattleStatsShowDamage { get; set; } = true;

        [SettingPropertyFloatingInteger("{=sxa_battle_width}Table Width", 400f, 3000f, "0", RequireRestart = false, HintText = "{=sxa_battle_width_hint}Width of the table; long lines wrap only when exceeding this. Default: 1200.", Order = 9)]
        [SettingPropertyGroup("{=sxa_group_battle}Battle Stats", GroupOrder = 3)]
        public float BattleStatsWidth { get; set; } = 1200f;

        [SettingPropertyFloatingInteger("{=sxa_battle_max_rows}Max Rows", 1f, 50f, "0", RequireRestart = false, HintText = "{=sxa_battle_max_rows_hint}Maximum number of hero rows shown; extra rows are hidden. Default: 10.", Order = 5)]
        [SettingPropertyGroup("{=sxa_group_battle}Battle Stats", GroupOrder = 3)]
        public float MaxRows { get; set; } = 10f;

        [SettingPropertyBool("{=sxa_battle_sort_dmg}Sort by Total Damage", RequireRestart = false, HintText = "{=sxa_battle_sort_dmg_hint}Sort rows by total damage dealt (highest first). Default: off.", Order = 6)]
        [SettingPropertyGroup("{=sxa_group_battle}Battle Stats", GroupOrder = 3)]
        public bool SortByDamage { get; set; } = false;

        [SettingPropertyBool("{=sxa_battle_pin_player}Pin Player First", RequireRestart = false, HintText = "{=sxa_battle_pin_player_hint}Always keep the player's row first (takes priority over sorting). Default: on.", Order = 4)]
        [SettingPropertyGroup("{=sxa_group_battle}Battle Stats", GroupOrder = 3)]
        public bool PinPlayer { get; set; } = true;

        [SettingPropertyFloatingInteger("{=sxa_battle_x}Battle Table X", 0f, 2000f, "0", RequireRestart = false, HintText = "{=sxa_battle_x_hint}Horizontal offset of the table. Default: 400.", Order = 7)]
        [SettingPropertyGroup("{=sxa_group_battle}Battle Stats", GroupOrder = 3)]
        public float BattleStatsX { get; set; } = 400f;

        [SettingPropertyFloatingInteger("{=sxa_battle_y}Battle Table Y", 0f, 1200f, "0", RequireRestart = false, HintText = "{=sxa_battle_y_hint}Vertical offset of the table. Default: 100.", Order = 8)]
        [SettingPropertyGroup("{=sxa_group_battle}Battle Stats", GroupOrder = 3)]
        public float BattleStatsY { get; set; } = 100f;

        [SettingPropertyBool("{=sxa_debug_log}Debug Info", RequireRestart = false, HintText = "{=sxa_debug_log_hint}Output debug information (SXA rows) to the game log.", Order = 0)]
        [SettingPropertyGroup("{=sxa_group_general}General", GroupOrder = 4)]
        public bool DebugLog { get; set; } = false;
    }
}
