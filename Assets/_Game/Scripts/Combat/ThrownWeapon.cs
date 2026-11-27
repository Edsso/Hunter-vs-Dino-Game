using System.Collections.Generic;
using HunterVsDino.Enemies;
using UnityEngine;

namespace HunterVsDino.Combat
{
    /// <summary>
    /// Grenade: lobs explosives at random enemies in range; each one lands after a short flight
    /// and damages everything within Area. Extra "projectiles" = more grenades per throw.
    /// </summary>
    public class ThrownWeapon : Weapon
    {
        [SerializeField] private ThrownExplosive explosivePrefab;
        [SerializeField] private FxFlash blastFx;
        [SerializeField] private float flightTime = 0.5f;

        private readonly List<EnemyController> inRange = new();
        private float nextThrowTime;

        protected override void OnInit()
        {
            nextThrowTime = 0f;
        }

        private void Update()
        {
            if (data == null || explosivePrefab == null || Time.time < nextThrowTime) return;

            Vector2 origin = Origin;
            CollectInRange(origin);
            if (inRange.Count == 0) return;

            int count = ProjectileCount;
            for (int i = 0; i < count; i++)
            {
                var target = inRange[Random.Range(0, inRange.Count)];
                // Small scatter so several grenades don't land on the exact same spot.
                Vector2 landing = (Vector2)target.transform.position + (i == 0 ? Vector2.zero : Random.insideUnitCircle * Area);
                var bomb = Instantiate(explosivePrefab, origin, Quaternion.identity);
                bomb.Launch(origin, landing, flightTime, Damage, Area, data, blastFx);
            }

            nextThrowTime = Time.time + Cooldown;
        }

        private void CollectInRange(Vector2 origin)
        {
            inRange.Clear();
            float r2 = data.range * data.range;
            var all = EnemyRegistry.All;
            for (int i = 0; i < all.Count; i++)
                if (((Vector2)all[i].transform.position - origin).sqrMagnitude <= r2) inRange.Add(all[i]);
        }
    }
}
