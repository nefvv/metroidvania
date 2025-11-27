using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class AbilityUnlockedEvent : UnityEvent<BaseAbility> { }

public class PlayerAbilities : MonoBehaviour
{
    [Header("Starting Abilities")]
    [SerializeField] private List<BaseAbility> startingAbilities;
    
    [Header("All Available Abilities")]
    [SerializeField] private List<BaseAbility> allAbilities;
    
    [Header("Events")]
    public AbilityUnlockedEvent OnAbilityUnlocked = new AbilityUnlockedEvent();
    
    private HashSet<AbilityId> _unlockedAbilityIds;
    private Dictionary<AbilityId, BaseAbility> _abilityLookup;

    void Awake()
    {
        InitializeAbilities();
    }
    
    void Start()
    {
        // Unlock starting abilities
        foreach (var ability in startingAbilities)
        {
            if (ability != null)
            {
                Debug.Log($"Starting ability found: Name='{ability.abilityName}', ID={ability.abilityId}");
                UnlockAbility(ability, false); // Don't trigger events for starting abilities
            }
            else
            {
                Debug.LogWarning("Null ability found in starting abilities list!");
            }
        }
    }
    
    private void InitializeAbilities()
    {
        _unlockedAbilityIds = new HashSet<AbilityId>();
        _abilityLookup = new Dictionary<AbilityId, BaseAbility>();
        
        // Build lookup dictionary
        foreach (var ability in allAbilities)
        {
            if (ability != null)
            {
                _abilityLookup[ability.abilityId] = ability;
            }
        }
    }
    
    public bool Has(AbilityId id) => _unlockedAbilityIds.Contains(id);
    
    public BaseAbility GetAbility(AbilityId id)
    {
        _abilityLookup.TryGetValue(id, out BaseAbility ability);
        return ability;
    }
    
    public void UnlockAbility(AbilityId id, bool triggerEvents = true)
    {
        if (_abilityLookup.TryGetValue(id, out BaseAbility ability))
        {
            UnlockAbility(ability, triggerEvents);
        }
        else
        {
            Debug.LogWarning($"Ability with ID {id} not found in ability lookup!");
        }
    }
    
    public void UnlockAbility(BaseAbility ability, bool triggerEvents = true)
    {
        if (ability == null) return;
        
        if (_unlockedAbilityIds.Add(ability.abilityId))
        {
            Debug.Log($"Unlocked ability: {ability.abilityName}");
            
            // Call the ability's unlock method
            ability.OnUnlock(gameObject);
            
            // Trigger events
            if (triggerEvents)
            {
                OnAbilityUnlocked?.Invoke(ability);
            }
        }
    }
    
    public bool CanUnlockAbility(AbilityId id)
    {
        if (_abilityLookup.TryGetValue(id, out BaseAbility ability))
        {
            return ability.CanUnlock(this) && !Has(id);
        }
        return false;
    }
    
    public void UseAbility(AbilityId id)
    {
        if (Has(id) && _abilityLookup.TryGetValue(id, out BaseAbility ability))
        {
            ability.OnUse(gameObject);
        }
    }
    
    public List<BaseAbility> GetUnlockedAbilities()
    {
        return _unlockedAbilityIds.Select(id => _abilityLookup[id]).ToList();
    }
    
    public List<BaseAbility> GetLockedAbilities()
    {
        return allAbilities.Where(ability => ability != null && !Has(ability.abilityId)).ToList();
    }
    
    public List<BaseAbility> GetUnlockableAbilities()
    {
        return GetLockedAbilities().Where(ability => ability.CanUnlock(this)).ToList();
    }
    
    // Legacy methods for compatibility
    public void Grant(AbilityId id) => UnlockAbility(id);
    public void Revoke(AbilityId id) => _unlockedAbilityIds.Remove(id);
}
