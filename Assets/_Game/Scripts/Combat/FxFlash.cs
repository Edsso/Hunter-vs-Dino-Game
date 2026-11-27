using UnityEngine;

namespace HunterVsDino.Combat
{
    /// <summary>
    /// Placeholder hit effect: a sprite that grows a little and fades out, then removes itself.
    /// Used for slashes and explosions until there are real particles (Etapa 3).
    /// </summary>
    public class FxFlash : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private float duration = 0.15f;
        [SerializeField] private float growTo = 1.15f;

        private float startTime;
        private Vector3 baseScale;
        private Color baseColor;

        public static FxFlash Spawn(FxFlash prefab, Vector2 position, float radius, float angleDeg, Color color)
        {
            var fx = Instantiate(prefab, position, Quaternion.Euler(0f, 0f, angleDeg));
            fx.transform.localScale = Vector3.one * radius * 2f; // sprites are 1 unit wide
            fx.spriteRenderer.color = color;
            return fx;
        }

        private void Start()
        {
            startTime = Time.time;
            baseScale = transform.localScale;
            baseColor = spriteRenderer.color;
        }

        private void Update()
        {
            float t = Mathf.Clamp01((Time.time - startTime) / duration);
            transform.localScale = baseScale * Mathf.Lerp(1f, growTo, t);
            var c = baseColor;
            c.a = baseColor.a * (1f - t);
            spriteRenderer.color = c;
            if (t >= 1f) Destroy(gameObject);
        }
    }
}
