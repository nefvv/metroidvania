using UnityEngine;

/// <summary>
/// Example script showing how to use the ability system.
/// Attach this to a GameObject in your scene to test ability unlocking.
/// </summary>
public class AbilitySystemExample : MonoBehaviour
{
    [Header("References")]
    public PlayerAbilities playerAbilities;
    public AbilityManager abilityManager;
    
    [Header("Testing")]
    public KeyCode unlockJumpKey = KeyCode.Alpha1;
    public KeyCode unlockDoubleJumpKey = KeyCode.Alpha2;
    public KeyCode unlockDashKey = KeyCode.Alpha3;
    public KeyCode triggerBossDefeatKey = KeyCode.B;
    
    void Start()
    {
        // Find components if not assigned
        if (playerAbilities == null)
            playerAbilities = FindFirstObjectByType<PlayerAbilities>();
        
        if (abilityManager == null)
            abilityManager = FindFirstObjectByType<AbilityManager>();
        
        // Display instructions
        Debug.Log("=== ABILITY SYSTEM TEST ===");
        Debug.Log("Press 1 to unlock Jump");
        Debug.Log("Press 2 to unlock Double Jump");
        Debug.Log("Press 3 to unlock Dash");
        Debug.Log("Press B to trigger 'boss defeated' condition");
        Debug.Log("Check the console for ability unlock messages!");
    }
    
    void Update()
    {
        // Test manual ability unlocking
        if (Input.GetKeyDown(unlockJumpKey))
        {
            TestUnlockAbility(AbilityId.Jump);
        }
        
        if (Input.GetKeyDown(unlockDoubleJumpKey))
        {
            TestUnlockAbility(AbilityId.DoubleJump);
        }
        
        if (Input.GetKeyDown(unlockDashKey))
        {
            TestUnlockAbility(AbilityId.Dash);
        }
        
        if (Input.GetKeyDown(triggerBossDefeatKey))
        {
            TestTriggerCondition("boss_defeated_example");
        }
    }
    
    private void TestUnlockAbility(AbilityId abilityId)
    {
        if (playerAbilities != null)
        {
            if (playerAbilities.Has(abilityId))
            {
                Debug.Log($"{abilityId} is already unlocked!");
            }
            else if (playerAbilities.CanUnlockAbility(abilityId))
            {
                playerAbilities.UnlockAbility(abilityId);
                Debug.Log($"✅ Unlocked {abilityId}!");
            }
            else
            {
                Debug.Log($"❌ Cannot unlock {abilityId} - prerequisites not met");
            }
        }
    }
    
    private void TestTriggerCondition(string condition)
    {
        if (abilityManager != null)
        {
            Debug.Log($"Triggering condition: {condition}");
            abilityManager.TriggerCondition(condition);
        }
        else
        {
            Debug.LogWarning("AbilityManager not found!");
        }
    }
    
    // Method to be called by UI buttons or other systems
    public void UnlockRandomAbility()
    {
        var lockedAbilities = playerAbilities.GetLockedAbilities();
        if (lockedAbilities.Count > 0)
        {
            var randomAbility = lockedAbilities[Random.Range(0, lockedAbilities.Count)];
            playerAbilities.UnlockAbility(randomAbility.abilityId);
        }
        else
        {
            Debug.Log("All abilities are already unlocked!");
        }
    }
}