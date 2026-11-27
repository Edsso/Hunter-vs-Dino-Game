using System;
using System.Collections.Generic;
using UnityEngine;

namespace HunterVsDino.Progression
{
    public enum StatType
    {
        [Tooltip("+0.2 = +20% weapon damage")] DamagePercent,
        [Tooltip("+0.1 = 10% shorter cooldown")] CooldownReductionPercent,
        [Tooltip("+0.1 = +10% move speed")] MoveSpeedPercent,
        [Tooltip("+20 = +20 max health (also heals 20)")] MaxHealthFlat,
        [Tooltip("+0.3 = +30% pickup radius")] MagnetRadiusPercent,
        [Tooltip("+1 = one more projectile per shot")] ProjectileCount,
        [Tooltip("+1 = goes through one more enemy")] Pierce,
        [Tooltip("+0.2 = +20% projectile speed")] ProjectileSpeedPercent,
        [Tooltip("+30 = heals 30 now (one-off)")] HealFlat,
        // Append new stats at the end only: assets store the enum as a number.
        [Tooltip("+0.15 = +15% area (blasts, slashes, traps)")] AreaPercent,
        [Tooltip("+0.2 = +20% effect duration (traps, roots)")] DurationPercent
    }

    [Serializable]
    public struct StatModifier
    {
        public StatType stat;
        public float value;
    }

    /// <summary>
    /// One option of the level-up screen. Create via Assets > Create > Hunter vs Dino > Upgrade.
    /// Each pick applies its modifiers once; it can be picked up to <see cref="maxLevel"/> times.
    /// </summary>
    [CreateAssetMenu(fileName = "Upgrade", menuName = "Hunter vs Dino/Upgrade")]
    public class UpgradeData : ScriptableObject
    {
        public string displayName = "Upgrade";
        [TextArea] public string description;
        [Tooltip("Placeholder color for the card accent until there are icons.")]
        public Color accentColor = Color.white;
        public Sprite icon;

        [Tooltip("How many times it can be picked. 0 = unlimited (e.g. a heal).")]
        [Min(0)] public int maxLevel = 5;

        [Tooltip("Relative chance of being offered.")]
        [Min(0.01f)] public float weight = 1f;

        public List<StatModifier> modifiers = new();
    }
}
