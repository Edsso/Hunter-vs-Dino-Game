using HunterVsDino.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HunterVsDino.UI
{
    /// <summary>
    /// "You died" screen. Shows the time survived and restarts the run
    /// (button, R on keyboard, or South/Start on gamepad).
    /// </summary>
    public class GameOverPanel : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private GameObject root;
        [SerializeField] private TMPro.TMP_Text timeLabel;
        [SerializeField] private UnityEngine.UI.Button restartButton;

        private void Awake()
        {
            root.SetActive(false);
            if (restartButton != null) restartButton.onClick.AddListener(gameManager.Restart);
        }

        private void OnEnable() => gameManager.StateChanged += OnStateChanged;
        private void OnDisable() => gameManager.StateChanged -= OnStateChanged;

        private void OnStateChanged(RunState state)
        {
            bool show = state == RunState.GameOver;
            root.SetActive(show);
            if (!show) return;

            float t = gameManager.Timer.Elapsed;
            timeLabel.text = $"Você sobreviveu {Mathf.FloorToInt(t / 60f):00}:{Mathf.FloorToInt(t % 60f):00}";
            if (restartButton != null) restartButton.Select();
        }

        private void Update()
        {
            if (!root.activeSelf) return;

            // Unscaled input still works while Time.timeScale is 0.
            var kb = Keyboard.current;
            var pad = Gamepad.current;
            if ((kb != null && kb.rKey.wasPressedThisFrame) ||
                (pad != null && pad.startButton.wasPressedThisFrame))
                gameManager.Restart();
        }
    }
}
