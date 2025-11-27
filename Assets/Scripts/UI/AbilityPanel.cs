using UnityEngine;
using System.Collections.Generic;

public class AbilityPanel : MonoBehaviour
{
    [Header("Configuration")]
    public GameObject abilityItemPrefab;
    public Transform abilityContainer;
    public PlayerAbilities playerAbilities;
    
    [Header("Filtering")]
    public bool showOnlyUnlocked = false;
    public bool showOnlyUnlockable = false;
    
    private List<AbilityUIItem> _abilityItems = new List<AbilityUIItem>();
    
    void Start()
    {
        if (playerAbilities == null)
        {
            playerAbilities = FindFirstObjectByType<PlayerAbilities>();
        }
        
        if (playerAbilities != null)
        {
            // Subscribe to ability unlock events
            playerAbilities.OnAbilityUnlocked.AddListener(OnAbilityUnlocked);
            
            // Initial setup
            RefreshAbilityDisplay();
        }
    }
    
    void OnDestroy()
    {
        if (playerAbilities != null)
        {
            playerAbilities.OnAbilityUnlocked.RemoveListener(OnAbilityUnlocked);
        }
    }
    
    public void RefreshAbilityDisplay()
    {
        if (playerAbilities == null || abilityContainer == null) return;
        
        // Clear existing items
        ClearAbilityItems();
        
        // Get abilities to display based on filter settings
        List<BaseAbility> abilitiesToShow = GetAbilitiesToShow();
        
        // Create UI items for each ability
        foreach (var ability in abilitiesToShow)
        {
            CreateAbilityItem(ability);
        }
    }
    
    private List<BaseAbility> GetAbilitiesToShow()
    {
        if (showOnlyUnlocked)
        {
            return playerAbilities.GetUnlockedAbilities();
        }
        else if (showOnlyUnlockable)
        {
            return playerAbilities.GetUnlockableAbilities();
        }
        else
        {
            // Show all abilities
            var allAbilities = new List<BaseAbility>();
            allAbilities.AddRange(playerAbilities.GetUnlockedAbilities());
            allAbilities.AddRange(playerAbilities.GetLockedAbilities());
            return allAbilities;
        }
    }
    
    private void CreateAbilityItem(BaseAbility ability)
    {
        if (abilityItemPrefab == null) return;
        
        GameObject itemObject = Instantiate(abilityItemPrefab, abilityContainer);
        AbilityUIItem abilityItem = itemObject.GetComponent<AbilityUIItem>();
        
        if (abilityItem != null)
        {
            abilityItem.Initialize(ability, playerAbilities);
            _abilityItems.Add(abilityItem);
        }
    }
    
    private void ClearAbilityItems()
    {
        foreach (var item in _abilityItems)
        {
            if (item != null && item.gameObject != null)
            {
                Destroy(item.gameObject);
            }
        }
        _abilityItems.Clear();
    }
    
    private void OnAbilityUnlocked(BaseAbility ability)
    {
        // Refresh the display when an ability is unlocked
        RefreshAbilityDisplay();
    }
    
    // Public methods for UI buttons
    public void ToggleShowOnlyUnlocked()
    {
        showOnlyUnlocked = !showOnlyUnlocked;
        if (showOnlyUnlocked) showOnlyUnlockable = false;
        RefreshAbilityDisplay();
    }
    
    public void ToggleShowOnlyUnlockable()
    {
        showOnlyUnlockable = !showOnlyUnlockable;
        if (showOnlyUnlockable) showOnlyUnlocked = false;
        RefreshAbilityDisplay();
    }
    
    public void ShowAllAbilities()
    {
        showOnlyUnlocked = false;
        showOnlyUnlockable = false;
        RefreshAbilityDisplay();
    }
}