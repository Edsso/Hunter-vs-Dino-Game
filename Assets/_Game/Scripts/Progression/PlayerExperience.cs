using System;
using UnityEngine;

namespace HunterVsDino.Progression
{
    /// <summary>
    /// A hunter's XP and level. XP needed per level grows linearly:
    /// <c>baseXP + (level - 1) * xpIncrement</c>. Level-ups that happen at once are queued
    /// (<see cref="PendingLevelUps"/>) so the upgrade screen can show one choice per level.
    /// </summary>
    public class PlayerExperience : MonoBehaviour
    {
        [SerializeField, Min(1)] private int baseXP = 5;
        [SerializeField, Min(0)] private int xpIncrement = 8;

        public int Level { get; private set; } = 1;
        public int CurrentXP { get; private set; }
        public int XPToNextLevel => baseXP + (Level - 1) * xpIncrement;
        public float Normalized => (float)CurrentXP / XPToNextLevel;

        /// <summary>Level-ups not yet spent on an upgrade choice.</summary>
        public int PendingLevelUps { get; private set; }

        public event Action<PlayerExperience> XPChanged;
        /// <summary>New level reached.</summary>
        public event Action<int> LeveledUp;

        public void AddXP(int amount)
        {
            if (amount <= 0) return;
            CurrentXP += amount;

            while (CurrentXP >= XPToNextLevel)
            {
                CurrentXP -= XPToNextLevel;
                Level++;
                PendingLevelUps++;
                LeveledUp?.Invoke(Level);
            }

            XPChanged?.Invoke(this);
        }

        /// <summary>Called by the upgrade screen after the player picks an option.</summary>
        public bool ConsumePendingLevelUp()
        {
            if (PendingLevelUps <= 0) return false;
            PendingLevelUps--;
            return true;
        }
    }
}
