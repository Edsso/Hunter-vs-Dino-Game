using System;
using HunterVsDino.Core;
using UnityEngine;

namespace HunterVsDino.Enemies
{
    /// <summary>
    /// End of the run: when the clock runs out, normal waves stop and the T-Rex arrives.
    /// Killing it wins the run.
    /// </summary>
    public class BossDirector : MonoBehaviour
    {
        [SerializeField] private RunTimer runTimer;
        [SerializeField] private GameManager gameManager;
        [SerializeField] private EnemySpawner spawner;
        [SerializeField] private EnemyData bossData;
        [Tooltip("Stop the normal waves when the boss shows up.")]
        [SerializeField] private bool stopWaves = true;

        public EnemyController Boss { get; private set; }
        public bool BossSpawned { get; private set; }

        /// <summary>Raised when the boss enters (HUD shows its health bar).</summary>
        public event Action<EnemyController> BossSpawnedEvent;

        private void Update()
        {
            if (BossSpawned || runTimer == null || !runTimer.IsFinished) return;
            if (gameManager.State != RunState.Playing) return;
            SpawnBoss();
        }

        private void SpawnBoss()
        {
            BossSpawned = true;
            if (stopWaves) spawner.WavesEnabled = false;

            Boss = spawner.Spawn(bossData, spawner.RandomOffscreenPoint());
            Boss.Died += OnBossDied;
            BossSpawnedEvent?.Invoke(Boss);
        }

        private void OnBossDied(EnemyController boss)
        {
            boss.Died -= OnBossDied;
            gameManager.Victory();
        }
    }
}
