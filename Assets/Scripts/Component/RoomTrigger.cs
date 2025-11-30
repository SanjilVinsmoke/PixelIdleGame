using UnityEngine;
using Managers;
using ScriptableObjects;

namespace Component
{
    [RequireComponent(typeof(Collider2D))]
    public class RoomTrigger : MonoBehaviour
    {
        [Tooltip("Unique ID for this room (e.g., 'Area1_01')")]
        [SerializeField] private string roomId;

        [Tooltip("Grid position for the map (X, Y)")]
        [SerializeField] private Vector2Int mapCoordinates;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                if (MapManager.Instance != null)
                {
                    MapManager.Instance.RegisterRoomVisit(roomId);
                }
            }
        }
        
        private void OnDrawGizmos()
        {
            #if UNITY_EDITOR
            var settings = GizmoSettingsSo.Instance;
            if (settings == null || !settings.ShouldDraw(GizmoCategory.RoomTrigger)) return;
            
            Gizmos.color = settings.roomTriggerColor;
            var collider = GetComponent<Collider2D>();
            if (collider != null)
            {
                Gizmos.DrawCube(transform.position, collider.bounds.size);
            }
            #endif
        }
    }
}

