using HunterVsDino.Enemies;
using UnityEngine;

namespace HunterVsDino.Combat
{
    /// <summary>
    /// Shoots projectiles at the nearest enemy in range every cooldown (rifle, shotgun, harpoon...).
    /// Several projectiles fan out around the aim direction.
    /// </summary>
    public class ProjectileWeapon : Weapon
    {
        [SerializeField] private Projectile projectilePrefab;

        private float nextFireTime;

        protected override void OnInit()
        {
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
                var projectile = Instantiate(projectilePrefab);
                projectile.Launch(data, shot, origin, dir);
            }
        }
    }
}
