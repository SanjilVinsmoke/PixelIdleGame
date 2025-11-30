using UnityEngine;
using UnityEditor;
using ScriptableObjects;

namespace Editor
{
    /// <summary>
    /// Editor window for managing gizmo visibility settings.
    /// Access via Tools > Gizmo Settings menu.
    /// </summary>
    public class GizmoSettingsWindow : EditorWindow
    {
        private GizmoSettingsSo settings;
        private Vector2 scrollPosition;
        private bool showColorSettings = false;

        [MenuItem("Tools/Gizmo Settings")]
        public static void ShowWindow()
        {
            var window = GetWindow<GizmoSettingsWindow>("Gizmo Settings");
            window.minSize = new Vector2(300, 400);
            window.Show();
        }

        private void OnEnable()
        {
            LoadSettings();
        }

        private void LoadSettings()
        {
            settings = GizmoSettingsSo.Instance;
        }

        private void OnGUI()
        {
            if (settings == null)
            {
                LoadSettings();
                if (settings == null)
                {
                    EditorGUILayout.HelpBox("Could not load or create GizmoSettings. Please create one manually via Create > Game > Gizmo Settings and place it in a Resources folder.", MessageType.Error);
                    return;
                }
            }

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            EditorGUI.BeginChangeCheck();

            // Header
            DrawHeader();

            EditorGUILayout.Space(10);

            // Master Toggle
            DrawMasterToggle();

            EditorGUILayout.Space(10);

            // Quick Actions
            DrawQuickActions();

            EditorGUILayout.Space(15);

            // Category Toggles
            EditorGUI.BeginDisabledGroup(!settings.enableAllGizmos);
            
            DrawCategorySection("Combat", ref settings.showAttackRange, ref settings.showSmashGizmos,
                "Attack Range", "Smash Gizmos");

            EditorGUILayout.Space(10);

            DrawCategorySection("Movement", ref settings.showGroundCheck, ref settings.showWallCheck,
                "Ground Check", "Wall Check");

            EditorGUILayout.Space(10);

            DrawSingleToggleSection("Detection", ref settings.showDetectionRange, "Enemy Detection Range");

            EditorGUILayout.Space(10);

            DrawCategorySection("Environment", ref settings.showRoomTriggers, ref settings.showDoorGizmos,
                "Room Triggers", "Door Gizmos");

            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space(15);

            // Color Settings
            DrawColorSettings();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(settings);
                SceneView.RepaintAll();
            }

            EditorGUILayout.EndScrollView();

            // Footer
            DrawFooter();
        }

