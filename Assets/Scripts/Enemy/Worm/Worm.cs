using System.Collections.Generic;
using Managers;
using StateMachine;
using UnityEngine;

public class Worm : Enemy<Worm>
{
    
    
    
    protected override void Start() {
        base.Start();
        // Initialize the state machine with the initial state
        stateMachine.AddEventMapping(EnemyEvent.Move, () => stateMachine.ChangeState<EnemyIdleState<Worm>>());
        stateMachine.AddState(new EnemyRoamState<Worm>());
        stateMachine.AddState(new EnemyIdleState<Worm>());
       stateMachine.SetInitialState<EnemyRoamState<Worm>>();
        
        
    }
    
    
}
