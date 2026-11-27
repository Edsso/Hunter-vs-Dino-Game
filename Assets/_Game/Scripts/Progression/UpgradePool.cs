using System.Collections.Generic;
using HunterVsDino.Combat;
using UnityEngine;

namespace HunterVsDino.Progression
{
    /// <summary>What can show up on the level-up screen in a run: passive upgrades and weapons.</summary>
    [CreateAssetMenu(fileName = "UpgradePool", menuName = "Hunter vs Dino/Upgrade Pool")]
    public class UpgradePool : ScriptableObject
    {
        public List<UpgradeData> upgrades = new();

        [Tooltip("Weapons that can be found (as new weapons) and leveled up.")]
        public List<WeaponData> weapons = new();

        [Tooltip("Offered when everything else is maxed (e.g. a heal).")]
        public UpgradeData fallback;
    }
}
