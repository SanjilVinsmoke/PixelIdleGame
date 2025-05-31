using UnityEngine;
using UnityEditor;
using ScriptableObjects;

namespace Editor
{
    [CustomPropertyDrawer(typeof(EnemyStateData))]
    public class EnemyStateDataPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            
            var enabledRect = new Rect(position.x, position.y, 20, position.height);
            var eventRect = new Rect(position.x + 25, position.y, 80, position.height);
            var classRect = new Rect(position.x + 110, position.y, position.width - 110, position.height);
            
            EditorGUI.PropertyField(enabledRect, property.FindPropertyRelative("isEnabled"), GUIContent.none);
            EditorGUI.PropertyField(eventRect, property.FindPropertyRelative("stateEvent"), GUIContent.none);
            EditorGUI.PropertyField(classRect, property.FindPropertyRelative("stateClassName"), GUIContent.none);
            
            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
    }
    
    [CustomEditor(typeof(EnemyDataSo))]
    public class EnemyDataSoEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("Open in Enemy Creation Tool"))
            {
                var window = EditorWindow.GetWindow<EnemyCreationTool>();
                window.Show();
            }
        }
    }
}
