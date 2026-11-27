using UnityEngine;

namespace HunterVsDino.Progression
{
    /// <summary>
    /// Experience left behind by a dead dino. Collection and the XP bar come in step 6;
    /// for now it just holds its value on the ground.
    /// </summary>
    public class XPGem : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        public int Value { get; private set; }

        public void Init(int value, Vector2 position)
        {
            Value = value;
            transform.position = position;
        }
    }
}
