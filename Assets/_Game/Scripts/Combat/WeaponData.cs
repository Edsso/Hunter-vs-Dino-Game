using UnityEngine;

namespace HunterVsDino.Combat
{
    /// <summary>
    /// Balance data for a projectile weapon. Create via Assets > Create > Hunter vs Dino > Weapon.
    /// Upgrades (step 7) will modify a runtime copy of these numbers, never the asset itself.
    /// </summary>
    [CreateAssetMenu(fileName = "Weapon", menuName = "Hunter vs Dino/Weapon")]
    public class WeaponData : ScriptableObject
    {
        [Header("Identity")]
        public string displayName = "Weapon";
        [TextArea] public string description;

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
    }
}
