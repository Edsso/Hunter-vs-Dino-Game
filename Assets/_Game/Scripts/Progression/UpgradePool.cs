using System.Collections.Generic;
using UnityEngine;

namespace HunterVsDino.Progression
{
    /// <summary>The upgrades that can show up on the level-up screen in a run.</summary>
    [CreateAssetMenu(fileName = "UpgradePool", menuName = "Hunter vs Dino/Upgrade Pool")]
    public class UpgradePool : ScriptableObject
    {
        public List<UpgradeData> upgrades = new();

        [Tooltip("Offered when every other upgrade is maxed (e.g. a heal).")]
        public UpgradeData fallback;
    }
}
