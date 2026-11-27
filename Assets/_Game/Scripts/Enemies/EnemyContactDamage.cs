using HunterVsDino.Players;
using UnityEngine;

namespace HunterVsDino.Enemies
{
    /// <summary>
    /// Hurts a hunter while touching it, at most once per <see cref="hitInterval"/>.
    /// Each dino has its own cooldown, so being surrounded hurts more (survivors-like feel).
    /// </summary>
    [RequireComponent(typeof(EnemyController))]
    public class EnemyContactDamage : MonoBehaviour
    {
        [SerializeField, Min(0.05f)] private float hitInterval = 0.75f;

        private EnemyController enemy;
        private float nextHitTime;

        private void Awake()
        {
            enemy = GetComponent<EnemyController>();
        }

        private void OnEnable()
        {
            nextHitTime = 0f;
        }

        private void OnCollisionEnter2D(Collision2D collision) => TryHit(collision);
        private void OnCollisionStay2D(Collision2D collision) => TryHit(collision);

        private void TryHit(Collision2D collision)
        {
            if (Time.time < nextHitTime || enemy.Data == null) return;
            if (!collision.collider.TryGetComponent(out Player player) || player.IsDead) return;

            player.Health.TakeDamage(enemy.ContactDamage);
            nextHitTime = Time.time + hitInterval;
        }
    }
}
