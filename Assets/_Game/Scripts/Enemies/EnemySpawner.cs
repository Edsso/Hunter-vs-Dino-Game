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

        /// <summary>Normal waves stop (e.g. when the boss arrives).</summary>
        public bool WavesEnabled { get; set; } = true;

        /// <summary>0..1 progress of the run.</summary>
        private float RunProgress => runTimer != null ? Mathf.Clamp01(runTimer.Elapsed / runTimer.Duration) : 0f;

        /// <summary>Run time mapped onto the schedule's designed length (compresses a short test run).</summary>
        private float ScheduleTime => RunProgress * waves.designedDuration;

        private void Update()
        {
            if (!WavesEnabled || waves == null || enemyPrefab == null || runTimer == null || !runTimer.IsRunning) return;
            if (PlayerRegistry.All.Count == 0) return;

            while (credits.Count < waves.entries.Count) credits.Add(0f);

            float time = ScheduleTime;
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
            Vector2 groupPos = RandomOffscreenPoint();
            float p = RunProgress;
            float hpMul = waves.healthMultiplier.Evaluate(p);
            float dmgMul = waves.damageMultiplier.Evaluate(p);

            for (int n = 0; n < entry.groupSize; n++)
            {
                if (EnemyRegistry.Count >= waves.maxAlive) return;
                Spawn(entry.enemy, groupPos + Random.insideUnitCircle * entry.groupSpread, hpMul, dmgMul);
            }
        }

        /// <summary>A point just outside the view of a random hunter (in co-op every player gets pressure).</summary>
        public Vector2 RandomOffscreenPoint()
        {
            var players = PlayerRegistry.All;
            Vector2 center = players.Count > 0 ? players[Random.Range(0, players.Count)].transform.position : Vector2.zero;
            return center + Random.insideUnitCircle.normalized * SpawnRadius();
        }

        public EnemyController Spawn(EnemyData data, Vector2 position, float healthMultiplier = 1f, float damageMultiplier = 1f)
        {
            var prefab = data.prefabOverride != null ? data.prefabOverride : enemyPrefab;
            var enemy = Instantiate(prefab, position, Quaternion.identity, transform);
            enemy.Init(data, position, healthMultiplier, damageMultiplier);
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
