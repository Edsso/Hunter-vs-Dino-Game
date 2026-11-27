using System;
using HunterVsDino.Progression;
using UnityEngine;

namespace HunterVsDino.UI
{
    /// <summary>One clickable option on the level-up screen.</summary>
    public class UpgradeCard : MonoBehaviour
    {
        [SerializeField] private UnityEngine.UI.Button button;
        [SerializeField] private UnityEngine.UI.Image accent;
        [SerializeField] private TMPro.TMP_Text title;
        [SerializeField] private TMPro.TMP_Text levelLabel;
        [SerializeField] private TMPro.TMP_Text description;
        [SerializeField] private TMPro.TMP_Text hotkey;

        private UpgradeChoice choice;
        private Action<UpgradeChoice> onPicked;

        public UnityEngine.UI.Button Button => button;

        private void Awake()
        {
            button.onClick.AddListener(() => onPicked?.Invoke(choice));
        }

        public void Show(UpgradeChoice upgradeChoice, int index, Action<UpgradeChoice> picked)
        {
            choice = upgradeChoice;
            onPicked = picked;
            gameObject.SetActive(true);

            title.text = choice.Title;
            description.text = choice.Description;
            accent.color = choice.Accent;
            levelLabel.text = choice.LevelText;
            if (hotkey != null) hotkey.text = (index + 1).ToString();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            choice = null;
            onPicked = null;
        }
    }
}
