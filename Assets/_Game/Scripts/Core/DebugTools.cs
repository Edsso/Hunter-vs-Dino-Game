using HunterVsDino.Players;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HunterVsDino.Core
{
    /// <summary>
    /// Dev-only shortcuts. Only active in the Editor and in Development Builds.
    /// F1: toggle game speed x1 / x10.   F2: jump the run clock forward 1 minute.
    /// F3: give the local player enough XP for one level.
    /// </summary>
    public class DebugTools : MonoBehaviour
    {
        [SerializeField] private RunTimer runTimer;
        [SerializeField] private GameManager gameManager;
        [SerializeField] private float fastSpeed = 10f;

        private void Awake()
        {
            if (!Debug.isDebugBuild) enabled = false;
        }

        private void Update()
        {
            var kb = Keyboard.current;
            if (kb == null || gameManager == null || gameManager.State != RunState.Playing) return;

            if (kb.f1Key.wasPressedThisFrame)
            {
                gameManager.PlaySpeed = Mathf.Approximately(gameManager.PlaySpeed, 1f) ? fastSpeed : 1f;
                Debug.Log($"[Debug] game speed = x{gameManager.PlaySpeed}");
            }

            if (kb.f2Key.wasPressedThisFrame && runTimer != null)
            {
                runTimer.SetElapsed(runTimer.Elapsed + 60f);
                Debug.Log($"[Debug] run time = {runTimer.Elapsed:0}s");
            }

            if (kb.f3Key.wasPressedThisFrame && PlayerRegistry.All.Count > 0)
            {
                var xp = PlayerRegistry.All[0].Experience;
                xp.AddXP(xp.XPToNextLevel - xp.CurrentXP);
            }
        }
    }
}
