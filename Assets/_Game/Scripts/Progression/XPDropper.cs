using HunterVsDino.Enemies;
using UnityEngine;

namespace HunterVsDino.Progression
{
    /// <summary>
    /// Scene system: whenever any dino dies, drops an XP gem worth its <see cref="EnemyData.xpValue"/>.
    /// Listening to the registry event keeps enemies unaware of loot rules.
    /// </summary>
    public class XPDropper : MonoBehaviour
    {
        [SerializeField] private XPGem gemPrefab;

        private void OnEnable() => EnemyRegistry.EnemyDied += OnEnemyDied;
        private void OnDisable() => EnemyRegistry.EnemyDied -= OnEnemyDied;

        private void OnEnemyDied(EnemyController enemy)
        {
            if (gemPrefab == null || enemy.Data == null || enemy.Data.xpValue <= 0) return;
            var gem = Instantiate(gemPrefab, transform);
            gem.Init(enemy.Data.xpValue, enemy.transform.position);
        }
    }
}
