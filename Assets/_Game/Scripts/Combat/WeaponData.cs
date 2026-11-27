using System;
using System.Collections.Generic;
using HunterVsDino.Progression;
using UnityEngine;

namespace HunterVsDino.Combat
{
    /// <summary>
    /// Balance data for a weapon. Create via Assets > Create > Hunter vs Dino > Weapon.
    /// <see cref="weaponPrefab"/> decides HOW it attacks (projectile, melee, thrown...);
    /// this asset decides the numbers. Level bonuses and player upgrades are applied on a
    /// runtime copy inside the Weapon component; the asset itself is never changed.
    /// </summary>
    [CreateAssetMenu(fileName = "Weapon", menuName = "Hunter vs Dino/Weapon")]
    public class WeaponData : ScriptableObject
    {
        [Serializable]
        public class WeaponLevel
        {
            [Tooltip("Shown on the level-up card, e.g. '+1 projétil'.")]
            public string description;
            public List<StatModifier> modifiers = new();
        }

        [Header("Identity")]
        public string displayName = "Weapon";
        [TextArea] public string description;
        public Color accentColor = Color.white;
        public Sprite icon;

        [Header("Behaviour")]
        [Tooltip("Prefab with a Weapon component (e.g. ProjectileWeapon) that is spawned on the hunter.")]
        public Weapon weaponPrefab;
        [Tooltip("Relative chance of being offered on level up.")]
        [Min(0.01f)] public float weight = 1f;

        [Header("Firing")]
        [Min(0.05f)] public float cooldown = 1f;
        [Tooltip("Only fires when an enemy is closer than this.")]
        [Min(0.5f)] public float range = 9f;
        [Min(1)] public int projectileCount = 1;
        [Tooltip("Total spread angle when firing several projectiles.")]
        [Range(0f, 180f)] public float spreadAngle = 10f;

        [Header("Projectile")]
        [Min(0f)] public float damage = 5f;
        [Min(0.1f)] public float projectileSpeed = 14f;
        [Min(0.1f)] public float projectileLifetime = 1.5f;
        [Tooltip("How many extra enemies a projectile goes through. 0 = stops at the first.")]
        [Min(0)] public int pierce;

        [Header("Look (placeholder until final art)")]
        public Sprite projectileSprite;
        public Color projectileColor = Color.yellow;
        [Min(0.05f)] public float projectileSize = 0.25f;

        [Header("Levels")]
        [Tooltip("Bonus gained at each level after the first. Level 2 = element 0.")]
        public List<WeaponLevel> levels = new();

        public int MaxLevel => levels.Count + 1;
    }
}