        private void DrawHeader()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            var headerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 16,
                alignment = TextAnchor.MiddleCenter
            };
            
            EditorGUILayout.LabelField("🎯 Gizmo Visibility Settings", headerStyle, GUILayout.Height(30));
            
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.HelpBox("Control which debug gizmos are visible in the Scene view.", MessageType.Info);
        }

        private void DrawMasterToggle()
        {
            EditorGUILayout.BeginVertical("box");
            
            var masterStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 14
            };
            
            EditorGUILayout.BeginHorizontal();
            
            var masterColor = settings.enableAllGizmos ? new Color(0.2f, 0.8f, 0.2f) : new Color(0.8f, 0.2f, 0.2f);
            GUI.color = masterColor;
            
            settings.enableAllGizmos = EditorGUILayout.Toggle(settings.enableAllGizmos, GUILayout.Width(20));
            
            GUI.color = Color.white;
            EditorGUILayout.LabelField("Enable All Gizmos", masterStyle);
            
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        private void DrawQuickActions()
        {
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Show All", GUILayout.Height(25)))
            {
                SetAllGizmos(true);
            }
            
            if (GUILayout.Button("Hide All", GUILayout.Height(25)))
            {
                SetAllGizmos(false);
            }
            
            if (GUILayout.Button("Combat Only", GUILayout.Height(25)))
            {
                SetCombatOnly();
            }
            
            EditorGUILayout.EndHorizontal();
        }

        private void DrawCategorySection(string title, ref bool toggle1, ref bool toggle2, string label1, string label2)
        {
            EditorGUILayout.BeginVertical("box");
            
            var sectionStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 12
            };
            
            EditorGUILayout.LabelField($"⚙️ {title}", sectionStyle);
            
            EditorGUI.indentLevel++;
            toggle1 = EditorGUILayout.Toggle(label1, toggle1);
            toggle2 = EditorGUILayout.Toggle(label2, toggle2);
            EditorGUI.indentLevel--;
            
            EditorGUILayout.EndVertical();
        }

        private void DrawSingleToggleSection(string title, ref bool toggle, string label)
        {
            EditorGUILayout.BeginVertical("box");
            
            var sectionStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 12
            };
            
            EditorGUILayout.LabelField($"⚙️ {title}", sectionStyle);
            
            EditorGUI.indentLevel++;
            toggle = EditorGUILayout.Toggle(label, toggle);
            EditorGUI.indentLevel--;
            
            EditorGUILayout.EndVertical();
        }

        private void DrawColorSettings()
        {
            showColorSettings = EditorGUILayout.Foldout(showColorSettings, "🎨 Gizmo Colors", true);
            
            if (showColorSettings)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUI.indentLevel++;
                
                settings.attackRangeColor = EditorGUILayout.ColorField("Attack Range", settings.attackRangeColor);
                settings.detectionRangeColor = EditorGUILayout.ColorField("Detection Range", settings.detectionRangeColor);
                settings.groundCheckColor = EditorGUILayout.ColorField("Ground Check", settings.groundCheckColor);
                settings.wallCheckActiveColor = EditorGUILayout.ColorField("Wall Check (Active)", settings.wallCheckActiveColor);
                settings.wallCheckInactiveColor = EditorGUILayout.ColorField("Wall Check (Inactive)", settings.wallCheckInactiveColor);
                settings.roomTriggerColor = EditorGUILayout.ColorField("Room Trigger", settings.roomTriggerColor);
                settings.doorBoundsColor = EditorGUILayout.ColorField("Door Bounds", settings.doorBoundsColor);
                settings.doorSpawnColor = EditorGUILayout.ColorField("Door Spawn Point", settings.doorSpawnColor);
                settings.smashDashColor = EditorGUILayout.ColorField("Smash Dash", settings.smashDashColor);
                settings.smashDownColor = EditorGUILayout.ColorField("Smash Down", settings.smashDownColor);
                
                EditorGUILayout.Space(5);
                
                if (GUILayout.Button("Reset Colors"))
                {
                    ResetColors();
                }
                
                EditorGUI.indentLevel--;
                EditorGUILayout.EndVertical();
            }
        }

        private void DrawFooter()
        {
            EditorGUILayout.Space(10);
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            if (GUILayout.Button("Select Settings Asset", GUILayout.Width(150)))
            {
                Selection.activeObject = settings;
                EditorGUIUtility.PingObject(settings);
            }
            
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        private void SetAllGizmos(bool enabled)
        {
            settings.enableAllGizmos = true;
            settings.showAttackRange = enabled;
            settings.showSmashGizmos = enabled;
            settings.showGroundCheck = enabled;
            settings.showWallCheck = enabled;
            settings.showDetectionRange = enabled;
            settings.showRoomTriggers = enabled;
            settings.showDoorGizmos = enabled;
            
            EditorUtility.SetDirty(settings);
            SceneView.RepaintAll();
        }

        private void SetCombatOnly()
        {
            settings.enableAllGizmos = true;
            settings.showAttackRange = true;
            settings.showSmashGizmos = true;
            settings.showGroundCheck = false;
            settings.showWallCheck = false;
            settings.showDetectionRange = true;
            settings.showRoomTriggers = false;
            settings.showDoorGizmos = false;
            
            EditorUtility.SetDirty(settings);
            SceneView.RepaintAll();
        }

        private void ResetColors()
        {
            settings.attackRangeColor = Color.red;
            settings.detectionRangeColor = Color.yellow;
            settings.groundCheckColor = Color.red;
            settings.wallCheckActiveColor = Color.red;
            settings.wallCheckInactiveColor = Color.yellow;
            settings.roomTriggerColor = new Color(0, 1, 0, 0.2f);
            settings.doorBoundsColor = Color.blue;
            settings.doorSpawnColor = Color.green;
            settings.smashDashColor = Color.red;
            settings.smashDownColor = Color.blue;
            
            EditorUtility.SetDirty(settings);
            SceneView.RepaintAll();
        }
    }
}

