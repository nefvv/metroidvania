using UnityEngine;

[CreateAssetMenu(fileName = "JumpAbility", menuName = "Abilities/Movement/Jump")]
public class JumpAbility : BaseAbility
{
    [Header("Jump Settings")]
    public float jumpForce = 10f;
    public int maxJumps = 1;
    
    public override void OnUnlock(GameObject player)
    {
        base.OnUnlock(player);
    }
    
    public override void OnUse(GameObject player)
    {
        base.OnUse(player);
        
        var movement = player.GetComponent<PlayerMovement>();
        if (movement != null)
        {
            PerformJump(player, movement);
        }
    }
    
    public bool CanJump(PlayerMovement movement, PlayerAbilities abilities)
    {
        // Can jump if grounded
        if (movement.IsGrounded) return true;
        
        // Can jump if we haven't used any jumps yet
        if (movement.CurrentJumps == 0) return true;
        
        // Can double jump if we have the ability and used exactly 1 jump
        if (movement.CurrentJumps == 1 && abilities.Has(AbilityId.DoubleJump))
        {
            var doubleJumpAbility = abilities.GetAbility(AbilityId.DoubleJump) as DoubleJumpAbility;
            return doubleJumpAbility != null && doubleJumpAbility.CanDoubleJump(movement, abilities);
        }
        
        return false;
    }
    
    public void PerformJump(GameObject player, PlayerMovement movement)
    {
        var abilities = player.GetComponent<PlayerAbilities>();
        if (!CanJump(movement, abilities)) return;
        
        var rb = movement.rb;
        if (rb == null) return;
        
        // If this is a second jump, delegate to double jump ability
        if (movement.CurrentJumps == 1 && abilities.Has(AbilityId.DoubleJump))
        {
            var doubleJumpAbility = abilities.GetAbility(AbilityId.DoubleJump) as DoubleJumpAbility;
            if (doubleJumpAbility != null)
            {
                doubleJumpAbility.PerformDoubleJump(player, movement);
                return;
            }
        }
        
        Debug.Log($"Jump! (Jump #{movement.CurrentJumps + 1})");
        
        // Apply jump force
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        
        // Increment jump counter
        movement.IncrementJumps();
    }
}