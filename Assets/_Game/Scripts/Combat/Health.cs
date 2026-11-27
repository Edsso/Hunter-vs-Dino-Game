using System;
using UnityEngine;

namespace HunterVsDino.Combat
{
    /// <summary>
    /// Hit points for anything that can be hurt (hunters, dinos, later breakable props).
    /// Other components listen to the events instead of polling.
    /// </summary>
    public class Health : MonoBehaviour
    {
        [SerializeField, Min(1f)] private float maxHealth = 100f;

        public float Max => maxHealth;
        public float Current { get; private set; }
        public bool IsDead { get; private set; }
        public float Normalized => maxHealth > 0f ? Current / maxHealth : 0f;

        /// <summary>Amount of damage actually taken (after clamping).</summary>
        public event Action<float> Damaged;
        public event Action<float> Healed;
        public event Action Died;
        /// <summary>Current or max changed (for health bars).</summary>
        public event Action<Health> Changed;

        private void Awake()
        {
            Current = maxHealth;
        }

        /// <summary>Full reset, used when (re)spawning or reusing from a pool.</summary>
        public void ResetHealth(float newMax)
        {
            maxHealth = Mathf.Max(1f, newMax);
            Current = maxHealth;
            IsDead = false;
            Changed?.Invoke(this);
        }

        public void SetMax(float newMax, bool healByDifference = true)
        {
            float diff = newMax - maxHealth;
            maxHealth = Mathf.Max(1f, newMax);
            if (healByDifference && diff > 0f) Current += diff;
            Current = Mathf.Min(Current, maxHealth);
            Changed?.Invoke(this);
        }

        public void TakeDamage(float amount)
        {
            if (IsDead || amount <= 0f) return;

            float taken = Mathf.Min(amount, Current);
            Current -= taken;
            Damaged?.Invoke(taken);
            Changed?.Invoke(this);

            if (Current <= 0f)
            {
                IsDead = true;
                Died?.Invoke();
            }
        }

        public void Heal(float amount)
        {
            if (IsDead || amount <= 0f) return;
            float healed = Mathf.Min(amount, maxHealth - Current);
            if (healed <= 0f) return;
            Current += healed;
            Healed?.Invoke(healed);
            Changed?.Invoke(this);
        }
    }
}
