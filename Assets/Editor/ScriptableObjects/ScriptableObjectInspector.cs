using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

// Make sure this file is in an Editor folder
[CustomEditor(typeof(ScriptableObject), true)]
public class ScriptableObjectInspector : UnityEditor.Editor
{
    private bool showDebugData = false;
    private Dictionary<string, bool> foldoutStates = new Dictionary<string, bool>();
    private GUIStyle headerStyle;
    private int maxDepth = 3;
    private bool showPrivateFields = false;
    private bool showBackingFields = false;
    
    public override void OnInspectorGUI()
    {
        // Initialize styles
        if (headerStyle == null)
        {
            headerStyle = new GUIStyle(EditorStyles.boldLabel);
            headerStyle.fontSize += 1;
        }
        
        // Draw the default inspector first
        DrawDefaultInspector();
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        
        // Debug inspection section
        EditorGUILayout.LabelField("ScriptableObject Debug Inspector", headerStyle);
        
        // Get the target ScriptableObject
        ScriptableObject targetSO = (ScriptableObject)target;
        
        // Check if this is actually a ScriptableObject
        if (targetSO != null)
        {
            EditorGUILayout.BeginHorizontal();
            showDebugData = EditorGUILayout.Foldout(showDebugData, "Debug Data", true);
            EditorGUILayout.EndHorizontal();
            
            if (showDebugData)
            {
                EditorGUILayout.Space();
                
                // Inspection settings
                EditorGUILayout.BeginHorizontal();
                maxDepth = EditorGUILayout.IntSlider("Max Depth", maxDepth, 1, 5);
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.BeginHorizontal();
                showPrivateFields = EditorGUILayout.ToggleLeft("Show Private Fields", showPrivateFields, GUILayout.Width(150));
                showBackingFields = EditorGUILayout.ToggleLeft("Show Backing Fields", showBackingFields, GUILayout.Width(150));
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.Space();
                
                // Begin inspecting the ScriptableObject
                EditorGUI.indentLevel++;
                InspectObject(targetSO, "root", 0);
                EditorGUI.indentLevel--;
            }
        }
        
        // Apply changes
        serializedObject.ApplyModifiedProperties();
    }
    
    private void InspectObject(object obj, string path, int depth)
    {
        if (obj == null)
        {
            EditorGUILayout.LabelField("null");
            return;
        }

        if (depth > maxDepth)
        {
            EditorGUILayout.LabelField($"(Max depth reached)", EditorStyles.miniLabel);
            return;
        }

        Type type = obj.GetType();
        
        // For Unity objects (except root), display a reference field
        if (obj is UnityEngine.Object unityObj && depth > 0)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField(unityObj, type, false);
            EditorGUI.EndDisabledGroup();
            return;
        }

