using System.Collections.Generic;
using HunterVsDino.Players;
using UnityEngine;

namespace HunterVsDino.Progression
{
    /// <summary>
    /// Which upgrades a hunter has picked and how many times. Rolls the options for the level-up
    /// screen and applies the chosen one through <see cref="PlayerStats"/>.
    /// </summary>
    [RequireComponent(typeof(PlayerStats))]
    public class PlayerUpgrades : MonoBehaviour
    {
        private readonly Dictionary<UpgradeData, int> levels = new();
        private PlayerStats stats;

        private void Awake()
        {
            stats = GetComponent<PlayerStats>();
        }

        public int GetLevel(UpgradeData upgrade) => levels.TryGetValue(upgrade, out int lvl) ? lvl : 0;

        public bool IsMaxed(UpgradeData upgrade) => upgrade.maxLevel > 0 && GetLevel(upgrade) >= upgrade.maxLevel;

        /// <summary>Up to <paramref name="count"/> different, non-maxed upgrades, weighted at random.</summary>
        public List<UpgradeData> RollChoices(UpgradePool pool, int count)
        {
            var candidates = new List<UpgradeData>();
            foreach (var u in pool.upgrades)
                if (u != null && !IsMaxed(u)) candidates.Add(u);

            var result = new List<UpgradeData>();
            while (result.Count < count && candidates.Count > 0)
            {
                float total = 0f;
                foreach (var c in candidates) total += c.weight;
                float roll = Random.value * total;
                int pick = candidates.Count - 1;
                for (int i = 0; i < candidates.Count; i++)
                {
                    roll -= candidates[i].weight;
                    if (roll <= 0f) { pick = i; break; }
                }
                result.Add(candidates[pick]);
                candidates.RemoveAt(pick);
            }

            if (result.Count == 0 && pool.fallback != null) result.Add(pool.fallback);
            return result;
        }

        public void Apply(UpgradeData upgrade)
        {
            levels[upgrade] = GetLevel(upgrade) + 1;
            foreach (var mod in upgrade.modifiers) stats.Apply(mod);
        }
    }
}
