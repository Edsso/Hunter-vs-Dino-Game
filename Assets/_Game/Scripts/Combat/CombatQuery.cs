using System.Collections.Generic;
using HunterVsDino.Enemies;
using UnityEngine;

namespace HunterVsDino.Combat
{
    /// <summary>Area queries for slashes, blasts and traps. Reuses buffers to avoid garbage every hit.</summary>
    public static class CombatQuery
    {
        private static readonly List<Collider2D> hits = new(256);
        private static ContactFilter2D enemyFilter;
        private static bool filterReady;

        private static ContactFilter2D EnemyFilter
        {
            get
            {
                if (!filterReady)
                {
                    enemyFilter = new ContactFilter2D { useTriggers = false };
                    enemyFilter.SetLayerMask(LayerMask.GetMask("Enemy", "EnemyFlying"));
                    filterReady = true;
                }
                return enemyFilter;
            }
        }

        /// <summary>Fills <paramref name="results"/> with living enemies inside the circle.</summary>
        public static void EnemiesInCircle(Vector2 center, float radius, List<EnemyController> results)
        {
            results.Clear();
            Physics2D.OverlapCircle(center, radius, EnemyFilter, hits);
            for (int i = 0; i < hits.Count; i++)
            {
                if (hits[i].TryGetComponent(out EnemyController e) && !e.Health.IsDead && !results.Contains(e))
                    results.Add(e);
            }
        }

        /// <summary>Damages every enemy in the circle. Returns how many were hit.</summary>
        public static int DamageCircle(Vector2 center, float radius, float damage, List<EnemyController> buffer)
        {
            EnemiesInCircle(center, radius, buffer);
            for (int i = 0; i < buffer.Count; i++) buffer[i].Health.TakeDamage(damage);
            return buffer.Count;
        }
    }
}