        // Handle arrays and lists
        if (obj is IList list)
        {
            string listPath = string.IsNullOrEmpty(path) ? "List" : path;
            bool showList = GetFoldoutState(listPath);
            showList = EditorGUILayout.Foldout(showList, $"{(depth == 0 ? "" : "List")} [{list.Count} items]", true);
            SetFoldoutState(listPath, showList);

            if (showList)
            {
                EditorGUI.indentLevel++;
                for (int i = 0; i < list.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField($"[{i}]", GUILayout.Width(30));
                    EditorGUILayout.BeginVertical();
                    InspectObject(list[i], $"{listPath}[{i}]", depth + 1);
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUI.indentLevel--;
            }
            return;
        }

        // Handle dictionaries
        if (obj is IDictionary dict)
        {
            string dictPath = string.IsNullOrEmpty(path) ? "Dictionary" : path;
            bool showDict = GetFoldoutState(dictPath);
            showDict = EditorGUILayout.Foldout(showDict, $"{(depth == 0 ? "" : "Dictionary")} [{dict.Count} entries]", true);
            SetFoldoutState(dictPath, showDict);

            if (showDict)
            {
                EditorGUI.indentLevel++;
                foreach (DictionaryEntry entry in dict)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField($"Key: {entry.Key}", GUILayout.Width(120));
                    EditorGUILayout.BeginVertical();
                    InspectObject(entry.Value, $"{dictPath}[{entry.Key}]", depth + 1);
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUI.indentLevel--;
            }
            return;
        }

        // Handle primitive types
        if (type.IsPrimitive || type == typeof(string) || type == typeof(decimal))
        {
            EditorGUILayout.LabelField(obj.ToString());
            return;
        }

        // For other objects, reflect through fields and properties
        BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
        if (showPrivateFields)
            flags |= BindingFlags.NonPublic;

        // Get fields
        FieldInfo[] fields = type.GetFields(flags);
        List<FieldInfo> filteredFields = new List<FieldInfo>();
        
        foreach (FieldInfo field in fields)
        {
            // Skip backing fields for properties if not showing them
            if (!showBackingFields && field.Name.Contains("k__BackingField"))
                continue;
                
            // Skip compiler-generated fields
            if (field.Name.StartsWith("<") && !showBackingFields)
                continue;
                
            filteredFields.Add(field);
        }
        
        if (filteredFields.Count > 0)
        {
            string fieldPath = path + ".Fields";
            bool showFields = GetFoldoutState(fieldPath);
            showFields = EditorGUILayout.Foldout(showFields, depth == 0 ? "Fields" : $"{type.Name} Fields", true);
            SetFoldoutState(fieldPath, showFields);

            if (showFields)
            {
                EditorGUI.indentLevel++;
                foreach (FieldInfo field in filteredFields)
                {
                    EditorGUILayout.BeginHorizontal();
                    
                    // Add style for private fields
                    string fieldName = field.Name;
                    if ((field.Attributes & FieldAttributes.Private) != 0)
                        fieldName = "(" + fieldName + ")";
                        
                    // For backing fields, show prettier name
                    if (field.Name.Contains("k__BackingField"))
                    {
                        fieldName = field.Name.Replace("<", "").Replace(">k__BackingField", "");
                        fieldName = "[backing] " + fieldName;
                    }
                    
                    EditorGUILayout.LabelField(fieldName, GUILayout.Width(140));
                    
                    EditorGUILayout.BeginVertical();
                    try
                    {
                        object value = field.GetValue(obj);
                        InspectObject(value, $"{fieldPath}.{field.Name}", depth + 1);
                    }
                    catch (Exception e)
                    {
                        EditorGUILayout.LabelField($"<Error: {e.Message}>", EditorStyles.miniLabel);
                    }
                    EditorGUILayout.EndVertical();
                    
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUI.indentLevel--;
            }
        }

        // Get properties with getters and no parameters
        PropertyInfo[] properties = type.GetProperties(flags);
        List<PropertyInfo> validProperties = new List<PropertyInfo>();
        
        foreach (PropertyInfo prop in properties)
        {
            if (prop.CanRead && prop.GetIndexParameters().Length == 0)
            {
                // Skip properties with [HideInInspector]
                if (Attribute.IsDefined(prop, typeof(HideInInspector)))
                    continue;
                    
                validProperties.Add(prop);
            }
        }
        
        if (validProperties.Count > 0)
        {
            string propPath = path + ".Properties";
            bool showProps = GetFoldoutState(propPath);
            showProps = EditorGUILayout.Foldout(showProps, depth == 0 ? "Properties" : $"{type.Name} Properties", true);
            SetFoldoutState(propPath, showProps);

            if (showProps)
            {
                EditorGUI.indentLevel++;
                foreach (PropertyInfo prop in validProperties)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(prop.Name, GUILayout.Width(140));
                    
                    EditorGUILayout.BeginVertical();
                    try
                    {
                        object value = prop.GetValue(obj);
                        InspectObject(value, $"{propPath}.{prop.Name}", depth + 1);
                    }
                    catch (Exception e)
                    {
                        EditorGUILayout.LabelField($"<Error: {e.Message}>", EditorStyles.miniLabel);
                    }
                    EditorGUILayout.EndVertical();
                    
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUI.indentLevel--;
            }
        }
    }

    private bool GetFoldoutState(string path)
    {
        if (!foldoutStates.ContainsKey(path))
            foldoutStates[path] = true; // Default to expanded
        return foldoutStates[path];
    }

    private void SetFoldoutState(string path, bool state)
    {
        foldoutStates[path] = state;
    }
}