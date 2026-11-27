using UnityEngine;

namespace HunterVsDino.Players
{
    /// <summary>
    /// Identity of a hunter in the run. Registers itself in <see cref="PlayerRegistry"/>
    /// so enemies, camera and pickups find players without a singleton.
    /// </summary>
    [DisallowMultipleComponent]
    public class Player : MonoBehaviour
    {
        [Tooltip("0 for the first player. In co-op each hunter gets its own index.")]
        [SerializeField] private int playerIndex;

        public int PlayerIndex => playerIndex;
        public PlayerMotor Motor { get; private set; }

        private void Awake()
        {
            Motor = GetComponent<PlayerMotor>();
        }

        private void OnEnable() => PlayerRegistry.Register(this);
        private void OnDisable() => PlayerRegistry.Unregister(this);
    }
}
