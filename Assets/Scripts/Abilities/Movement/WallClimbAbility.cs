using UnityEngine;

[CreateAssetMenu(fileName = "WallClimbAbility", menuName = "Abilities/Movement/Wall Climb")]
public class WallClimbAbility : BaseAbility
{
    [Header("Wall Climb Settings")]
    public float climbSpeed = 3f;
    public float wallJumpForce = 12f;
    public LayerMask wallLayerMask = 1;
    
    public override void OnUnlock(GameObject player)
    {
        base.OnUnlock(player);
        
        // Enable wall climbing mechanics
        Debug.Log("Wall climbing unlocked!");
    }
    
    public override void OnUse(GameObject player)
    {
        base.OnUse(player);
        Debug.Log($"Using {abilityName}");
    }
}