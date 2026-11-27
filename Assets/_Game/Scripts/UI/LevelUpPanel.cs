using System.Collections.Generic;
using HunterVsDino.Core;
using HunterVsDino.Players;
using HunterVsDino.Progression;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HunterVsDino.UI
{
    /// <summary>
    /// Level-up screen for the local hunter: pauses the run, offers 3 upgrades from the pool and
    /// applies the one picked (click, keys 1-3, or gamepad/keyboard navigation + submit).
    /// Several pending level-ups are shown one after another.
    /// </summary>
    public class LevelUpPanel : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private UpgradePool pool;
        [SerializeField] private GameObject root;
        [SerializeField] private TMPro.TMP_Text titleLabel;
        [SerializeField] private UpgradeCard[] cards;
        [Tooltip("Ignore input for a moment so a held attack/submit key doesn't pick by accident.")]
        [SerializeField] private float inputDelay = 0.35f;

        private Player player;
        private List<UpgradeData> choices = new();
        private float acceptInputAt;

        private void Awake()
        {
            root.SetActive(false);
        }

        private void Update()
        {
            if (player == null && PlayerRegistry.All.Count > 0) Bind(PlayerRegistry.All[0]);

            // A level-up that happened while another screen was open waits here.
            if (!root.activeSelf && player != null && player.Experience.PendingLevelUps > 0
                && gameManager.State == RunState.Playing)
                Open();

            if (root.activeSelf) HandleHotkeys();
        }

        private void Bind(Player p)
        {
            player = p;
            player.Experience.LeveledUp += OnLeveledUp;
        }

        private void OnDestroy()
        {
            if (player != null) player.Experience.LeveledUp -= OnLeveledUp;
        }

        private void OnLeveledUp(int level)
        {
            if (!root.activeSelf) Open();
        }

        private void Open()
        {
            if (player.IsDead || !gameManager.EnterLevelUp()) return;

            choices = player.GetComponent<PlayerUpgrades>().RollChoices(pool, cards.Length);
            if (choices.Count == 0)
            {
                // Nothing to offer: spend the level and carry on.
                player.Experience.ConsumePendingLevelUp();
                gameManager.ExitLevelUp();
                return;
            }

            var upgrades = player.GetComponent<PlayerUpgrades>();
            for (int i = 0; i < cards.Length; i++)
            {
                if (i < choices.Count) cards[i].Show(choices[i], upgrades.GetLevel(choices[i]), i, Pick);
                else cards[i].Hide();
            }

            titleLabel.text = $"NÍVEL {player.Experience.Level - player.Experience.PendingLevelUps + 1}!";
            root.SetActive(true);
            acceptInputAt = Time.unscaledTime + inputDelay;
            SetCardsInteractable(false);
        }

        private void SetCardsInteractable(bool value)
        {
            foreach (var c in cards) c.Button.interactable = value;
            if (value && choices.Count > 0) cards[0].Button.Select();
        }

        private void HandleHotkeys()
        {
            if (Time.unscaledTime < acceptInputAt) return;
            if (!cards[0].Button.interactable) SetCardsInteractable(true);

            var kb = Keyboard.current;
            if (kb == null) return;
            if (kb.digit1Key.wasPressedThisFrame || kb.numpad1Key.wasPressedThisFrame) PickIndex(0);
            else if (kb.digit2Key.wasPressedThisFrame || kb.numpad2Key.wasPressedThisFrame) PickIndex(1);
            else if (kb.digit3Key.wasPressedThisFrame || kb.numpad3Key.wasPressedThisFrame) PickIndex(2);
        }

        private void PickIndex(int index)
        {
            if (index < choices.Count) Pick(choices[index]);
        }

        private void Pick(UpgradeData upgrade)
        {
            if (!root.activeSelf || Time.unscaledTime < acceptInputAt) return;

            player.GetComponent<PlayerUpgrades>().Apply(upgrade);
            player.Experience.ConsumePendingLevelUp();

            root.SetActive(false);
            foreach (var c in cards) c.Hide();
            UnityEngine.EventSystems.EventSystem.current?.SetSelectedGameObject(null);
            gameManager.ExitLevelUp();
            // If more level-ups are pending, Update() opens the screen again next frame.
        }
    }
}
