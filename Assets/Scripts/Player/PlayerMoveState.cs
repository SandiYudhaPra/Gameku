using UnityEngine;

public class PlayerMoveState : PlayerState
{
    public PlayerMoveState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
        if (AttackPressed && combat.CanAttack)
            player.ChangeState(player.attackState);

        else if (JumpPressed)
        {
            player.ChangeState(player.jumpState);
        }
        else if (Mathf.Abs(MoveInput.x) < .1f)
        {
            player.ChangeState(player.idleState);
        }
        else if (player.isGrounded && RunPressed && MoveInput.y < -.1f)
        {
            player.ChangeState(player.slideState);
        }
        else 
        { 
            anim.SetBool("isWalking", !RunPressed);
            anim.SetBool("isRunning", RunPressed);
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        float speed = RunPressed ? player.runSpeed : player.walkSpeed;
        rb.linearVelocity = new Vector2(speed * player.facingDir, rb.linearVelocity.y);
    }

    public override void Exit()
    {
        base.Exit();
        anim.SetBool("isWalking", false);
        anim.SetBool("isRunning", false);
    }
}
