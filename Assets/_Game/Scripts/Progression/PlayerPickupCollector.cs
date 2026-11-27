using UnityEngine;

namespace HunterVsDino.Progression
{
    /// <summary>
    /// Pulls XP gems inside <see cref="MagnetRadius"/> toward the hunter and gives their XP to
    /// <see cref="PlayerExperience"/>. The radius is a stat upgrades can raise.
    /// </summary>
    [RequireComponent(typeof(PlayerExperience))]
    public class PlayerPickupCollector : MonoBehaviour
    {
        [SerializeField, Min(0.1f)] private float magnetRadius = 2f;

        private PlayerExperience experience;

        public float MagnetRadius
        {
            get => magnetRadius;
            set => magnetRadius = Mathf.Max(0.1f, value);
        }

        private void Awake()
        {
            experience = GetComponent<PlayerExperience>();
        }

        private void Update()
        {
            Vector2 pos = transform.position;
            float r2 = magnetRadius * magnetRadius;
            var gems = XPGem.Active;
            for (int i = 0; i < gems.Count; i++)
            {
                var gem = gems[i];
                if (gem.IsAttracted) continue;
                if (((Vector2)gem.transform.position - pos).sqrMagnitude <= r2) gem.Attract(this);
            }
        }

        public void Collect(XPGem gem) => experience.AddXP(gem.Value);

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0.3f, 0.85f, 1f, 0.5f);
            Gizmos.DrawWireSphere(transform.position, magnetRadius);
        }
    }
}
