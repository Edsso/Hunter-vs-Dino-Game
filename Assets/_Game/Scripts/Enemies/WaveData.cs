using System;
using System.Collections.Generic;
using UnityEngine;

namespace HunterVsDino.Enemies
{
    /// <summary>
    /// Spawn schedule for a whole run. Each entry spawns one enemy type during a time window,
    /// with the spawn rate going from <see cref="SpawnEntry.ratePerSecondStart"/> to
    /// <see cref="SpawnEntry.ratePerSecondEnd"/> across that window.
    /// </summary>
    [CreateAssetMenu(fileName = "Waves", menuName = "Hunter vs Dino/Wave Schedule")]
    public class WaveData : ScriptableObject
    {
        [Serializable]
        public class SpawnEntry
        {
            public EnemyData enemy;

            [Tooltip("Run time (seconds) when this entry starts spawning.")]
            [Min(0f)] public float startTime;
            [Tooltip("Run time (seconds) when it stops. 900 = 15 minutes.")]
            [Min(0f)] public float endTime = 900f;

            [Tooltip("Groups per second at startTime.")]
            [Min(0f)] public float ratePerSecondStart = 0.5f;
            [Tooltip("Groups per second at endTime.")]
            [Min(0f)] public float ratePerSecondEnd = 2f;

            [Tooltip("Enemies per group. Raptors hunt in packs.")]
            [Min(1)] public int groupSize = 1;
            [Tooltip("How spread out a group spawns, in world units.")]
            [Min(0f)] public float groupSpread = 1f;

            public bool IsActive(float time) => time >= startTime && time < endTime;

            public float RateAt(float time)
            {
                float t = Mathf.InverseLerp(startTime, endTime, time);
                return Mathf.Lerp(ratePerSecondStart, ratePerSecondEnd, t);
            }
        }

        [Tooltip("Hard cap of enemies alive at once (performance safety net).")]
        [Min(1)] public int maxAlive = 300;

        [Tooltip("Run length the entry times were written for (900 = 15 min). If the RunTimer is shorter " +
                 "(e.g. 60 s for testing), the whole schedule is compressed to fit.")]
        [Min(1f)] public float designedDuration = 900f;

        [Header("Difficulty over the run (X = 0..1 of the run)")]
        [Tooltip("Enemy max health multiplier.")]
        public AnimationCurve healthMultiplier = AnimationCurve.Linear(0f, 1f, 1f, 4f);
        [Tooltip("Enemy contact damage multiplier.")]
        public AnimationCurve damageMultiplier = AnimationCurve.Linear(0f, 1f, 1f, 2f);

        public List<SpawnEntry> entries = new();
    }
}
