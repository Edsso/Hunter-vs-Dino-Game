using UnityEngine;

namespace HunterVsDino.Combat
{
    /// <summary>Tints the sprite for a moment whenever the Health takes damage.</summary>
    public class DamageFlash : MonoBehaviour
    {
        [SerializeField] private Health health;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Color flashColor = Color.white;
        [SerializeField] private float duration = 0.1f;

        private Color baseColor;
        private float timer;

        private void Awake()
        {
            if (health == null) health = GetComponent<Health>();
        }

        private void OnEnable()
        {
            if (health != null) health.Damaged += OnDamaged;
            timer = 0f;
        }

        private void OnDisable()
        {
            if (health != null) health.Damaged -= OnDamaged;
            if (timer > 0f && spriteRenderer != null) spriteRenderer.color = baseColor;
            timer = 0f;
        }

        private void OnDamaged(float amount)
        {
            if (spriteRenderer == null) return;
            if (timer <= 0f) baseColor = spriteRenderer.color;
            spriteRenderer.color = flashColor;
            timer = duration;
        }

        private void Update()
        {
            if (timer <= 0f) return;
            timer -= Time.deltaTime;
            if (timer <= 0f) spriteRenderer.color = baseColor;
        }
    }
}
