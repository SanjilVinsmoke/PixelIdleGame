using UnityEngine.Events;

namespace UI.Interfaces
{
    public interface UIPanel
    {
        public string PanelName { get; }
        public UnityEvent OnShow { get; }
        public UnityEvent OnHide { get; }
        public void Show();
        public void Hide();
        
    }
}