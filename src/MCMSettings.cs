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

        [SettingPropertyBool("{=sxa_enable}Enable Announcer", RequireRestart = false, HintText = "{=sxa_enable_hint}Master switch.", Order = 0)]
        [SettingPropertyGroup("{=sxa_group_main}Skill XP Announcer", GroupOrder = 1)]
        public bool Enabled { get; set; } = true;

        [SettingPropertyDropdown("{=sxa_scope}Report Scope", RequireRestart = false, HintText = "{=sxa_scope_hint}Player only, party members, or everyone.", Order = 1)]
        [SettingPropertyGroup("{=sxa_group_main}Skill XP Announcer", GroupOrder = 1)]
        public Dropdown<string> ReportScope { get; set; } = new Dropdown<string>(new[] { "{=sxa_scope_player}Player only", "{=sxa_scope_party}Party members only", "{=sxa_scope_all}All" }, 2);

        [SettingPropertyFloatingInteger("{=sxa_merge}Merge Interval (seconds)", 0.2f, 5f, "0.0", RequireRestart = false, HintText = "{=sxa_merge_hint}Merge gains of the same hero and skill within this interval into one message.", Order = 2)]
        [SettingPropertyGroup("{=sxa_group_main}Skill XP Announcer", GroupOrder = 1)]
        public float MergeIntervalSeconds { get; set; } = 1f;

        [SettingPropertyBool("{=sxa_skillxp}Report Skill XP", RequireRestart = false, HintText = "{=sxa_skillxp_hint}Announce skill proficiency XP.", Order = 3)]
        [SettingPropertyGroup("{=sxa_group_content}Announcement Content", GroupOrder = 2)]
        public bool ReportSkillXp { get; set; } = true;

        [SettingPropertyBool("{=sxa_charxp}Report Character XP", RequireRestart = false, HintText = "{=sxa_charxp_hint}Announce character (total) XP.", Order = 4)]
        [SettingPropertyGroup("{=sxa_group_content}Announcement Content", GroupOrder = 2)]
        public bool ReportCharXp { get; set; } = true;

        [SettingPropertyBool("{=sxa_skill_remain}Show Skill XP Remaining", RequireRestart = false, HintText = "{=sxa_skill_remain_hint}Show remaining XP to the next skill level, e.g. +10(990).", Order = 5)]
        [SettingPropertyGroup("{=sxa_group_content}Announcement Content", GroupOrder = 2)]
        public bool ShowSkillXpRemaining { get; set; } = true;

        [SettingPropertyBool("{=sxa_char_remain}Show Character XP Remaining", RequireRestart = false, HintText = "{=sxa_char_remain_hint}Show remaining XP to the next level, e.g. +300(4500).", Order = 6)]
        [SettingPropertyGroup("{=sxa_group_content}Announcement Content", GroupOrder = 2)]
        public bool ShowCharXpRemaining { get; set; } = true;
    }
}
