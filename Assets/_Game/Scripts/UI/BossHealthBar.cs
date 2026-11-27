using HunterVsDino.Combat;
using HunterVsDino.Enemies;
using UnityEngine;

namespace HunterVsDino.UI
{
    /// <summary>Big health bar at the top while the boss is alive.</summary>
    public class BossHealthBar : MonoBehaviour
    {
        [SerializeField] private BossDirector director;
        [SerializeField] private GameObject root;
        [SerializeField] private RectTransform fill;
        [SerializeField] private TMPro.TMP_Text nameLabel;

        private Health bossHealth;

        private void Awake()
        {
            root.SetActive(false);
        }

        private void OnEnable() => director.BossSpawnedEvent += OnBossSpawned;

        private void OnDisable()
        {
            director.BossSpawnedEvent -= OnBossSpawned;
            Unbind();
        }

        private void OnBossSpawned(EnemyController boss)
        {
            Unbind();
            bossHealth = boss.Health;
            bossHealth.Changed += Refresh;
            bossHealth.Died += OnBossDied;
            if (nameLabel != null) nameLabel.text = boss.Data.displayName.ToUpperInvariant();
            root.SetActive(true);
            Refresh(bossHealth);
        }

        private void OnBossDied()
        {
            root.SetActive(false);
            Unbind();
        }

        private void Unbind()
        {
            if (bossHealth == null) return;
            bossHealth.Changed -= Refresh;
            bossHealth.Died -= OnBossDied;
            bossHealth = null;
        }

        private void Refresh(Health h)
        {
            fill.localScale = new Vector3(h.Normalized, 1f, 1f);
        }
    }
}
