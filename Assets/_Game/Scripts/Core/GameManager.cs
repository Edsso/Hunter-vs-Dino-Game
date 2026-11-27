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
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private RunTimer runTimer;

        public RunState State { get; private set; } = RunState.Playing;
        public RunTimer Timer => runTimer;

        public event Action<RunState> StateChanged;

        private bool sawPlayers;

        private void Start()
        {
            Time.timeScale = 1f;
            SetState(RunState.Playing);
        }

        private void Update()
        {
            if (State != RunState.Playing) return;

            int alive = PlayerRegistry.All.Count;
            if (alive > 0) sawPlayers = true;
            else if (sawPlayers) GameOver();
        }

        public void GameOver()
        {
            if (State == RunState.GameOver) return;
            runTimer.Pause();
            Time.timeScale = 0f;
            SetState(RunState.GameOver);
        }

        public void Restart()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void SetState(RunState state)
        {
            State = state;
            StateChanged?.Invoke(state);
        }
    }
}
