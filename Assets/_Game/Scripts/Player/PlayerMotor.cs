using UnityEngine;

namespace HunterVsDino.Players
{
    /// <summary>
    /// Moves the hunter from a move intent. It does not read input:
    /// whoever controls the player (local input now, network later) sets <see cref="MoveIntent"/>.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMotor : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;

        [Tooltip("Optional child that rotates to face the move direction (the triangle).")]
        [SerializeField] private Transform visual;

        private Rigidbody2D body;
        private Vector2 moveIntent;

        /// <summary>Desired direction, magnitude 0..1 (analog sticks give partial values).</summary>
        public Vector2 MoveIntent
        {
            get => moveIntent;
            set => moveIntent = Vector2.ClampMagnitude(value, 1f);
        }

        /// <summary>Last non-zero move direction. Weapons can use it to aim.</summary>
        public Vector2 FacingDirection { get; private set; } = Vector2.up;

        public float MoveSpeed
        {
            get => moveSpeed;
            set => moveSpeed = Mathf.Max(0f, value);
        }

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            body.linearVelocity = moveIntent * moveSpeed;

            if (moveIntent.sqrMagnitude > 0.01f)
            {
                FacingDirection = moveIntent.normalized;
                if (visual != null)
                {
                    float angle = Mathf.Atan2(FacingDirection.y, FacingDirection.x) * Mathf.Rad2Deg - 90f;
                    visual.rotation = Quaternion.Euler(0f, 0f, angle);
                }
            }
        }
    }
}
