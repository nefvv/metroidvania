using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

[System.Serializable]
public class AbilityUnlockCondition
{
    public BaseAbility ability;
    public string conditionKey; // e.g., "boss_defeated", "item_collected", etc.
    public bool unlocked = false;
}

public class AbilityManager : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private PlayerAbilities playerAbilities;
    [SerializeField] private List<AbilityUnlockCondition> conditionalUnlocks;
    
    [Header("Events")]
    public UnityEvent<string> OnConditionMet = new UnityEvent<string>();
    
    private Dictionary<string, List<AbilityUnlockCondition>> _conditionLookup;
    
    public static AbilityManager Instance { get; private set; }
    
    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeConditions();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        if (playerAbilities == null)
        {
            playerAbilities = FindFirstObjectByType<PlayerAbilities>();
        }
        
        // Check for any abilities that should be unlocked immediately
        CheckAllConditions();
    }
    
    private void InitializeConditions()
    {
        _conditionLookup = new Dictionary<string, List<AbilityUnlockCondition>>();
        
        foreach (var condition in conditionalUnlocks)
        {
            if (!_conditionLookup.ContainsKey(condition.conditionKey))
            {
                _conditionLookup[condition.conditionKey] = new List<AbilityUnlockCondition>();
            }
            _conditionLookup[condition.conditionKey].Add(condition);
        }
    }
    
    /// <summary>
    /// Call this method when a condition is met (e.g., boss defeated, item collected)
    /// </summary>
    public void TriggerCondition(string conditionKey)
    {
        Debug.Log($"Condition triggered: {conditionKey}");
        
        if (_conditionLookup.TryGetValue(conditionKey, out List<AbilityUnlockCondition> conditions))
        {
            foreach (var condition in conditions)
            {
                if (!condition.unlocked && condition.ability != null)
                {
                    // Check if the ability can be unlocked (prerequisites met)
                    if (playerAbilities.CanUnlockAbility(condition.ability.abilityId))
                    {
                        playerAbilities.UnlockAbility(condition.ability);
                        condition.unlocked = true;
                        
                        // Show unlock notification
                        ShowAbilityUnlockedNotification(condition.ability);
                    }
                    else
                    {
                        Debug.Log($"Cannot unlock {condition.ability.abilityName} - prerequisites not met");
                    }
                }
            }
        }
        
        OnConditionMet?.Invoke(conditionKey);
    }
    
    /// <summary>
    /// Manually unlock an ability (for testing or special cases)
    /// </summary>
    public void UnlockAbility(AbilityId abilityId)
    {
        if (playerAbilities != null)
        {
            playerAbilities.UnlockAbility(abilityId);
        }
    }
    
    /// <summary>
    /// Check if all prerequisites are met for unlockable abilities
    /// </summary>
    public void CheckAllConditions()
    {
        if (playerAbilities == null) return;
        
        foreach (var condition in conditionalUnlocks)
        {
            if (!condition.unlocked && condition.ability != null)
            {
                // If this ability has no unlock condition key, check if prerequisites are met
                if (string.IsNullOrEmpty(condition.conditionKey))
                {
                    if (playerAbilities.CanUnlockAbility(condition.ability.abilityId))
                    {
                        playerAbilities.UnlockAbility(condition.ability);
                        condition.unlocked = true;
                        ShowAbilityUnlockedNotification(condition.ability);
                    }
                }
            }
        }
    }
    
    private void ShowAbilityUnlockedNotification(BaseAbility ability)
    {
        // This could trigger a UI notification, play a sound, etc.
        Debug.Log($"🎉 NEW ABILITY UNLOCKED: {ability.abilityName}!");
        
        // You could add more sophisticated notification logic here
        // For example, showing a popup UI, playing an unlock sound, etc.
    }
    
    /// <summary>
    /// Get the unlock condition for a specific ability
    /// </summary>
    public string GetUnlockCondition(AbilityId abilityId)
    {
        foreach (var condition in conditionalUnlocks)
        {
            if (condition.ability != null && condition.ability.abilityId == abilityId)
            {
                return condition.conditionKey;
            }
        }
        return null;
    }
    
    /// <summary>
    /// Check if a specific condition has been met
    /// </summary>
    public bool IsConditionMet(string conditionKey)
    {
        if (_conditionLookup.TryGetValue(conditionKey, out List<AbilityUnlockCondition> conditions))
        {
            return conditions.Exists(c => c.unlocked);
        }
        return false;
    }
    
    // Helper methods for common unlock triggers
    public void OnBossDefeated(string bossName) => TriggerCondition($"boss_defeated_{bossName}");
    public void OnItemCollected(string itemName) => TriggerCondition($"item_collected_{itemName}");
    public void OnAreaEntered(string areaName) => TriggerCondition($"area_entered_{areaName}");
    public void OnQuestCompleted(string questName) => TriggerCondition($"quest_completed_{questName}");
}