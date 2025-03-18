using UnityEngine;
using UnityEngine.UI;

namespace UIEditor
{
    public class PlayerUIController : MonoBehaviour
    {
        [SerializeField] private Button attackButton;
        [SerializeField] private Player player;
        
        private void Start()
        {
            attackButton.onClick.AddListener(player.InvokeAttack);
        }
        
        private void OnDestroy()
        {
            attackButton.onClick.RemoveListener(player.InvokeAttack);
        }

      
    }
}