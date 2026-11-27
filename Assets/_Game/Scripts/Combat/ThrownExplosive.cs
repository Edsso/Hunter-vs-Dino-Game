using System.Collections.Generic;
using HunterVsDino.Enemies;
using UnityEngine;

namespace HunterVsDino.Combat
{
    /// <summary>A grenade in flight: arcs to its landing point, then explodes in an area.</summary>
    public class ThrownExplosive : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [Tooltip("Height of the fake arc, in world units.")]
        [SerializeField] private float arcHeight = 1.2f;

        private static readonly List<EnemyController> buffer = new();

        private Vector2 from, to;
        private float flightTime, startTime, damage, radius;
        private WeaponData data;
        private FxFlash blastFx;

        public void Launch(Vector2 start, Vector2 landing, float time, float blastDamage, float blastRadius, WeaponData weapon, FxFlash fx)
        {
            from = start;
            to = landing;
            flightTime = Mathf.Max(0.05f, time);
            startTime = Time.time;
            damage = blastDamage;
            radius = blastRadius;
            data = weapon;
            blastFx = fx;

            transform.position = start;
            transform.localScale = Vector3.one * weapon.projectileSize;
            if (spriteRenderer != null)
            {
                if (weapon.projectileSprite != null) spriteRenderer.sprite = weapon.projectileSprite;
                spriteRenderer.color = weapon.projectileColor;
            }
        }

        private void Update()
        {
            float t = Mathf.Clamp01((Time.time - startTime) / flightTime);
            Vector2 pos = Vector2.Lerp(from, to, t);
            pos.y += Mathf.Sin(t * Mathf.PI) * arcHeight; // top-down "lob"
            transform.position = pos;
            transform.Rotate(0f, 0f, 540f * Time.deltaTime);

            if (t >= 1f) Explode();
        }

        private void Explode()
        {
            CombatQuery.DamageCircle(to, radius, damage, buffer);
            if (blastFx != null) FxFlash.Spawn(blastFx, to, radius, 0f, new Color(1f, 0.6f, 0.2f, 0.6f));
            // Etapa 4 (performance): return to a pool instead of destroying.
            Destroy(gameObject);
        }
    }
}
