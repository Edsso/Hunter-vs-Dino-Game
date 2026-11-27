using System.Collections.Generic;
using UnityEngine;

namespace HunterVsDino.Enemies
{
    /// <summary>Every enemy alive in the run. Weapons use it to find targets, the spawner to count.</summary>
    public static class EnemyRegistry
    {
        private static readonly List<EnemyController> enemies = new();

        public static IReadOnlyList<EnemyController> All => enemies;
        public static int Count => enemies.Count;

        public static void Register(EnemyController enemy)
        {
            if (!enemies.Contains(enemy)) enemies.Add(enemy);
        }

        public static void Unregister(EnemyController enemy) => enemies.Remove(enemy);

        /// <summary>Nearest enemy within maxDistance, or null.</summary>
        public static EnemyController GetNearest(Vector2 position, float maxDistance = float.MaxValue)
        {
            EnemyController nearest = null;
            float best = maxDistance * maxDistance;
            for (int i = 0; i < enemies.Count; i++)
            {
                float d = ((Vector2)enemies[i].transform.position - position).sqrMagnitude;
                if (d < best)
                {
                    best = d;
                    nearest = enemies[i];
                }
            }
            return nearest;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics() => enemies.Clear();
    }
}
