using System;
using HunterVsDino.Combat;
using HunterVsDino.Players;
using UnityEngine;

namespace HunterVsDino.Enemies
{
    /// <summary>
    /// Generic dinosaur: takes its look and stats from an <see cref="EnemyData"/> and walks toward the
    /// nearest hunter. Special behaviours (charge, roar...) are extra components on a prefab variant;
    /// they take over movement with <see cref="SetVelocityOverride"/> and use the action lock so two
    /// behaviours never act at the same time.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D), typeof(Health))]
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private EnemyData data;
        [SerializeField] private SpriteRenderer spriteRenderer;

        private Rigidbody2D body;
        private float rootedUntil;
        private bool hasVelocityOverride;
        private Vector2 velocityOverride;
        private object actionOwner;

        public EnemyData Data => data;
        public Health Health { get; private set; }
        public SpriteRenderer SpriteRenderer => spriteRenderer;
        public Rigidbody2D Body => body;

        /// <summary>Contact damage after the difficulty multiplier of when it spawned.</summary>
        public float ContactDamage { get; private set; }

        /// <summary>Held in place (trap, bola). It can still be hit and still hurts on contact.</summary>
        public bool IsRooted => Time.time < rootedUntil;

        /// <summary>Raised when this dino dies (XP drops and kill counters hook in here).</summary>
        public event Action<EnemyController> Died;
        /// <summary>Raised after <see cref="Init"/>: behaviours reset their state here (works with pooling).</summary>
        public event Action<EnemyController> Initialized;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            Health = GetComponent<Health>();
            Health.Died += OnDied;
            if (data != null) ApplyData();
        }

        private void OnDestroy()
        {
            if (Health != null) Health.Died -= OnDied;
        }

        private void OnDied()
        {
            Died?.Invoke(this);
            EnemyRegistry.RaiseDied(this);
            // Etapa 4 (performance): return to a pool instead of destroying.
            Destroy(gameObject);
        }

        /// <summary>
        /// Sets up this enemy for a new life. Called by the spawner right after creating it
        /// (and later, when pooling, every time it is reused).
        /// </summary>
        public void Init(EnemyData enemyData, Vector2 position, float healthMultiplier = 1f, float damageMultiplier = 1f)
        {
            data = enemyData;
            rootedUntil = 0f;
            hasVelocityOverride = false;
            actionOwner = null;
            transform.position = position;
            body.position = position;
            body.linearVelocity = Vector2.zero;
            ApplyData(healthMultiplier, damageMultiplier);
            Initialized?.Invoke(this);
        }

        private void ApplyData(float healthMultiplier = 1f, float damageMultiplier = 1f)
        {
            name = data.displayName;
            transform.localScale = Vector3.one * data.size;
            body.mass = data.mass;
            Health.ResetHealth(data.maxHealth * healthMultiplier);
            ContactDamage = data.contactDamage * damageMultiplier;
            if (spriteRenderer != null)
            {
                if (data.sprite != null) spriteRenderer.sprite = data.sprite;
                spriteRenderer.color = data.color;
            }
        }

        // ---------- Hooks for behaviours ----------

        /// <summary>Stops the dino for <paramref name="seconds"/>. Bosses shake it off 4x faster.</summary>
        public void Root(float seconds)
        {
            if (data != null && data.isBoss) seconds *= 0.25f;
            rootedUntil = Mathf.Max(rootedUntil, Time.time + seconds);
        }

        /// <summary>Move with this velocity instead of chasing (a charge, a windup that stands still...).</summary>
        public void SetVelocityOverride(Vector2 velocity)
        {
            hasVelocityOverride = true;
            velocityOverride = velocity;
        }

        public void ClearVelocityOverride() => hasVelocityOverride = false;

        /// <summary>True while some behaviour is in the middle of an action.</summary>
        public bool IsBusy => actionOwner != null;

        /// <summary>Claim the dino for an action. Fails if another behaviour is acting.</summary>
        public bool TryBeginAction(object owner)
        {
            if (actionOwner != null && actionOwner != owner) return false;
            actionOwner = owner;
            return true;
        }

        public void EndAction(object owner)
        {
            if (actionOwner != owner) return;
            actionOwner = null;
            hasVelocityOverride = false;
        }

        /// <summary>Unit vector toward the nearest hunter (zero if none).</summary>
        public Vector2 DirectionToNearestPlayer(out float distance)
        {
            Player target = PlayerRegistry.GetNearest(body.position);
            if (target == null) { distance = float.MaxValue; return Vector2.zero; }
            Vector2 to = (Vector2)target.transform.position - body.position;
            distance = to.magnitude;
            return distance > 0.0001f ? to / distance : Vector2.zero;
        }

        private void OnEnable() => EnemyRegistry.Register(this);
        private void OnDisable() => EnemyRegistry.Unregister(this);

        private void FixedUpdate()
        {
            if (IsRooted || data == null)
            {
                body.linearVelocity = Vector2.zero;
                return;
            }

            if (hasVelocityOverride)
            {
                body.linearVelocity = velocityOverride;
                return;
            }

            Vector2 dir = DirectionToNearestPlayer(out _);
            body.linearVelocity = dir * data.moveSpeed;

            // Placeholder sprites flip to face the player (works for any sprite, including final art).
            if (spriteRenderer != null && Mathf.Abs(dir.x) > 0.01f)
                spriteRenderer.flipX = dir.x < 0f;
        }
    }
}
