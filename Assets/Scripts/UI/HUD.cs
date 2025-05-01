using UI.Interfaces;
using UnityEngine;
using UnityEngine.Events;

namespace UIEditor
{
    public class HUD: MonoBehaviour, UIPanel
    {
        public string PanelName => "HUD";
        public UnityEvent OnShow { get; }
        public UnityEvent OnHide { get; }
        
        
        private void Awake()
        {
            OnShow.AddListener(Show);
            OnHide.AddListener(Hide);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        
        
       

        public void OnDestroy()
        {
            OnShow.RemoveAllListeners();
            OnHide.RemoveAllListeners();
        }
    }
  
}