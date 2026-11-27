using UnityEngine;
using UnityEngine.InputSystem;

namespace HunterVsDino.Core
{
    /// <summary>
    /// Dev-only shortcuts. Only active in the Editor and in Development Builds.
    /// F1: toggle game speed x1 / x10.   F2: jump the run clock forward 1 minute.
    /// </summary>
    public class DebugTools : MonoBehaviour
    {
        [SerializeField] private RunTimer runTimer;
        [SerializeField] private float fastTimeScale = 10f;

        private void Awake()
        {
            if (!Debug.isDebugBuild) enabled = false;
        }

        private void Update()
        {
            var kb = Keyboard.current;
            if (kb == null) return;

            if (kb.f1Key.wasPressedThisFrame)
            {
                Time.timeScale = Mathf.Approximately(Time.timeScale, 1f) ? fastTimeScale : 1f;
                Debug.Log($"[Debug] timeScale = {Time.timeScale}");
            }

            if (kb.f2Key.wasPressedThisFrame && runTimer != null)
            {
                runTimer.SetElapsed(runTimer.Elapsed + 60f);
                Debug.Log($"[Debug] run time = {runTimer.Elapsed:0}s");
            }
        }

        private void OnDisable()
        {
            Time.timeScale = 1f;
        }
    }
}
