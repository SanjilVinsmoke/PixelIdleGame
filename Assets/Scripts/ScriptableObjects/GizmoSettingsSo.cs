using UnityEngine;

namespace ScriptableObjects
{
    /// <summary>
    /// ScriptableObject that stores gizmo visibility settings for all components.
    /// Use the Gizmo Settings window (Tools > Gizmo Settings) to toggle visibility.
    /// </summary>
    [CreateAssetMenu(fileName = "GizmoSettings", menuName = "Game/Gizmo Settings")]
    public class GizmoSettingsSo : ScriptableObject
    {
        private static GizmoSettingsSo _instance;
        
        /// <summary>
        /// Singleton instance that auto-loads from Resources folder.
        /// </summary>
        public static GizmoSettingsSo Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<GizmoSettingsSo>("GizmoSettings");
                    
                    #if UNITY_EDITOR
                    // Create instance in editor if it doesn't exist
                    if (_instance == null)
                    {
                        _instance = CreateInstance<GizmoSettingsSo>();
                        
                        // Ensure Resources folder exists
                        if (!UnityEditor.AssetDatabase.IsValidFolder("Assets/Resources"))
                        {
                            UnityEditor.AssetDatabase.CreateFolder("Assets", "Resources");
                        }
                        
                        UnityEditor.AssetDatabase.CreateAsset(_instance, "Assets/Resources/GizmoSettings.asset");
                        UnityEditor.AssetDatabase.SaveAssets();
                        Debug.Log("[GizmoSettings] Created new GizmoSettings asset in Resources folder.");
                    }
                    #endif
                }
                return _instance;
            }
        }

        [Header("Master Toggle")]
        [Tooltip("Master switch to enable/disable ALL gizmos")]
        public bool enableAllGizmos = true;

        [Header("Combat Gizmos")]
        [Tooltip("Show attack range circles")]
        public bool showAttackRange = true;
        
        [Tooltip("Show smash hitbox and radius")]
        public bool showSmashGizmos = true;

        [Header("Movement Gizmos")]
        [Tooltip("Show ground check boxes")]
        public bool showGroundCheck = true;
        
        [Tooltip("Show wall detection rays")]
        public bool showWallCheck = true;

        [Header("Detection Gizmos")]
        [Tooltip("Show enemy detection range")]
        public bool showDetectionRange = true;

        [Header("Environment Gizmos")]
        [Tooltip("Show room trigger areas")]
        public bool showRoomTriggers = true;
        
        [Tooltip("Show door bounds and spawn points")]
        public bool showDoorGizmos = true;

        [Header("Gizmo Colors")]
        public Color attackRangeColor = Color.red;
        public Color detectionRangeColor = Color.yellow;
        public Color groundCheckColor = Color.red;
        public Color wallCheckActiveColor = Color.red;
        public Color wallCheckInactiveColor = Color.yellow;
        public Color roomTriggerColor = new Color(0, 1, 0, 0.2f);
        public Color doorBoundsColor = Color.blue;
        public Color doorSpawnColor = Color.green;
        public Color smashDashColor = Color.red;
        public Color smashDownColor = Color.blue;

        /// <summary>
        /// Checks if a specific gizmo category should be drawn.
        /// </summary>
        public bool ShouldDraw(GizmoCategory category)
        {
            if (!enableAllGizmos) return false;
            
            return category switch
            {
                GizmoCategory.AttackRange => showAttackRange,
                GizmoCategory.SmashGizmos => showSmashGizmos,
                GizmoCategory.GroundCheck => showGroundCheck,
                GizmoCategory.WallCheck => showWallCheck,
                GizmoCategory.DetectionRange => showDetectionRange,
                GizmoCategory.RoomTrigger => showRoomTriggers,
                GizmoCategory.DoorGizmos => showDoorGizmos,
                _ => true
            };
        }
    }

    /// <summary>
    /// Categories of gizmos that can be toggled.
    /// </summary>
    public enum GizmoCategory
    {
        AttackRange,
        SmashGizmos,
        GroundCheck,
        WallCheck,
        DetectionRange,
        RoomTrigger,
        DoorGizmos
    }
}

