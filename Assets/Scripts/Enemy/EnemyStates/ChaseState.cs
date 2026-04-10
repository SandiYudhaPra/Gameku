using UnityEngine;

public class ChaseState : State
{
    private Transform target;
    protected override string AnimBoolName => "isRunning";
    public ChaseState (Enemy enemy) : base(enemy) { }

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

        // cek udah sampe belum
        float distance = Mathf.Abs(target.position.x - enemy.transform.position.x);
        if (distance <= config.turnThreshold) 
        {
            stateMachine.ChangeState(new IdleState(enemy));
            return;
        }

        //cek obstacle
        if (senses.IsHittingWall() || senses.IsAtCliff()) 
        {
            stateMachine.ChangeState(new IdleState(enemy));
            return;
        }
        
        //jalan ke target
        rb.linearVelocity = new Vector2(config.chaseSpeed * enemy.FacingDir, rb.linearVelocity.y);
    }

    public override void Exit()
    {
        base.Exit();
        rb.linearVelocity = Vector2.zero;    
    }
}
