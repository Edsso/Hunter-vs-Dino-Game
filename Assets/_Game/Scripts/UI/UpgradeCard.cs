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

        private UpgradeData upgrade;
        private Action<UpgradeData> onPicked;

        public UnityEngine.UI.Button Button => button;

        private void Awake()
        {
            button.onClick.AddListener(() => onPicked?.Invoke(upgrade));
        }

        public void Show(UpgradeData data, int currentLevel, int index, Action<UpgradeData> picked)
        {
            upgrade = data;
            onPicked = picked;
            gameObject.SetActive(true);

            title.text = data.displayName;
            description.text = data.description;
            accent.color = data.accentColor;
            levelLabel.text = currentLevel == 0 ? "NOVO" : data.maxLevel > 0 ? $"Nv {currentLevel} → {currentLevel + 1}" : "";
            if (hotkey != null) hotkey.text = (index + 1).ToString();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            upgrade = null;
            onPicked = null;
        }
    }
}
