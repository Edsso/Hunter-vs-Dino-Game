using UnityEngine;

namespace HunterVsDino.Enemies
{
    /// <summary>
    /// T-Rex roar: every so often it stops, swells up (telegraph) and roars, calling a pack of
    /// smaller dinos around itself. Waits its turn if another action (a charge) is running.
    /// </summary>
    [RequireComponent(typeof(EnemyController))]
    public class RoarBehaviour : MonoBehaviour
    {
        [SerializeField] private float interval = 10f;
        [SerializeField] private float windupTime = 0.9f;
        [SerializeField] private EnemyData summon;
        [SerializeField, Min(0)] private int summonCount = 6;
        [SerializeField] private float summonRadius = 3.5f;
        [SerializeField] private float windupPulse = 0.3f;

        private EnemyController enemy;
        private EnemySpawner spawner;
        private Transform visual;
        private bool roaring;
        private float nextRoar, roarAt;

        private void Awake()
        {
            enemy = GetComponent<EnemyController>();
            visual = enemy.SpriteRenderer != null ? enemy.SpriteRenderer.transform : null;
            enemy.Initialized += _ => ResetState();
            ResetState();
        }

        private void Start()
        {
            spawner = FindAnyObjectByType<EnemySpawner>();
        }

        private void ResetState()
        {
            roaring = false;
            nextRoar = Time.time + interval * 0.5f;
            if (visual != null) visual.localScale = Vector3.one;
            enemy.EndAction(this);
        }

        private void Update()
        {
            if (!roaring)
            {
                if (Time.time < nextRoar || enemy.IsRooted || !enemy.TryBeginAction(this)) return;
                roaring = true;
                roarAt = Time.time + windupTime;
                enemy.SetVelocityOverride(Vector2.zero);
                return;
            }

            if (visual != null)
                visual.localScale = Vector3.one * (1f + Mathf.PingPong(Time.time * 6f, 1f) * windupPulse);
            if (Time.time < roarAt) return;

            Roar();
            if (visual != null) visual.localScale = Vector3.one;
            roaring = false;
            nextRoar = Time.time + interval;
            enemy.EndAction(this);
        }

        private void Roar()
        {
            if (spawner == null || summon == null) return;
            Vector2 center = transform.position;
            for (int i = 0; i < summonCount; i++)
            {
                float a = (i / (float)summonCount) * Mathf.PI * 2f;
                spawner.Spawn(summon, center + new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * summonRadius);
            }
        }
    }
}
