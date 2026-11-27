using HunterVsDino.Combat;
using UnityEngine;

namespace HunterVsDino.Progression
{
    public enum UpgradeKind
    {
        Passive,
        NewWeapon,
        WeaponLevel
    }

    /// <summary>One option on the level-up screen: a passive upgrade, a new weapon, or a weapon level.</summary>
    public class UpgradeChoice
    {
        public UpgradeKind Kind { get; private set; }
        public UpgradeData Passive { get; private set; }
        public WeaponData Weapon { get; private set; }

        public string Title { get; private set; }
        public string Description { get; private set; }
        public string LevelText { get; private set; }
        public Color Accent { get; private set; }
        public float Weight { get; private set; }

        public static UpgradeChoice ForPassive(UpgradeData u, int currentLevel) => new()
        {
            Kind = UpgradeKind.Passive,
            Passive = u,
            Title = u.displayName,
            Description = u.description,
            LevelText = currentLevel == 0 ? "NOVO" : u.maxLevel > 0 ? $"Nv {currentLevel} » {currentLevel + 1}" : "",
            Accent = u.accentColor,
            Weight = u.weight
        };

        public static UpgradeChoice ForNewWeapon(WeaponData w) => new()
        {
            Kind = UpgradeKind.NewWeapon,
            Weapon = w,
            Title = w.displayName,
            Description = w.description,
            LevelText = "NOVA ARMA",
            Accent = w.accentColor,
            Weight = w.weight
        };

        public static UpgradeChoice ForWeaponLevel(WeaponData w, int currentLevel) => new()
        {
            Kind = UpgradeKind.WeaponLevel,
            Weapon = w,
            Title = w.displayName,
            Description = w.levels[currentLevel - 1].description,
            LevelText = $"Arma Nv {currentLevel} » {currentLevel + 1}",
            Accent = w.accentColor,
            // Leveling what you already carry is offered a bit more often, like in most survivors-likes.
            Weight = w.weight * 1.5f
        };
    }
}
