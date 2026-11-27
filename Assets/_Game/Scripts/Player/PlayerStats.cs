using HunterVsDino.Combat;
using HunterVsDino.Progression;
using UnityEngine;

namespace HunterVsDino.Players
{
    /// <summary>
    /// A hunter's bonuses from upgrades. Weapons read the combat bonuses when they fire;
    /// movement, health and pickup bonuses are pushed into their components when they change.
    /// Base values stay in the prefab/assets, so balancing data is never overwritten at runtime.
    /// </summary>
    public class PlayerStats : MonoBehaviour
    {
        public float DamageMultiplier { get; private set; } = 1f;
        public float CooldownMultiplier => Mathf.Max(0.2f, 1f - cooldownReduction);
        public float ProjectileSpeedMultiplier { get; private set; } = 1f;
        public int ExtraProjectiles { get; private set; }
        public int ExtraPierce { get; private set; }

        private float cooldownReduction;
        private float moveSpeedBonus;
        private float magnetBonus;

        private float baseMoveSpeed;
        private float baseMagnetRadius;

        private PlayerMotor motor;
        private Health health;
        private PlayerPickupCollector collector;

        private void Awake()
        {
            motor = GetComponent<PlayerMotor>();
            health = GetComponent<Health>();
            collector = GetComponent<PlayerPickupCollector>();
            if (motor != null) baseMoveSpeed = motor.MoveSpeed;
            if (collector != null) baseMagnetRadius = collector.MagnetRadius;
        }

        public void Apply(StatModifier mod)
        {
            switch (mod.stat)
            {
                case StatType.DamagePercent: DamageMultiplier += mod.value; break;
                case StatType.CooldownReductionPercent: cooldownReduction += mod.value; break;
                case StatType.ProjectileSpeedPercent: ProjectileSpeedMultiplier += mod.value; break;
                case StatType.ProjectileCount: ExtraProjectiles += Mathf.RoundToInt(mod.value); break;
                case StatType.Pierce: ExtraPierce += Mathf.RoundToInt(mod.value); break;
                case StatType.MoveSpeedPercent:
                    moveSpeedBonus += mod.value;
                    if (motor != null) motor.MoveSpeed = baseMoveSpeed * (1f + moveSpeedBonus);
                    break;
                case StatType.MagnetRadiusPercent:
                    magnetBonus += mod.value;
                    if (collector != null) collector.MagnetRadius = baseMagnetRadius * (1f + magnetBonus);
                    break;
                case StatType.MaxHealthFlat:
                    if (health != null) health.SetMax(health.Max + mod.value, healByDifference: true);
                    break;
                case StatType.HealFlat:
                    if (health != null) health.Heal(mod.value);
                    break;
            }
        }
    }
}
