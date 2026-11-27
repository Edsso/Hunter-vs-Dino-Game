using HunterVsDino.Combat;
using HunterVsDino.Players;
using UnityEngine;

namespace HunterVsDino.UI
{
    /// <summary>
    /// HUD bar for a hunter's health. With no Health assigned it binds to the local player (index 0).
    /// The fill is a child RectTransform scaled on X, so it needs no sprite.
    /// </summary>
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Health target;
        [SerializeField] private RectTransform fill;
        [SerializeField] private TMPro.TMP_Text label;

        private void Update()
        {
            if (target == null)
            {
                if (PlayerRegistry.All.Count == 0) return;
                Bind(PlayerRegistry.All[0].Health);
            }
        }

        public void Bind(Health health)
        {
            if (target != null) target.Changed -= Refresh;
            target = health;
            if (target != null)
            {
                target.Changed += Refresh;
                Refresh(target);
            }
        }

        private void OnDestroy()
        {
            if (target != null) target.Changed -= Refresh;
        }

        private void Refresh(Health h)
        {
            if (fill != null) fill.localScale = new Vector3(h.Normalized, 1f, 1f);
            if (label != null) label.text = $"{Mathf.CeilToInt(h.Current)} / {Mathf.CeilToInt(h.Max)}";
        }
    }
}
