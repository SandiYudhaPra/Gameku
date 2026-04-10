using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class PlayerController : MonoBehaviour
{
    public PlayerState currentState;

    public PlayerIdleState idleState;
    public PlayerJumpState jumpState;
    public PlayerMoveState moveState;
    public PlayerCrouchState crouchState;
    public PlayerSlideState slideState;
    public PlayerAttackState attackState;

    [Header("Core Component")]
    public Combat combat;

    [Header("Components")]
    public Rigidbody2D rb;
    public PlayerInput input;
    public Animator anim;
    public CapsuleCollider2D playerCollider;
    
    [Header("Movement Variables")]
    public float walkSpeed;
    public float runSpeed;
    public float jumpForce;
    public float jumpMultiplier = 0.5f;
    public float normalGravity;
    public float jumpGravity;
    public float fallGravity;

    public int facingDir = 1;

    //input
    public Vector2 moveInput;
    public bool runPressed;
    public bool jumpPressed;
    public bool jumpReleased;
    public bool attackPressed;
    
    
    [Header("Ground Check")]
    public Transform GroundCheck;
    public float groundCheckRadius;
    public LayerMask groundLayer;
    public bool isGrounded;

    [Header("Crouch Settings")]
    public Transform headCheck;
    public float headCheckRadius;

    [Header("Slide Settings")]
    public float slideDur;
    public float slideStopDur = .15f;
    public float slideSpeed;

    public float slideHeight;
    public Vector2 slideOffset;
    public float normalHeight;
    public Vector2 normalOffset;

    private bool isSliding;
   
    private void Awake()
    {
        idleState = new PlayerIdleState(this);
        jumpState = new PlayerJumpState(this);
        moveState = new PlayerMoveState(this);
        crouchState = new PlayerCrouchState(this);
        slideState = new PlayerSlideState(this);
        attackState = new PlayerAttackState(this); 
    }

    private void Start()
    {
        rb.gravityScale = normalGravity;

       ChangeState(idleState);
    }

    void Update()
    {
        currentState.Update();

        if(!isSliding)
            Flip();
        
        HandleAnimations();
    }

    void FixedUpdate()
    {
        currentState.FixedUpdate();

        CheckGround();
    }

    public void ChangeState(PlayerState newState)
    {
        if(currentState != null) 
            currentState.Exit();     
        
        currentState=newState;
        currentState.Enter();
    }  
    
    private void HandleMovement()
    {
        float currentSpeed = runPressed ? runSpeed : walkSpeed;
        float targetSpeed = moveInput.x * currentSpeed;
        rb.linearVelocity = new Vector2(targetSpeed, rb.linearVelocity.y);
    }

    public void SetColliderNormal()
    {
        playerCollider.size = new Vector2(playerCollider.size.x, normalHeight);
        playerCollider.offset = normalOffset;
    }

    public void SetColliderSlide()
    {
        playerCollider.size = new Vector2(playerCollider.size.x, slideHeight);
        playerCollider.offset = slideOffset;
    }

    public void ApplyVariableGravity()
    {
        if(rb.linearVelocity.y < -0.1f)
        {
            rb.gravityScale = fallGravity;
        }
        else if (rb.linearVelocity.y > 0.1f)
        {
            rb.gravityScale = jumpGravity;
        }
        else
        {
            rb.gravityScale = normalGravity;
        }
    }

    void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(GroundCheck.position, groundCheckRadius, groundLayer);
    }

    public bool CheckForCeiling()
    {
        return Physics2D.OverlapCircle(headCheck.position, headCheckRadius, groundLayer);
    }

    void HandleAnimations()
    {
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVelocity", rb.linearVelocity.y); 
    }
     
    void Flip()
    {
        if(moveInput.x > 0.1f)
        {
            facingDir = 1;
        }
        else if(moveInput.x < -0.1f)
        {
            facingDir = -1;
        }
        transform.localScale = new Vector3(facingDir, 1, 1);
    }

    public void AttackAnimationFinished()
    {
        currentState.AttackAnimationFinished();
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnRun(InputValue value)
    {
        runPressed = value.isPressed;
    }

    public void OnAttack(InputValue value)
    {
        attackPressed = value.isPressed;
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            if(isGrounded && !CheckForCeiling())
                jumpPressed = true;
            
            jumpReleased = false;
        }
        else
        {
           jumpReleased = true;
        }          
    }

    private void OnDrawGizmosSelected()
    {   
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(GroundCheck.position, groundCheckRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere (headCheck.position, headCheckRadius);
    }

}
