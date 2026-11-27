using HunterVsDino.Enemies;
using HunterVsDino.Players;
using UnityEngine;

namespace HunterVsDino.Combat
{
    /// <summary>Final numbers for one shot: weapon data combined with the owner's upgrades.</summary>
    public struct ShotStats
    {
        public float damage;
        public float speed;
        public int pierce;
        public float lifetime;
    }

    /// <summary>
    /// Auto-firing weapon held by a hunter. Every <c>cooldown</c> seconds it shoots at the nearest
    /// enemy in range. One component per weapon, on a child of the player, so a hunter can carry several.
    /// Upgrades come from the owner's <see cref="PlayerStats"/>; the WeaponData asset is never changed.
    /// </summary>
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private WeaponData data;
        [SerializeField] private Projectile projectilePrefab;

        private PlayerStats stats;
        private float nextFireTime;

        public WeaponData Data => data;

        private void Awake()
        {
            stats = GetComponentInParent<PlayerStats>();
        }

        /// <summary>Called when a weapon is given at runtime (level-up choice).</summary>
        public void Init(WeaponData weaponData, Projectile prefab)
        {
            data = weaponData;
            projectilePrefab = prefab;
            nextFireTime = 0f;
        }

        private float Cooldown => data.cooldown * (stats != null ? stats.CooldownMultiplier : 1f);
        private int ProjectileCount => data.projectileCount + (stats != null ? stats.ExtraProjectiles : 0);

        private ShotStats CurrentShot => new()
        {
            damage = data.damage * (stats != null ? stats.DamageMultiplier : 1f),
            speed = data.projectileSpeed * (stats != null ? stats.ProjectileSpeedMultiplier : 1f),
            pierce = data.pierce + (stats != null ? stats.ExtraPierce : 0),
            lifetime = data.projectileLifetime
        };

        private void Update()
        {
            if (data == null || projectilePrefab == null || Time.time < nextFireTime) return;

            Vector2 origin = transform.position;
            EnemyController target = EnemyRegistry.GetNearest(origin, data.range);
            if (target == null) return; // hold fire (keeps the cooldown ready) until something is in range

            Vector2 aim = (Vector2)target.transform.position - origin;
            Fire(origin, aim.sqrMagnitude > 0.0001f ? aim.normalized : Vector2.up);
            nextFireTime = Time.time + Cooldown;
        }

        private void Fire(Vector2 origin, Vector2 direction)
        {
            int count = ProjectileCount;
            // Spread widens a little with each extra projectile so they don't overlap.
            float spread = count > 1 ? Mathf.Max(data.spreadAngle, 8f * (count - 1)) : 0f;
            float step = count > 1 ? spread / (count - 1) : 0f;
            float start = -spread * 0.5f;
            ShotStats shot = CurrentShot;

            for (int i = 0; i < count; i++)
            {
                Vector2 dir = Quaternion.Euler(0f, 0f, start + step * i) * direction;
                SpawnProjectile(origin, dir, shot);
            }
        }

        private void SpawnProjectile(Vector2 origin, Vector2 direction, ShotStats shot)
        {
            var projectile = Instantiate(projectilePrefab);
            projectile.Launch(data, shot, origin, direction);
        }
    }
}
