using HunterVsDino.Players;
using UnityEngine;

namespace HunterVsDino.Enemies
{
    /// <summary>
    /// Generic dinosaur: takes its look and stats from an <see cref="EnemyData"/> and
    /// walks toward the nearest hunter. Special behaviours (charge, flying) come later as extra components.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private EnemyData data;
        [SerializeField] private SpriteRenderer spriteRenderer;

        private Rigidbody2D body;

        public EnemyData Data => data;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            if (data != null) ApplyData();
        }

        /// <summary>
        /// Sets up this enemy for a new life. Called by the spawner right after creating it
        /// (and later, when pooling, every time it is reused).
        /// </summary>
        public void Init(EnemyData enemyData, Vector2 position)
        {
            data = enemyData;
            transform.position = position;
            body.position = position;
            body.linearVelocity = Vector2.zero;
            ApplyData();
        }

        private void ApplyData()
        {
            name = data.displayName;
            transform.localScale = Vector3.one * data.size;
            body.mass = data.mass;
            if (spriteRenderer != null)
            {
                if (data.sprite != null) spriteRenderer.sprite = data.sprite;
                spriteRenderer.color = data.color;
            }
        }

        private void OnEnable() => EnemyRegistry.Register(this);
        private void OnDisable() => EnemyRegistry.Unregister(this);

        private void FixedUpdate()
        {
            Player target = PlayerRegistry.GetNearest(body.position);
            if (target == null || data == null)
            {
                body.linearVelocity = Vector2.zero;
                return;
            }

            Vector2 toTarget = (Vector2)target.transform.position - body.position;
            Vector2 dir = toTarget.sqrMagnitude > 0.0001f ? toTarget.normalized : Vector2.zero;
            body.linearVelocity = dir * data.moveSpeed;

            // Placeholder sprites flip to face the player (works for any sprite, including final art).
            if (spriteRenderer != null && Mathf.Abs(dir.x) > 0.01f)
                spriteRenderer.flipX = dir.x < 0f;
        }
    }
}
