using UnityEngine;


public class IdleState : State
{
    private Transform target;
    protected override string AnimBoolName => "isIdling";
    public IdleState(Enemy enemy) : base(enemy) { }

    public override void Enter()
    {
        base.Enter();
        rb.linearVelocity = Vector2.zero;
    }

    public override void FixedUpdate()
    {
        // cek target
        target = senses.GetChaseTarget();
        if (!target)
        {
            stateMachine.ChangeState(new PatrolState(enemy));
            return;
        }
        enemy.FaceTarget(target);
        
        //cek jarak
        float distance = Mathf.Abs(target.position.x - enemy.transform.position.x);
        if (distance <= config.turnThreshold)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        //cek obstacle 
        if (senses.IsHittingWall() || senses.IsAtCliff())
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        //ketemu target, gaada obstacle, tapi tapi target ngga cukup dekat buat diserang 
        stateMachine.ChangeState(new ChaseState(enemy));
    }
}
