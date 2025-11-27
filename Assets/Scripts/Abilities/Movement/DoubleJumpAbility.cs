using UnityEngine;

[CreateAssetMenu(fileName = "DoubleJumpAbility", menuName = "Abilities/Movement/Double Jump")]
public class DoubleJumpAbility : BaseAbility
{
    [Header("Double Jump Settings")]
    public float doubleJumpForce = 8f;
    
    public override void OnUnlock(GameObject player)
    {
        base.OnUnlock(player);
        
        // Enable double jump in player movement
        var movement = player.GetComponent<PlayerMovement>();
        if (movement != null)
        {
            Debug.Log("Double jump unlocked!");
        }
    }
    
    public override void OnUse(GameObject player)
    {
        base.OnUse(player);
        
        var movement = player.GetComponent<PlayerMovement>();
        if (movement != null)
        {
            PerformDoubleJump(player, movement);
        }
    }
    
    public bool CanDoubleJump(PlayerMovement movement, PlayerAbilities abilities)
    {
        // Can only double jump if we're not grounded and have used exactly 1 jump
        return !movement.IsGrounded && movement.CurrentJumps == 1 && abilities.Has(AbilityId.Jump);
    }
    
    public void PerformDoubleJump(GameObject player, PlayerMovement movement)
    {
        if (!CanDoubleJump(movement, player.GetComponent<PlayerAbilities>())) return;
        
        var rb = movement.rb;
        if (rb == null) return;
        
        Debug.Log("Double Jump!");
        
        // Apply double jump force
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, doubleJumpForce);
        
        // Increment jump counter
        movement.IncrementJumps();
    }
}