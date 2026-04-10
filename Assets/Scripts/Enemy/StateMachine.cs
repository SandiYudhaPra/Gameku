using UnityEngine;

public class StateMachine 
{
   public State CurrentState { get; private set; }

   public void Initialize(State StartingState)
    {
        CurrentState = StartingState;
        CurrentState.Enter();
    } 
    
   public void ChangeState(State newState)
    {
        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }
}
