using HunterVsDino.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HunterVsDino.UI
{
    /// <summary>
    /// End-of-run screen for both outcomes: death ("devorado") and victory (T-Rex down).
    /// Shows the time and restarts the run (button, R on keyboard, or Start on gamepad).
    /// </summary>
    public class GameOverPanel : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private GameObject root;
        [SerializeField] private TMPro.TMP_Text titleLabel;
        [SerializeField] private TMPro.TMP_Text timeLabel;
        [SerializeField] private UnityEngine.UI.Button restartButton;

        [Header("Texts")]
        [SerializeField] private string defeatTitle = "VOCÊ FOI DEVORADO";
        [SerializeField] private Color defeatColor = new(0.95f, 0.75f, 0.35f);
        [SerializeField] private string victoryTitle = "O T-REX CAIU!";
        [SerializeField] private Color victoryColor = new(0.55f, 0.95f, 0.45f);

        private void Awake()
        {
            root.SetActive(false);
            if (restartButton != null) restartButton.onClick.AddListener(gameManager.Restart);
        }

        private void OnEnable() => gameManager.StateChanged += OnStateChanged;
        private void OnDisable() => gameManager.StateChanged -= OnStateChanged;

        private void OnStateChanged(RunState state)
        {
            bool victory = state == RunState.Victory;
            bool show = victory || state == RunState.GameOver;
            root.SetActive(show);
            if (!show) return;

            if (titleLabel != null)
            {
                titleLabel.text = victory ? victoryTitle : defeatTitle;
                titleLabel.color = victory ? victoryColor : defeatColor;
            }

            float t = gameManager.Timer.Elapsed;
            string time = $"{Mathf.FloorToInt(t / 60f):00}:{Mathf.FloorToInt(t % 60f):00}";
            timeLabel.text = victory ? $"Caçada completa em {time}" : $"Você sobreviveu {time}";
            if (restartButton != null) restartButton.Select();
        }

        private void Update()
        {
            if (!root.activeSelf) return;

            // Device input still works while Time.timeScale is 0.
            var kb = Keyboard.current;
            var pad = Gamepad.current;
            if ((kb != null && kb.rKey.wasPressedThisFrame) ||
                (pad != null && pad.startButton.wasPressedThisFrame))
                gameManager.Restart();
        }
    }
}
