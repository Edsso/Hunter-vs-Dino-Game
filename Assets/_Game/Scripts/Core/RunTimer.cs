using UnityEngine;

namespace HunterVsDino.Core
{
    /// <summary>
    /// Clock of the current run (15 minutes). Spawner, difficulty and HUD read from it.
    /// It is run state, not player state, so one instance serves every player (and the host in co-op).
    /// </summary>
    public class RunTimer : MonoBehaviour
    {
        [Tooltip("Run length in seconds. 900 = 15 minutes.")]
        [SerializeField] private float duration = 900f;
        [SerializeField] private bool startOnAwake = true;

        public float Duration => duration;
        public float Elapsed { get; private set; }
        public float Remaining => Mathf.Max(0f, duration - Elapsed);
        public bool IsRunning { get; private set; }
        public bool IsFinished => Elapsed >= duration;

        private void Awake()
        {
            if (startOnAwake) StartRun();
        }

        public void StartRun()
        {
            Elapsed = 0f;
            IsRunning = true;
        }

        public void Pause() => IsRunning = false;
        public void Resume() => IsRunning = true;

        private void Update()
        {
            if (!IsRunning) return;
            Elapsed = Mathf.Min(duration, Elapsed + Time.deltaTime);
        }

        /// <summary>Debug helper: jump the clock (e.g. to test the 10-minute waves).</summary>
        public void SetElapsed(float seconds) => Elapsed = Mathf.Clamp(seconds, 0f, duration);
    }
}
