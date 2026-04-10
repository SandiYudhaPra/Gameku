using UnityEngine;

public class Enemy : MonoBehaviour
{
    //variable
    public int FacingDir { get; private set; } = 1;

    //component
    public Rigidbody2D RB {get; private set;}
    public Animator Anim {get; private set;}
    public EnemyConfig Config;
    public Enemy_Senses Senses {get; private set;}
    public StateMachine StateMachine {get; private set;}

    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
        Anim = GetComponent<Animator>();
        StateMachine = new StateMachine();
        Senses = GetComponent<Enemy_Senses>();
    }

    public void Start()
    {
        StateMachine.Initialize(new PatrolState(this));    
    }

    private void Update() => StateMachine.CurrentState?.Update();

    private void FixedUpdate() => StateMachine.CurrentState?.FixedUpdate();
    

    public void FaceTarget(Transform target)
    {
        float offset = target.position.x - transform.position.x;
        
        int direction = offset > 0 ? 1 : -1;
        if (direction != FacingDir)
            Flip();
    }

    public void Flip()
    {
        FacingDir *= -1;

        Vector3 scale = transform.localScale;
        scale.x = FacingDir;
        transform.localScale = scale;
    }
}
