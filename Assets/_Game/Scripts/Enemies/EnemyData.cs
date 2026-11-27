using UnityEngine;

namespace HunterVsDino.Enemies
{
    /// <summary>
    /// Balance data for one dinosaur type. Create via Assets > Create > Hunter vs Dino > Enemy.
    /// All enemies share one prefab; this asset decides how it looks and behaves.
    /// </summary>
    [CreateAssetMenu(fileName = "Enemy", menuName = "Hunter vs Dino/Enemy")]
    public class EnemyData : ScriptableObject
    {
        [Header("Identity")]
        public string displayName = "Dino";
        [Tooltip("Bosses resist roots and can't be tamed.")]
        public bool isBoss;
        [Tooltip("Optional prefab variant with extra behaviours (charge, flying...). Empty = the generic Enemy prefab.")]
        public EnemyController prefabOverride;

        [Header("Look (placeholder until final art)")]
        public Sprite sprite;
        public Color color = Color.green;
        [Min(0.1f)] public float size = 1f;

        [Header("Stats")]
        [Min(0f)] public float moveSpeed = 2.5f;
        [Min(1f)] public float maxHealth = 10f;
        [Min(0f)] public float contactDamage = 5f;
        [Min(0)] public int xpValue = 1;

        [Header("Physics")]
        [Tooltip("Heavier dinos push lighter ones out of the way.")]
        [Min(0.01f)] public float mass = 1f;
    }
}
