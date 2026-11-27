using System.Collections.Generic;
using UnityEngine;

namespace HunterVsDino.Players
{
    /// <summary>
    /// Every active player in the run. Works the same for 1 player or 4 (co-op).
    /// </summary>
    public static class PlayerRegistry
    {
        private static readonly List<Player> players = new();

        public static IReadOnlyList<Player> All => players;

        public static void Register(Player player)
        {
            if (!players.Contains(player)) players.Add(player);
        }

        public static void Unregister(Player player) => players.Remove(player);

        /// <summary>Nearest player to a world position, or null if none.</summary>
        public static Player GetNearest(Vector2 position)
        {
            Player nearest = null;
            float best = float.MaxValue;
            for (int i = 0; i < players.Count; i++)
            {
                float d = ((Vector2)players[i].transform.position - position).sqrMagnitude;
                if (d < best)
                {
                    best = d;
                    nearest = players[i];
                }
            }
            return nearest;
        }

        // Static state survives Play mode restarts when Domain Reload is off; clear it on load.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics() => players.Clear();
    }
}
