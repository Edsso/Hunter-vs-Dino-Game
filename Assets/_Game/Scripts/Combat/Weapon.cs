using HunterVsDino.Players;
using HunterVsDino.Progression;
using UnityEngine;

namespace HunterVsDino.Combat
{
    /// <summary>Final numbers for one shot: weapon data + weapon level + the owner's upgrades.</summary>
    public struct ShotStats
    {
        public float damage;
        public float speed;
        public int pierce;
        public float lifetime;
    }

    /// <summary>
    /// Base of every weapon a hunter carries. Holds the data, the weapon's own level bonuses and the
    /// owner's <see cref="PlayerStats"/>, and exposes the combined numbers. Subclasses only decide how
    /// to attack (shoot, slash, throw...). Spawned and leveled by <see cref="PlayerWeapons"/>.
    /// </summary>
    public abstract class Weapon : MonoBehaviour
    {
        [SerializeField] protected WeaponData data;

        protected PlayerStats ownerStats;

        // Bonuses from this weapon's own levels.
        private float damageBonus;
        private float cooldownReduction;
        private float speedBonus;
        private int extraProjectiles;
        private int extraPierce;
        private float areaBonus;
        private float durationBonus;

        public WeaponData Data => data;
        public int Level { get; private set; } = 1;
        public bool IsMaxLevel => data == null || Level >= data.MaxLevel;

        public void Init(WeaponData weaponData, PlayerStats owner)
        {
            data = weaponData;
            ownerStats = owner;
            Level = 1;
            damageBonus = cooldownReduction = speedBonus = areaBonus = durationBonus = 0f;
            extraProjectiles = extraPierce = 0;
            OnInit();
        }

        /// <summary>Apply the next level's bonuses. Returns false if already at max.</summary>
        public bool LevelUp()
        {
            if (IsMaxLevel) return false;
            foreach (var mod in data.levels[Level - 1].modifiers) ApplyLevelModifier(mod);
            Level++;
            return true;
        }

        private void ApplyLevelModifier(StatModifier mod)
        {
            switch (mod.stat)
            {
                case StatType.DamagePercent: damageBonus += mod.value; break;
                case StatType.CooldownReductionPercent: cooldownReduction += mod.value; break;
                case StatType.ProjectileSpeedPercent: speedBonus += mod.value; break;
                case StatType.ProjectileCount: extraProjectiles += Mathf.RoundToInt(mod.value); break;
                case StatType.Pierce: extraPierce += Mathf.RoundToInt(mod.value); break;
                case StatType.AreaPercent: areaBonus += mod.value; break;
                case StatType.DurationPercent: durationBonus += mod.value; break;
                default: Debug.LogWarning($"{data.displayName}: stat {mod.stat} has no effect on a weapon level.", this); break;
            }
        }

        protected virtual void OnInit() { }

        // ---- Combined stats (weapon base × weapon level × player upgrades) ----
        private bool HasOwner => ownerStats != null;

        protected float Damage => data.damage * (1f + damageBonus) * (HasOwner ? ownerStats.DamageMultiplier : 1f);
        protected float Cooldown => data.cooldown * Mathf.Max(0.2f, 1f - cooldownReduction) * (HasOwner ? ownerStats.CooldownMultiplier : 1f);
        protected float ProjectileSpeed => data.projectileSpeed * (1f + speedBonus) * (HasOwner ? ownerStats.ProjectileSpeedMultiplier : 1f);
        protected int ProjectileCount => data.projectileCount + extraProjectiles + (HasOwner ? ownerStats.ExtraProjectiles : 0);
        protected int Pierce => data.pierce + extraPierce + (HasOwner ? ownerStats.ExtraPierce : 0);
        protected float Area => data.area * (1f + areaBonus) * (HasOwner ? ownerStats.AreaMultiplier : 1f);
        protected float EffectDuration => data.effectDuration * (1f + durationBonus) * (HasOwner ? ownerStats.DurationMultiplier : 1f);

        /// <summary>Position the weapon attacks from (the hunter).</summary>
        protected Vector2 Origin => transform.position;

        protected ShotStats CurrentShot => new()
        {
            damage = Damage,
            speed = ProjectileSpeed,
            pierce = Pierce,
            lifetime = data.projectileLifetime
        };
    }
}
