using System.Collections.Generic;
using UnityEngine;

namespace HunterVsDino.Progression
{
    /// <summary>
    /// Experience left behind by a dead dino. Collectors (hunters) attract gems inside their magnet radius;
    /// once attracted, a gem flies to that hunter, speeding up, and is collected on arrival.
    /// No physics: a plain list is cheaper than hundreds of trigger colliders.
    /// </summary>
    public class XPGem : MonoBehaviour
    {
        private static readonly List<XPGem> active = new();
        public static IReadOnlyList<XPGem> Active => active;

        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private float startSpeed = 4f;
        [SerializeField] private float acceleration = 30f;
        [SerializeField] private float collectDistance = 0.35f;

        private PlayerPickupCollector target;
        private float speed;

        public int Value { get; private set; }
        public bool IsAttracted => target != null;

        public void Init(int value, Vector2 position)
        {
            Value = value;
            transform.position = position;
            target = null;
        }

        private void OnEnable() => active.Add(this);
        private void OnDisable() => active.Remove(this);

        public void Attract(PlayerPickupCollector collector)
        {
            if (target != null) return;
            target = collector;
            speed = startSpeed;
        }

        private void Update()
        {
            if (target == null) return;
            if (!target.isActiveAndEnabled)
            {
                target = null; // hunter died: gem stays for someone else
                return;
            }

            speed += acceleration * Time.deltaTime;
            Vector3 to = target.transform.position;
            transform.position = Vector3.MoveTowards(transform.position, to, speed * Time.deltaTime);

            if ((transform.position - to).sqrMagnitude <= collectDistance * collectDistance)
            {
                target.Collect(this);
                // Etapa 4 (performance): return to a pool instead of destroying.
                Destroy(gameObject);
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics() => active.Clear();
    }
}
