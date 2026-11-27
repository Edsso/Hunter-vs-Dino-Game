using System;
using HunterVsDino.Combat;
using UnityEngine;

namespace HunterVsDino.Players
{
    /// <summary>
    /// Identity of a hunter in the run. Registers itself in <see cref="PlayerRegistry"/>
    /// so enemies, camera and pickups find players without a singleton.
    /// A dead hunter leaves the registry: enemies stop chasing it and, in co-op, go after the others.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Health))]
    public class Player : MonoBehaviour
    {
        [Tooltip("0 for the first player. In co-op each hunter gets its own index.")]
        [SerializeField] private int playerIndex;
        [SerializeField] private SpriteRenderer visual;
        [SerializeField] private Color deadColor = new(0.35f, 0.35f, 0.35f);

        public int PlayerIndex => playerIndex;
        public PlayerMotor Motor { get; private set; }
        public Health Health { get; private set; }
        public bool IsDead => Health.IsDead;

        /// <summary>Raised once when this hunter dies.</summary>
        public event Action<Player> Died;

        private void Awake()
        {
            Motor = GetComponent<PlayerMotor>();
            Health = GetComponent<Health>();
            Health.Died += OnDied;
        }

        private void OnDestroy()
        {
            if (Health != null) Health.Died -= OnDied;
        }

        private void OnEnable()
        {
            if (!IsDead) PlayerRegistry.Register(this);
        }

        private void OnDisable() => PlayerRegistry.Unregister(this);

        private void OnDied()
        {
            PlayerRegistry.Unregister(this);

            // Stop controlling the body.
            if (TryGetComponent(out PlayerInputHandler input)) input.enabled = false;
            Motor.MoveIntent = Vector2.zero;
            Motor.enabled = false;
            if (TryGetComponent(out Rigidbody2D body)) body.linearVelocity = Vector2.zero;
            if (TryGetComponent(out Collider2D col)) col.enabled = false;

            if (visual != null) visual.color = deadColor;

            Died?.Invoke(this);
        }
    }
}
