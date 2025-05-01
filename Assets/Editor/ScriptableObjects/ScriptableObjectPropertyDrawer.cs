using UnityEditor;
using UnityEngine;

// This attribute tells Unity to use this drawer for any ScriptableObject field

    [CustomPropertyDrawer(typeof(ScriptableObject), true)]
    public class ScriptableObjectPropertyDrawer : PropertyDrawer
    {
        private UnityEditor.Editor editor = null;
        private bool foldout = true;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float totalHeight = EditorGUIUtility.singleLineHeight;
        
            if (property.objectReferenceValue != null && foldout)
            {
                if (editor == null)
                    UnityEditor.Editor.CreateCachedEditor(property.objectReferenceValue, null, ref editor);
            
                if (editor != null)
                {
                    SerializedObject serializedObject = editor.serializedObject;
                    SerializedProperty prop = serializedObject.GetIterator();
                
                    if (prop.NextVisible(true))
                    {
                        do
                        {
                            if (prop.name == "m_Script") continue;
                        
                            SerializedProperty subProp = serializedObject.FindProperty(prop.name);
                            totalHeight += EditorGUI.GetPropertyHeight(subProp, null, true) + EditorGUIUtility.standardVerticalSpacing;
                        }
                        while (prop.NextVisible(false));
                    }
                
                    totalHeight += EditorGUIUtility.standardVerticalSpacing * 2;
                }
            }
        
            return totalHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
        
            // Draw the main property field
            Rect objectFieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.ObjectField(objectFieldRect, property, label);
        
            // If we have a valid reference, draw the foldout and the properties
            if (property.objectReferenceValue != null)
            {
                // Draw the foldout arrow
                Rect foldoutRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
                foldout = EditorGUI.Foldout(foldoutRect, foldout, GUIContent.none, true);
            
                // Draw the ScriptableObject properties if foldout is open
                if (foldout)
                {
                    if (editor == null)
                        UnityEditor.Editor.CreateCachedEditor(property.objectReferenceValue, null, ref editor);
                
                    if (editor != null)
                    {
                        // Indent the properties
                        EditorGUI.indentLevel++;
                    
                        // Draw the properties
                        SerializedObject serializedObject = editor.serializedObject;
                        serializedObject.Update();
                    
                        SerializedProperty prop = serializedObject.GetIterator();
                        float yOffset = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    
                        if (prop.NextVisible(true))
                        {
                            do
                            {
                                // Skip the script field
                                if (prop.name == "m_Script") continue;
                            
                                float height = EditorGUI.GetPropertyHeight(prop, null, true);
                                Rect propRect = new Rect(position.x, position.y + yOffset, position.width, height);
                            
                                EditorGUI.PropertyField(propRect, prop, true);
                                yOffset += height + EditorGUIUtility.standardVerticalSpacing;
                            }
                            while (prop.NextVisible(false));
                        }
                    
                        serializedObject.ApplyModifiedProperties();
                    
                        // Reset the indent
                        EditorGUI.indentLevel--;
                    }
                }
            }
        
            EditorGUI.EndProperty();
        }
    }
