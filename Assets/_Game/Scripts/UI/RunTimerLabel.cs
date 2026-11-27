using HunterVsDino.Core;
using UnityEngine;

namespace HunterVsDino.UI
{
    /// <summary>Countdown to the T-Rex at the top of the screen. Shows a warning once the boss is due.</summary>
    public class RunTimerLabel : MonoBehaviour
    {
        [SerializeField] private RunTimer runTimer;
        [SerializeField] private TMPro.TMP_Text label;
        [SerializeField] private string bossText = "T-REX!";
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color warningColor = new(1f, 0.75f, 0.3f);
        [SerializeField] private Color bossColor = new(1f, 0.3f, 0.25f);
        [Tooltip("Last seconds shown in the warning color.")]
        [SerializeField] private float warningSeconds = 10f;

        private int lastShown = -1;

        private void Update()
        {
            if (runTimer == null) return;

            if (runTimer.IsFinished)
            {
                if (lastShown == -2) return;
                label.text = bossText;
                label.color = bossColor;
                lastShown = -2;
                return;
            }

            int remaining = Mathf.CeilToInt(runTimer.Remaining);
            if (remaining == lastShown) return; // only rebuild the string once per second
            lastShown = remaining;
            label.text = $"{remaining / 60:00}:{remaining % 60:00}";
            label.color = runTimer.Remaining <= warningSeconds ? warningColor : normalColor;
        }
    }
}
