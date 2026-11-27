using System;
using System.Collections.Generic;
using HunterVsDino.Players;
using UnityEngine;

namespace HunterVsDino.Combat
{
    /// <summary>
    /// The weapons a hunter carries (up to <see cref="MaxWeapons"/>). Spawns each weapon's prefab
    /// under <c>weaponsRoot</c> and levels them up. The starting weapon is given on Start.
    /// </summary>
    [RequireComponent(typeof(PlayerStats))]
    public class PlayerWeapons : MonoBehaviour
    {
        [SerializeField] private WeaponData startingWeapon;
        [SerializeField] private Transform weaponsRoot;
        [SerializeField, Min(1)] private int maxWeapons = 6;

        private readonly List<Weapon> weapons = new();
        private PlayerStats stats;

        public IReadOnlyList<Weapon> Weapons => weapons;
        public int MaxWeapons => maxWeapons;
        public bool HasFreeSlot => weapons.Count < maxWeapons;

        public event Action<PlayerWeapons> Changed;

        private void Awake()
        {
            stats = GetComponent<PlayerStats>();
            if (weaponsRoot == null) weaponsRoot = transform;
        }

        private void Start()
        {
            if (startingWeapon != null && weapons.Count == 0) Add(startingWeapon);
        }

        public Weapon Get(WeaponData data)
        {
            foreach (var w in weapons) if (w.Data == data) return w;
            return null;
        }

        public bool Has(WeaponData data) => Get(data) != null;

        public Weapon Add(WeaponData data)
        {
            if (data == null || data.weaponPrefab == null)
            {
                Debug.LogError($"Weapon '{(data ? data.name : "null")}' has no weaponPrefab.", this);
                return null;
            }
            if (Has(data)) return Get(data);
            if (!HasFreeSlot) return null;

            var weapon = Instantiate(data.weaponPrefab, weaponsRoot);
            weapon.name = data.displayName;
            weapon.transform.localPosition = Vector3.zero;
            weapon.Init(data, stats);
            weapons.Add(weapon);
            Changed?.Invoke(this);
            return weapon;
        }

        public bool LevelUp(WeaponData data)
        {
            var weapon = Get(data);
            if (weapon == null || !weapon.LevelUp()) return false;
            Changed?.Invoke(this);
            return true;
        }
    }
}
