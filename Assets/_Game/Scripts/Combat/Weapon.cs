using HunterVsDino.Enemies;
using UnityEngine;

namespace HunterVsDino.Combat
{
    /// <summary>
    /// Auto-firing weapon held by a hunter. Every <c>cooldown</c> seconds it shoots at the nearest
    /// enemy in range. One component per weapon, on a child of the player, so a hunter can carry several.
    /// </summary>
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private WeaponData data;
        [SerializeField] private Projectile projectilePrefab;

        private float nextFireTime;

        public WeaponData Data => data;

        /// <summary>Called when a weapon is given at runtime (level-up choice).</summary>
        public void Init(WeaponData weaponData, Projectile prefab)
        {
            data = weaponData;
            projectilePrefab = prefab;
            nextFireTime = 0f;
        }

        private void Update()
        {
            if (data == null || projectilePrefab == null || Time.time < nextFireTime) return;

            Vector2 origin = transform.position;
            EnemyController target = EnemyRegistry.GetNearest(origin, data.range);
            if (target == null) return; // hold fire (keeps the cooldown ready) until something is in range

            Vector2 aim = (Vector2)target.transform.position - origin;
            Fire(origin, aim.sqrMagnitude > 0.0001f ? aim.normalized : Vector2.up);
            nextFireTime = Time.time + data.cooldown;
        }

        private void Fire(Vector2 origin, Vector2 direction)
        {
            int count = data.projectileCount;
            float step = count > 1 ? data.spreadAngle / (count - 1) : 0f;
            float start = count > 1 ? -data.spreadAngle * 0.5f : 0f;

            for (int i = 0; i < count; i++)
            {
                Vector2 dir = Quaternion.Euler(0f, 0f, start + step * i) * direction;
                SpawnProjectile(origin, dir);
            }
        }

        private void SpawnProjectile(Vector2 origin, Vector2 direction)
        {
            var projectile = Instantiate(projectilePrefab);
            projectile.Launch(data, origin, direction);
        }
    }
}
