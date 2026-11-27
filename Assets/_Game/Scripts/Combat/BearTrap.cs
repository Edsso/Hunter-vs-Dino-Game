using System.Collections.Generic;
using HunterVsDino.Enemies;
using UnityEngine;

namespace HunterVsDino.Combat
{
    /// <summary>
    /// A trap on the ground. Checks for dinos a few times per second (cheaper than a trigger per trap);
    /// catches up to <c>charges</c> of them, then disappears. Also expires after its lifetime.
    /// </summary>
    public class BearTrap : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private float checkInterval = 0.1f;
        [SerializeField] private Color armedColor = new(0.55f, 0.55f, 0.6f);
        [SerializeField] private Color snapColor = new(1f, 0.3f, 0.2f);

        private static readonly List<EnemyController> buffer = new();
        private readonly HashSet<EnemyController> caught = new();

        private float damage, radius, rootTime, expireTime, nextCheck;
        private int charges;

        public void Arm(WeaponData data, float trapDamage, float trapRadius, float rootSeconds, int maxCatches, float lifetime)
        {
            damage = trapDamage;
            radius = trapRadius;
            rootTime = rootSeconds;
            charges = Mathf.Max(1, maxCatches);
            expireTime = Time.time + lifetime;
            caught.Clear();

            transform.localScale = Vector3.one * radius * 2f;
            if (spriteRenderer != null)
            {
                if (data.projectileSprite != null) spriteRenderer.sprite = data.projectileSprite;
                spriteRenderer.color = armedColor;
            }
        }

        private void Update()
        {
            if (Time.time >= expireTime) { Destroy(gameObject); return; }
            if (Time.time < nextCheck) return;
            nextCheck = Time.time + checkInterval;

            CombatQuery.EnemiesInCircle(transform.position, radius, buffer);
            for (int i = 0; i < buffer.Count && charges > 0; i++)
            {
                var e = buffer[i];
                if (caught.Contains(e)) continue;
                caught.Add(e);
                e.Root(rootTime);
                e.Health.TakeDamage(damage);
                charges--;
                if (spriteRenderer != null) spriteRenderer.color = snapColor;
            }

            // Used up: linger a moment so the snap is visible, then go.
            if (charges <= 0) expireTime = Mathf.Min(expireTime, Time.time + 0.4f);
        }
    }
}
