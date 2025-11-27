using UnityEngine;

[CreateAssetMenu(fileName = "DashAbility", menuName = "Abilities/Movement/Dash")]
public class DashAbility : BaseAbility
{
    [Header("Dash Settings")]
    public float dashForce = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    
    public override void OnUnlock(GameObject player)
    {
        base.OnUnlock(player);
        
        // Add dash component or enable dash in movement
        var movement = player.GetComponent<PlayerMovement>();
        if (movement != null)
        {
            Debug.Log("Dash ability unlocked!");
        }
    }
    
    public override void OnUse(GameObject player)
    {
        base.OnUse(player);
        
        // Dash logic would be implemented in PlayerMovement or a separate DashController
        Debug.Log($"Using {abilityName} - Dash!");
    }
}