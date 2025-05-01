using UnityEngine;

namespace Component
{
    
    public class AnimationComponent:MonoBehaviour
    {
        [SerializeField]
        Animator animator;
        
       
        
        public void SetBool(string parameterName, bool value)
        {
            animator.SetBool(parameterName, value);
        }
        public void SetTrigger(string parameterName)
        {
            animator.SetTrigger(parameterName);
        }
        public void SetFloat(string parameterName, float value)
        {
            animator.SetFloat(parameterName, value);
        }
        public void SetInt(string parameterName, int value)
        {
            animator.SetInteger(parameterName, value);
        }
        public void PlayAnimation(string animationName)
        {
            // Check if the animation exists in the animator
            
            animator.Play(animationName);
            
        }
        public void StopAnimation(string animationName)
        {
            animator.StopPlayback();
        }
    }
}