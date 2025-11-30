using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using ScriptableObjects;

namespace Component
{
    /// <summary>
    /// Handles transitions between scenes or positions.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class DoorComponent : MonoBehaviour
    {
        [Header("Connection Settings")]
        [Tooltip("ID of this door. Must be unique within the scene.")]
        [SerializeField] private string doorID;

        [Tooltip("The name of the target scene to load. Leave empty if teleporting within the same scene.")]
        [SerializeField] private string targetSceneName;

        [Tooltip("The ID of the target door in the destination scene.")]
        [SerializeField] private string targetDoorID;
        
        [Header("Transition Settings")]
        [Tooltip("Position offset where player spawns relative to the door.")]
        [SerializeField] private Vector2 spawnOffset = Vector2.zero;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                Debug.Log($"Player entered door {doorID}. Teleporting to {targetSceneName} -> Door {targetDoorID}");
                StartCoroutine(TransitionRoutine(collision.gameObject));
            }
        }

        private IEnumerator TransitionRoutine(GameObject player)
        {
            // Optional: Disable player control
            // player.GetComponent<InputComponent>().DisableInput();
            
            // Optional: Fade Out UI
            
            if (!string.IsNullOrEmpty(targetSceneName))
            {
                // Set the target door ID in a static manager so the next scene knows where to spawn
                // For MVP simplicity, we can use a static field or GameManager
                // TransitionManager.TargetDoorID = targetDoorID; 
                
                // Saving this to PlayerPrefs or a static variable is a quick way for MVP
                PlayerPrefs.SetString("TargetDoorID", targetDoorID);
                
                // Load Scene
                yield return SceneManager.LoadSceneAsync(targetSceneName);
                
                // Logic to find door and place player happens in the new scene's Start/Awake
                // Usually handled by a SceneInitializer or the Door itself checking "Am I the target?"
            }
            else
            {
                // Teleport within same scene
                // Find target door
                DoorComponent targetDoor = FindDoorByID(targetDoorID);
                if (targetDoor != null)
                {
                    player.transform.position = (Vector2)targetDoor.transform.position + targetDoor.spawnOffset;
                }
                else
                {
                    Debug.LogWarning($"Target Door {targetDoorID} not found in current scene.");
                }
            }
            
            // Optional: Fade In UI
            // player.GetComponent<InputComponent>().EnableInput();
        }
        
        // Static helper to find doors (inefficient but fine for MVP/small scenes)
        public static DoorComponent FindDoorByID(string id)
        {
            var doors = FindObjectsOfType<DoorComponent>();
            foreach (var door in doors)
            {
                if (door.doorID == id) return door;
            }
            return null;
        }

        // On Scene Load logic to catch the player
        private void Start()
        {
            string targetID = PlayerPrefs.GetString("TargetDoorID", "");
            if (!string.IsNullOrEmpty(targetID) && targetID == doorID)
            {
                // This is the target door! Spawn player here.
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    player.transform.position = (Vector2)transform.position + spawnOffset;
                    Debug.Log($"Player spawned at Door {doorID}");
                    
                    // Clear the target so we don't re-spawn here on reload unless requested
                    PlayerPrefs.SetString("TargetDoorID", ""); 
                }
            }
        }
        
        private void OnDrawGizmos()
        {
            #if UNITY_EDITOR
            var settings = GizmoSettingsSo.Instance;
            if (settings == null || !settings.ShouldDraw(GizmoCategory.DoorGizmos)) return;
            
            var collider = GetComponent<Collider2D>();
            if (collider != null)
            {
                Gizmos.color = settings.doorBoundsColor;
                Gizmos.DrawWireCube(transform.position, collider.bounds.size);
            }
            
            Gizmos.color = settings.doorSpawnColor;
            Gizmos.DrawSphere((Vector2)transform.position + spawnOffset, 0.2f);
            #endif
        }
    }
}

