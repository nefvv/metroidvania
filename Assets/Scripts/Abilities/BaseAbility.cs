using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewAbility", menuName = "Abilities/Base Ability")]
public abstract class BaseAbility : ScriptableObject
{
    [Header("Basic Info")]
    public string abilityName;
    [TextArea(3, 5)]
    public string description;
    public Sprite icon;
    public AbilityId abilityId;
    
    [Header("Unlock Requirements")]
    public bool startingAbility = false;
    public List<AbilityId> requiredAbilities = new List<AbilityId>();
    public string unlockCondition; // e.g., "Defeat Boss", "Find Item", etc.
    
    [Header("Gameplay")]
    public float cooldown = 0f;
    public float energyCost = 0f;
    public bool isPassive = false;
    
    /// <summary>
    /// Called when the ability is unlocked
    /// </summary>
    public virtual void OnUnlock(GameObject player) 
    {
        Debug.Log($"Unlocked ability: {abilityName}");
    }
    
    /// <summary>
    /// Called when the ability is used (for active abilities)
    /// </summary>
    public virtual void OnUse(GameObject player) 
    {
        if (isPassive)
        {
            Debug.LogWarning($"{abilityName} is a passive ability and cannot be used directly");
            return;
        }
    }
    
    /// <summary>
    /// Check if all requirements are met to unlock this ability
    /// </summary>
    public virtual bool CanUnlock(PlayerAbilities playerAbilities)
    {
        foreach (var requiredAbility in requiredAbilities)
        {
            if (!playerAbilities.Has(requiredAbility))
                return false;
        }
        return true;
    }
    
    /// <summary>
    /// Get a formatted description with unlock requirements
    /// </summary>
    public virtual string GetFullDescription()
    {
        string fullDesc = description;
        
        if (requiredAbilities.Count > 0)
        {
            fullDesc += "\n\nRequires: ";
            for (int i = 0; i < requiredAbilities.Count; i++)
            {
                fullDesc += requiredAbilities[i].ToString();
                if (i < requiredAbilities.Count - 1)
                    fullDesc += ", ";
            }
        }
        
        if (!string.IsNullOrEmpty(unlockCondition))
        {
            fullDesc += "\nUnlock Condition: " + unlockCondition;
        }
        
        return fullDesc;
    }
}