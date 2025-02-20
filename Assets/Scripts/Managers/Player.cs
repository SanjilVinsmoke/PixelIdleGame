using UnityEngine;

namespace Test
{
    public class Player : MonoBehaviour

    {
        private StateMachine<Player> stateMachine;

        private void Start()
        {
            stateMachine = new StateMachine<Player>(this);
            stateMachine.AddState(new PlayerIdleState());
            stateMachine.AddState(new PlayerAttackState());
            stateMachine.SetInitialState<PlayerIdleState>();
            
        }


        //  TODO : Add Editor-only method to toggle debug mode
        public  void  ToggleDebugMode()
        {
            StateMachine<Player>.ToggleDebugMode();
        }
        void Update()
        {
            // Update the state machine every frame.
            stateMachine.Update();

            // Optional: Log the current state if debug mode is enabled.
            if (StateMachine<Player>.DebugMode)
            {
                Debug.Log("Current State: " + stateMachine.CurrentStateName);
            }

        }

        void FixedUpdate()
        {
            stateMachine.FixedUpdate();
        }

        public void ChangePlayerState<TState>() where TState : BaseState<Player>
        {
            stateMachine.ChangeState<TState>();
        }


    }
}