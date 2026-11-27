using UnityEngine;

namespace HunterVsDino.Enemies
{
    /// <summary>
    /// Triceratops-style charge: when a hunter is in range it stops and "pulses" (telegraph),
    /// then rushes in a straight line, then needs a moment to recover. Dodge sideways!
    /// </summary>
    [RequireComponent(typeof(EnemyController))]
    public class ChargeBehaviour : MonoBehaviour
    {
        [SerializeField] private float triggerRange = 7f;
        [SerializeField] private float windupTime = 0.7f;
        [SerializeField] private float chargeSpeedMultiplier = 4f;
        [SerializeField] private float chargeTime = 0.8f;
        [SerializeField] private float recoverTime = 0.6f;
        [SerializeField] private float cooldown = 3.5f;
        [Tooltip("How much the sprite swells while winding up (telegraph).")]
        [SerializeField] private float windupPulse = 0.2f;

        private enum State { Chase, Windup, Charge, Recover }

        private EnemyController enemy;
        private Transform visual;
        private State state;
        private float stateEnd, nextCharge;
        private Vector2 direction;

        public bool IsCharging => state == State.Charge;

        private void Awake()
        {
            enemy = GetComponent<EnemyController>();
            visual = enemy.SpriteRenderer != null ? enemy.SpriteRenderer.transform : null;
            enemy.Initialized += _ => ResetState();
            ResetState();
        }

        private void ResetState()
        {
            state = State.Chase;
            nextCharge = Time.time + Random.Range(1f, cooldown);
            if (visual != null) visual.localScale = Vector3.one;
            enemy.EndAction(this);
        }

        private void Update()
        {
            switch (state)
            {
                case State.Chase:
                    if (Time.time < nextCharge || enemy.IsRooted) return;
                    Vector2 dir = enemy.DirectionToNearestPlayer(out float dist);
                    if (dist > triggerRange || dir == Vector2.zero || !enemy.TryBeginAction(this)) return;
                    direction = dir;
                    enemy.SetVelocityOverride(Vector2.zero);
                    Enter(State.Windup, windupTime);
                    break;

                case State.Windup:
                    if (visual != null)
                        visual.localScale = Vector3.one * (1f + Mathf.PingPong(Time.time * 8f, 1f) * windupPulse);
                    if (Time.time < stateEnd) return;
                    if (visual != null) visual.localScale = Vector3.one;
                    enemy.SetVelocityOverride(direction * enemy.Data.moveSpeed * chargeSpeedMultiplier);
                    Enter(State.Charge, chargeTime);
                    break;

                case State.Charge:
                    if (Time.time < stateEnd && !enemy.IsRooted) return;
                    enemy.SetVelocityOverride(direction * enemy.Data.moveSpeed * 0.2f);
                    Enter(State.Recover, recoverTime);
                    break;

                case State.Recover:
                    if (Time.time < stateEnd) return;
                    enemy.EndAction(this);
                    state = State.Chase;
                    nextCharge = Time.time + cooldown;
                    break;
            }
        }

        private void Enter(State next, float duration)
        {
            state = next;
            stateEnd = Time.time + duration;
        }
    }
}
