using HunterVsDino.Players;
using HunterVsDino.Progression;
using UnityEngine;

namespace HunterVsDino.UI
{
    /// <summary>XP bar across the top of the screen plus the level label. Binds to the local player.</summary>
    public class XPBar : MonoBehaviour
    {
        [SerializeField] private PlayerExperience target;
        [SerializeField] private RectTransform fill;
        [SerializeField] private TMPro.TMP_Text levelLabel;

        private void Update()
        {
            if (target == null && PlayerRegistry.All.Count > 0)
                Bind(PlayerRegistry.All[0].Experience);
        }

        public void Bind(PlayerExperience experience)
        {
            if (target != null) target.XPChanged -= Refresh;
            target = experience;
            if (target != null)
            {
                target.XPChanged += Refresh;
                Refresh(target);
            }
        }

        private void OnDestroy()
        {
            if (target != null) target.XPChanged -= Refresh;
        }

        private void Refresh(PlayerExperience xp)
        {
            if (fill != null) fill.localScale = new Vector3(Mathf.Clamp01(xp.Normalized), 1f, 1f);
            if (levelLabel != null) levelLabel.text = $"Nv {xp.Level}";
        }
    }
}
