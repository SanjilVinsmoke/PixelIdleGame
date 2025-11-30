using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using Utils.Managers;

namespace Managers
{
    public enum PlayerAbility
    {
        None = 0,
        Dash = 1,
        DoubleJump = 2,
        DownSmash = 3
    }

    public class PlayerAbilityManager : SingletonMonoBehavior<PlayerAbilityManager>
    {
        [Header("Debug")]
        [SerializeField] private List<PlayerAbility> unlockedAbilities = new List<PlayerAbility>();

        public event Action<PlayerAbility> OnAbilityUnlocked;

        protected override void InitializeSingleton()
        {
            Debug.Log("PlayerAbilityManager Initialized");
        }

        public bool HasAbility(PlayerAbility ability)
        {
            return unlockedAbilities.Contains(ability);
        }

        public void UnlockAbility(PlayerAbility ability)
        {
            if (!HasAbility(ability))
            {
                unlockedAbilities.Add(ability);
                Debug.Log($"Ability Unlocked: {ability}");
                OnAbilityUnlocked?.Invoke(ability);
            }
        }

        public void LockAbility(PlayerAbility ability)
        {
            if (HasAbility(ability))
            {
                unlockedAbilities.Remove(ability);
                Debug.Log($"Ability Locked: {ability}");
            }
        }
        
        // For Save System later
        public List<PlayerAbility> GetUnlockedAbilities()
        {
            return new List<PlayerAbility>(unlockedAbilities);
        }

        public void LoadAbilities(List<PlayerAbility> abilities)
        {
            unlockedAbilities = new List<PlayerAbility>(abilities);
            foreach (var ability in unlockedAbilities)
            {
                // Re-trigger events if needed, or just silent load
                // OnAbilityUnlocked?.Invoke(ability); 
            }
        }
    }
}

