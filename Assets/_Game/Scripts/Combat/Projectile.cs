using HunterVsDino.Enemies;
using UnityEngine;

namespace HunterVsDino.Combat
{
    /// <summary>
    /// Straight-flying bullet. Damages enemies it touches (trigger), goes through <c>pierce</c> of them,
    /// and expires after its lifetime. Set up with <see cref="Launch"/> so pooling can reuse it later.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        private Rigidbody2D body;
        private float damage;
        private int hitsLeft;
        private float expireTime;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
        }

        public void Launch(WeaponData data, Vector2 position, Vector2 direction)
        {
            transform.position = position;
            body.position = position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
            body.rotation = angle;
            body.linearVelocity = direction.normalized * data.projectileSpeed;

            damage = data.damage;
            hitsLeft = data.pierce + 1;
            expireTime = Time.time + data.projectileLifetime;

            transform.localScale = Vector3.one * data.projectileSize;
            if (spriteRenderer != null)
            {
                if (data.projectileSprite != null) spriteRenderer.sprite = data.projectileSprite;
                spriteRenderer.color = data.projectileColor;
            }
        }

        private void Update()
        {
            if (Time.time >= expireTime) Despawn();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (hitsLeft <= 0) return;
            if (!other.TryGetComponent(out EnemyController enemy) || enemy.Health.IsDead) return;

            enemy.Health.TakeDamage(damage);
            hitsLeft--;
            if (hitsLeft <= 0) Despawn();
        }

        private void Despawn()
        {
            // Etapa 4 (performance): return to a pool instead of destroying.
            Destroy(gameObject);
        }
    }
}
