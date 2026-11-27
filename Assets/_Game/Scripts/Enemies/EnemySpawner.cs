using System.Collections.Generic;
using HunterVsDino.Core;
using HunterVsDino.Players;
using UnityEngine;

namespace HunterVsDino.Enemies
{
    /// <summary>
    /// Spawns enemies just outside the camera view, following a <see cref="WaveData"/> schedule
    /// driven by the run clock. All spawning goes through <see cref="Spawn"/> so pooling can plug in later.
    /// </summary>
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private WaveData waves;
        [SerializeField] private EnemyController enemyPrefab;
        [SerializeField] private RunTimer runTimer;

        [Tooltip("Extra distance beyond the camera edge where enemies appear.")]
        [SerializeField] private float offscreenMargin = 2f;

        // Fractional spawn "credit" per entry: rate * dt accumulates until it reaches 1 group.
        private readonly List<float> credits = new();
        private Camera cam;

        public WaveData Waves => waves;

        private void Awake()
        {
            cam = Camera.main;
        }

        private void Update()
        {
            if (waves == null || enemyPrefab == null || runTimer == null || !runTimer.IsRunning) return;
            if (PlayerRegistry.All.Count == 0) return;

            while (credits.Count < waves.entries.Count) credits.Add(0f);

            float time = runTimer.Elapsed;
            for (int i = 0; i < waves.entries.Count; i++)
            {
                var entry = waves.entries[i];
                if (entry.enemy == null || !entry.IsActive(time)) continue;

                credits[i] += entry.RateAt(time) * Time.deltaTime;
                while (credits[i] >= 1f)
                {
                    credits[i] -= 1f;
                    SpawnGroup(entry);
                }
            }
        }

        private void SpawnGroup(WaveData.SpawnEntry entry)
        {
            // Pick a random hunter so in co-op every player gets pressure.
            var players = PlayerRegistry.All;
            Vector2 center = players[Random.Range(0, players.Count)].transform.position;
            Vector2 groupPos = center + Random.insideUnitCircle.normalized * SpawnRadius();

            for (int n = 0; n < entry.groupSize; n++)
            {
                if (EnemyRegistry.Count >= waves.maxAlive) return;
                Spawn(entry.enemy, groupPos + Random.insideUnitCircle * entry.groupSpread);
            }
        }

        public EnemyController Spawn(EnemyData data, Vector2 position)
        {
            var enemy = Instantiate(enemyPrefab, position, Quaternion.identity, transform);
            enemy.Init(data, position);
            return enemy;
        }

        /// <summary>Half the camera diagonal plus a margin: always just outside the view.</summary>
        private float SpawnRadius()
        {
            if (cam == null || !cam.orthographic) return 15f;
            float halfH = cam.orthographicSize;
            float halfW = halfH * cam.aspect;
            return Mathf.Sqrt(halfW * halfW + halfH * halfH) + offscreenMargin;
        }
    }
}
