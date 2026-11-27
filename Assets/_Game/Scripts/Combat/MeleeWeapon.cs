using System.Collections.Generic;
using HunterVsDino.Enemies;
using UnityEngine;

namespace HunterVsDino.Combat
{
    /// <summary>
    /// Hunting knife: slashes an arc toward the nearest enemy, hitting everything inside
    /// (radius = Area, arc width = spreadAngle). Extra "projectiles" add slashes in other directions.
    /// </summary>
    public class MeleeWeapon : Weapon
    {
        [SerializeField] private FxFlash slashFx;

        private readonly List<EnemyController> buffer = new();
        private float nextAttackTime;

        protected override void OnInit()
        {
            nextAttackTime = 0f;
        }

        private void Update()
        {
            if (data == null || Time.time < nextAttackTime) return;

            Vector2 origin = Origin;
            EnemyController target = EnemyRegistry.GetNearest(origin, Mathf.Max(data.range, Area));
            if (target == null) return;

            Vector2 aim = (Vector2)target.transform.position - origin;
            float baseAngle = Mathf.Atan2(aim.y, aim.x) * Mathf.Rad2Deg;

            int slashes = ProjectileCount;
            for (int i = 0; i < slashes; i++)
                Slash(origin, baseAngle + 360f / slashes * i);

            nextAttackTime = Time.time + Cooldown;
        }

        private void Slash(Vector2 origin, float angleDeg)
        {
            float radius = Area;
            float halfArc = data.spreadAngle * 0.5f;
            float damage = Damage;
            Vector2 forward = new(Mathf.Cos(angleDeg * Mathf.Deg2Rad), Mathf.Sin(angleDeg * Mathf.Deg2Rad));

            CombatQuery.EnemiesInCircle(origin, radius, buffer);
            for (int i = 0; i < buffer.Count; i++)
            {
                Vector2 to = (Vector2)buffer[i].transform.position - origin;
                // Enemies right on top of the hunter always get hit.
                if (to.sqrMagnitude < 0.25f || Vector2.Angle(forward, to) <= halfArc)
                    buffer[i].Health.TakeDamage(damage);
            }

            // Semicircle sprite points up (+Y); rotate so it points along the slash.
            if (slashFx != null) FxFlash.Spawn(slashFx, origin, radius, angleDeg - 90f, data.projectileColor);
        }
    }
}
