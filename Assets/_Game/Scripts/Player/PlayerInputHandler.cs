using UnityEngine;
using UnityEngine.InputSystem;

namespace HunterVsDino.Players
{
    /// <summary>
    /// Local input → <see cref="PlayerMotor.MoveIntent"/>. Only this script knows about the Input System.
    /// In online co-op, remote hunters won't have this component; the network sets their intent.
    /// </summary>
    [RequireComponent(typeof(PlayerMotor))]
    public class PlayerInputHandler : MonoBehaviour
    {
        [Tooltip("Player/Move from InputSystem_Actions (WASD, arrows, gamepad left stick).")]
        [SerializeField] private InputActionReference moveAction;

        private PlayerMotor motor;

        private void Awake()
        {
            motor = GetComponent<PlayerMotor>();
        }

        private void OnEnable()
        {
            if (moveAction != null) moveAction.action.Enable();
        }

        private void OnDisable()
        {
            if (motor != null) motor.MoveIntent = Vector2.zero;
        }

        private void Update()
        {
            if (moveAction == null) return;
            motor.MoveIntent = moveAction.action.ReadValue<Vector2>();
        }
    }
}
