using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Components")]
    public Rigidbody2D rb;
    [SerializeField] private PlayerAbilities abilities;
    
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float fallSpeedMultiplier = 1f;
    public float maxFallSpeed = 20f;
    
    [Header("Jump Settings")]
    public int maxJumps = 1;
    private int _currentJumps = 0;
    private bool _isGrounded = false;
    
    // Public properties for abilities to access
    public bool IsGrounded => _isGrounded;
    public int CurrentJumps => _currentJumps;
    
    public void IncrementJumps() => _currentJumps++;
    public void ResetJumps() => _currentJumps = 0;
    
    [Header("Ground Detection")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayerMask = 1;
    
    private float horizontalMovement;
    private float verticalMovement;

    void Start()
    {
        // Ensure components are assigned
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (abilities == null) abilities = GetComponent<PlayerAbilities>();
        
        // Create ground check if it doesn't exist
        if (groundCheck == null)
        {
            GameObject groundCheckObj = new GameObject("GroundCheck");
            groundCheckObj.transform.SetParent(transform);
            groundCheckObj.transform.localPosition = new Vector3(0, -0.5f, 0);
            groundCheck = groundCheckObj.transform;
        }
    }

    void Update()
    {
        CheckGrounded();
        HandleMovement();
    }
    
    private void CheckGrounded()
    {
        bool wasGrounded = _isGrounded;
        _isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayerMask);
        
        // Reset jumps when landing
        if (!wasGrounded && _isGrounded)
        {
            ResetJumps();
        }
    }
    
    private void HandleMovement()
    {
        if (rb != null)
        {
            Vector2 velocity = rb.linearVelocity;
            
            // Horizontal movement
            velocity.x = horizontalMovement * moveSpeed;
            
            // Apply faster falling when moving downward
            if (velocity.y < 0 && !_isGrounded) // Falling and not grounded
            {
                velocity.y *= fallSpeedMultiplier;
                velocity.y = Mathf.Max(velocity.y, -maxFallSpeed); // Clamp to max fall speed
            }
            
            rb.linearVelocity = velocity;
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        Debug.Log($"Jump method called! Phase: {context.phase}, Started: {context.started}");
        
        if (!context.started) return;

        // Check if player has jump ability
        if (abilities != null && !abilities.Has(AbilityId.Jump))
        {
            Debug.Log("Jump ability not unlocked");
            return;
        }
        
        Debug.Log("Jump ability found, attempting jump...");
        
        // Get the jump ability and let it handle the jump logic
        var jumpAbility = abilities.GetAbility(AbilityId.Jump) as JumpAbility;
        if (jumpAbility != null)
        {
            jumpAbility.PerformJump(gameObject, this);
            
            // Track which ability was used
            if (_currentJumps == 1)
            {
                abilities.UseAbility(AbilityId.Jump);
            }
            else if (_currentJumps == 2 && abilities.Has(AbilityId.DoubleJump))
            {
                abilities.UseAbility(AbilityId.DoubleJump);
            }
        }
        else
        {
            Debug.LogWarning("Jump ability is null!");
        }
    }
    
    public void Dash(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        
        // Check if player has dash ability
        if (abilities != null && !abilities.Has(AbilityId.Dash))
        {
            return;
        }
        
        if (rb == null) return;
        
        // Get dash ability settings
        var dashAbility = abilities.GetAbility(AbilityId.Dash) as DashAbility;
        if (dashAbility != null)
        {
            // Apply dash force in movement direction
            Vector2 dashDirection = horizontalMovement != 0 ? new Vector2(Mathf.Sign(horizontalMovement), 0) : Vector2.right;
            rb.linearVelocity = dashDirection * dashAbility.dashForce;
            
            // Use the ability
            abilities.UseAbility(AbilityId.Dash);
        }
    }
    
    void OnDrawGizmosSelected()
    {
        // Draw ground check radius
        if (groundCheck != null)
        {
            Gizmos.color = _isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
