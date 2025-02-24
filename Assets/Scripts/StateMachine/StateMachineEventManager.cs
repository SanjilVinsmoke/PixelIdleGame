using System;

namespace StateMachine
{
   

    public static class StateMachineEventManager
    {
        public static event Action<string> OnEvent;

        public static void RaiseEvent(string eventName)
        {
            OnEvent?.Invoke(eventName);
        }
    }

    }
