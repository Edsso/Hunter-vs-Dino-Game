using System.Collections.Generic;
using HunterVsDino.Combat;
using HunterVsDino.Players;
using UnityEngine;

namespace HunterVsDino.Progression
{
    /// <summary>
    /// Builds the level-up options for a hunter (passives, new weapons, weapon levels) and applies
    /// the chosen one. Tracks how many times each passive was picked.
    /// </summary>
    [RequireComponent(typeof(PlayerStats), typeof(PlayerWeapons))]
    public class PlayerUpgrades : MonoBehaviour
    {
        private readonly Dictionary<UpgradeData, int> levels = new();
        private PlayerStats stats;
        private PlayerWeapons weapons;

        private void Awake()
        {
            stats = GetComponent<PlayerStats>();
            weapons = GetComponent<PlayerWeapons>();
        }

        public int GetLevel(UpgradeData upgrade) => levels.TryGetValue(upgrade, out int lvl) ? lvl : 0;

        public bool IsMaxed(UpgradeData upgrade) => upgrade.maxLevel > 0 && GetLevel(upgrade) >= upgrade.maxLevel;

        /// <summary>Every option currently possible for this hunter.</summary>
        public List<UpgradeChoice> GetCandidates(UpgradePool pool)
        {
            var list = new List<UpgradeChoice>();

            foreach (var u in pool.upgrades)
                if (u != null && !IsMaxed(u)) list.Add(UpgradeChoice.ForPassive(u, GetLevel(u)));

            foreach (var w in pool.weapons)
            {
                if (w == null) continue;
                var owned = weapons.Get(w);
                if (owned != null)
                {
                    if (!owned.IsMaxLevel) list.Add(UpgradeChoice.ForWeaponLevel(w, owned.Level));
                }
                else if (weapons.HasFreeSlot && w.weaponPrefab != null)
                {
                    list.Add(UpgradeChoice.ForNewWeapon(w));
                }
            }
            return list;
        }

        /// <summary>Up to <paramref name="count"/> different options, weighted at random.</summary>
        public List<UpgradeChoice> RollChoices(UpgradePool pool, int count)
        {
            var candidates = GetCandidates(pool);
            var result = new List<UpgradeChoice>();

            while (result.Count < count && candidates.Count > 0)
            {
                float total = 0f;
                foreach (var c in candidates) total += c.Weight;
                float roll = Random.value * total;
                int pick = candidates.Count - 1;
                for (int i = 0; i < candidates.Count; i++)
                {
                    roll -= candidates[i].Weight;
                    if (roll <= 0f) { pick = i; break; }
                }
                result.Add(candidates[pick]);
                candidates.RemoveAt(pick);
            }

            if (result.Count == 0 && pool.fallback != null)
                result.Add(UpgradeChoice.ForPassive(pool.fallback, GetLevel(pool.fallback)));
            return result;
        }

        public void Apply(UpgradeChoice choice)
        {
            switch (choice.Kind)
            {
                case UpgradeKind.Passive:
                    levels[choice.Passive] = GetLevel(choice.Passive) + 1;
                    foreach (var mod in choice.Passive.modifiers) stats.Apply(mod);
                    break;
                case UpgradeKind.NewWeapon:
                    weapons.Add(choice.Weapon);
                    break;
                case UpgradeKind.WeaponLevel:
                    weapons.LevelUp(choice.Weapon);
                    break;
            }
        }

        /// <summary>Shortcut for passives (debug tools, tests).</summary>
        public void Apply(UpgradeData upgrade) => Apply(UpgradeChoice.ForPassive(upgrade, GetLevel(upgrade)));
    }
}
