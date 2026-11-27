using System;
using HunterVsDino.Players;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HunterVsDino.Core
{
    public enum RunState
    {
        Playing,
        LevelUp,
        GameOver,
        Victory
    }

    /// <summary>
    /// Owns the run state machine (playing, level up, game over, victory) and pausing.
    /// It watches the players through <see cref="PlayerRegistry"/>: the run ends when no hunter is left alive.
    /// Only this class writes Time.timeScale, so pauses never fight each other.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private RunTimer runTimer;

        public RunState State { get; private set; } = RunState.Playing;
        public RunTimer Timer => runTimer;

        /// <summary>Game speed while playing (debug x10 sets this).</summary>
        public float PlaySpeed
        {
            get => playSpeed;
            set
            {
                playSpeed = Mathf.Max(0f, value);
                if (State == RunState.Playing) Time.timeScale = playSpeed;
            }
        }

        public event Action<RunState> StateChanged;

        private float playSpeed = 1f;
        private bool sawPlayers;

        private void Start()
        {
            SetState(RunState.Playing);
        }

        private void Update()
        {
            if (State != RunState.Playing) return;

            int alive = PlayerRegistry.All.Count;
            if (alive > 0) sawPlayers = true;
            else if (sawPlayers) GameOver();
        }

        /// <summary>Pause for the upgrade choice. Returns false if the run can't pause now.</summary>
        public bool EnterLevelUp()
        {
            if (State != RunState.Playing) return false;
            SetState(RunState.LevelUp);
            return true;
        }

        public void ExitLevelUp()
        {
            if (State == RunState.LevelUp) SetState(RunState.Playing);
        }

        public void GameOver()
        {
            if (State == RunState.GameOver) return;
            runTimer.Pause();
            SetState(RunState.GameOver);
        }

        public void Victory()
        {
            if (State == RunState.GameOver || State == RunState.Victory) return;
            runTimer.Pause();
            SetState(RunState.Victory);
        }

        public void Restart()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void SetState(RunState state)
        {
            State = state;
            Time.timeScale = state == RunState.Playing ? playSpeed : 0f;
            StateChanged?.Invoke(state);
        }

        private void OnDestroy()
        {
            Time.timeScale = 1f;
        }
    }
}
