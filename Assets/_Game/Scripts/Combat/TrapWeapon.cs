using System.Collections.Generic;
using UnityEngine;

namespace HunterVsDino.Combat
{
    /// <summary>
    /// Bear trap: every cooldown drops traps around the hunter. A trap snaps on the first dinos
    /// that step in (damage + root for EffectDuration). Extra "projectiles" = more traps per drop,
    /// pierce = how many dinos one trap can catch.
    /// </summary>
    public class TrapWeapon : Weapon
    {
        [SerializeField] private BearTrap trapPrefab;
        [Tooltip("Max traps on the ground from this weapon; the oldest disappears first.")]
        [SerializeField] private int maxActiveTraps = 8;
        [Tooltip("Traps land this far from the hunter.")]
        [SerializeField] private float dropRadius = 1.5f;

        private readonly List<BearTrap> active = new();
        private float nextDropTime;

        protected override void OnInit()
        {
            nextDropTime = Time.time + 1f; // first trap shortly after pickup
        }

        private void Update()
        {
            if (data == null || trapPrefab == null || Time.time < nextDropTime) return;

            active.RemoveAll(t => t == null);
            int count = ProjectileCount;
            for (int i = 0; i < count; i++)
            {
                if (active.Count >= maxActiveTraps)
                {
                    if (active[0] != null) Destroy(active[0].gameObject);
                    active.RemoveAt(0);
                }

                Vector2 pos = Origin + Random.insideUnitCircle.normalized * Random.Range(0.5f, dropRadius);
                var trap = Instantiate(trapPrefab, pos, Quaternion.identity);
                trap.Arm(data, Damage, Area, EffectDuration, Pierce + 1, data.projectileLifetime);
                active.Add(trap);
            }

            nextDropTime = Time.time + Cooldown;
        }

        private void OnDestroy()
        {
            foreach (var t in active) if (t != null) Destroy(t.gameObject);
        }
    }
}
