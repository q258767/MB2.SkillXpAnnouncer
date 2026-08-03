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

        [SettingPropertyBool("{=sxa_enable}Enable Announcer", RequireRestart = false, HintText = "{=sxa_enable_hint}Master switch for the skill/XP announcements.", Order = 0)]
        [SettingPropertyGroup("{=sxa_group_main}Skill XP Announcer", GroupOrder = 1)]
        public bool Enabled { get; set; } = true;

        [SettingPropertyDropdown("{=sxa_scope}Report Scope", RequireRestart = false, HintText = "{=sxa_scope_hint}Which heroes to announce for: player only / party members (player party or other lords in the army) / everyone.", Order = 1)]
        [SettingPropertyGroup("{=sxa_group_main}Skill XP Announcer", GroupOrder = 1)]
        public Dropdown<string> ReportScope { get; set; } = new Dropdown<string>(new[] { "{=sxa_scope_player}Player only", "{=sxa_scope_party}Party members only", "{=sxa_scope_all}All" }, 2);

        [SettingPropertyFloatingInteger("{=sxa_merge}Merge Interval (seconds)", 0.2f, 5f, "0.0", RequireRestart = false, HintText = "{=sxa_merge_hint}Skill XP gains of the same hero and skill within this interval are merged into one message to avoid spam. Default 1 second.", Order = 2)]
        [SettingPropertyGroup("{=sxa_group_main}Skill XP Announcer", GroupOrder = 1)]
        public float MergeIntervalSeconds { get; set; } = 1f;

        [SettingPropertyBool("{=sxa_skillxp}Report Skill XP", RequireRestart = false, HintText = "{=sxa_skillxp_hint}When disabled, skill proficiency XP is not announced.", Order = 3)]
        [SettingPropertyGroup("{=sxa_group_content}Announcement Content", GroupOrder = 2)]
        public bool ReportSkillXp { get; set; } = true;

        [SettingPropertyBool("{=sxa_charxp}Report Character XP", RequireRestart = false, HintText = "{=sxa_charxp_hint}When disabled, character (total) XP is not announced.", Order = 4)]
        [SettingPropertyGroup("{=sxa_group_content}Announcement Content", GroupOrder = 2)]
        public bool ReportCharXp { get; set; } = true;

        [SettingPropertyBool("{=sxa_skill_remain}Show Skill XP Remaining", RequireRestart = false, HintText = "{=sxa_skill_remain_hint}Show remaining XP to the next skill level in parentheses after the skill XP, e.g. +10(990).", Order = 5)]
        [SettingPropertyGroup("{=sxa_group_content}Announcement Content", GroupOrder = 2)]
        public bool ShowSkillXpRemaining { get; set; } = true;

        [SettingPropertyBool("{=sxa_char_remain}Show Character XP Remaining", RequireRestart = false, HintText = "{=sxa_char_remain_hint}Show remaining XP to the next character level in parentheses, e.g. Character XP +300(4500).", Order = 6)]
        [SettingPropertyGroup("{=sxa_group_content}Announcement Content", GroupOrder = 2)]
        public bool ShowCharXpRemaining { get; set; } = true;
    }
}
