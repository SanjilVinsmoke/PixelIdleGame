using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class ScriptableObjectEditorWindow : EditorWindow
{
    private Vector2 scrollPosition;
    private string searchString = "";
    private List<ScriptableObject> foundObjects = new List<ScriptableObject>();
    private Dictionary<ScriptableObject, bool> foldoutStates = new Dictionary<ScriptableObject, bool>();
    private GUIStyle headerStyle;
    private ScriptableObject selectedObject;

    // Add a menu item to open the window
    [MenuItem("Tools/Scriptable Object Editor")]
    public static void ShowWindow()
    {
        GetWindow<ScriptableObjectEditorWindow>("Scriptable Object Editor");
    }

    private void OnEnable()
    {
        // Initialize style
        headerStyle = new GUIStyle();
        headerStyle.fontStyle = FontStyle.Bold;
        headerStyle.fontSize = 14;
        headerStyle.margin = new RectOffset(5, 5, 5, 5);

        // Load all scriptable objects on startup
        RefreshObjectList();
    }

    private void OnGUI()
    {
        DrawHeader();
        DrawSearchBar();
        DrawObjectList();
        DrawSelectedObjectDetails();
        
        // Handle keyboard shortcuts
        HandleKeyboardShortcuts();
    }

    private void DrawHeader()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        GUILayout.Label("Scriptable Object Editor", headerStyle);
        GUILayout.FlexibleSpace();
        
        if (GUILayout.Button("Refresh", EditorStyles.toolbarButton))
        {
            RefreshObjectList();
        }
        
        if (GUILayout.Button("Create New", EditorStyles.toolbarButton))
        {
            ShowCreateMenu();
        }
        
        EditorGUILayout.EndHorizontal();
    }

    private void DrawSearchBar()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        GUILayout.Label("Search:", GUILayout.Width(50));
        string newSearchString = EditorGUILayout.TextField(searchString);
        
        if (newSearchString != searchString)
        {
            searchString = newSearchString;
            FilterObjectList();
        }
        
        EditorGUILayout.EndHorizontal();
    }

    private void DrawObjectList()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        GUILayout.Label("Found Objects: " + foundObjects.Count, EditorStyles.boldLabel);
        
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));
        
        foreach (var obj in foundObjects)
        {
            if (obj == null) continue;
            
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            
            // Initialize foldout state if not exists
            if (!foldoutStates.ContainsKey(obj))
            {
                foldoutStates[obj] = false;
            }
            
            foldoutStates[obj] = EditorGUILayout.Foldout(foldoutStates[obj], obj.name);
            
            // Show object type
            GUILayout.Label(obj.GetType().Name, EditorStyles.miniLabel);
            
            // Select button
            if (GUILayout.Button("Select", EditorStyles.miniButton, GUILayout.Width(60)))
            {
                selectedObject = obj;
                Selection.activeObject = obj;
            }
            
            EditorGUILayout.EndHorizontal();
            
            // Draw object details if foldout is expanded
            if (foldoutStates[obj])
            {
                EditorGUI.indentLevel++;
                
                // Get the path to the asset
                string path = AssetDatabase.GetAssetPath(obj);
                EditorGUILayout.LabelField("Path:", path);
                
                EditorGUI.indentLevel--;
            }
        }
        
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    private void DrawSelectedObjectDetails()
    {
        if (selectedObject == null)
        {
            EditorGUILayout.HelpBox("No object selected", MessageType.Info);
            return;
        }

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        GUILayout.Label("Object Details", EditorStyles.boldLabel);
        
        // Create a custom editor for the selected object
        Editor editor = Editor.CreateEditor(selectedObject);
        editor.OnInspectorGUI();
        
        EditorGUILayout.Space();
        
        // Actions for the selected object
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Duplicate"))
        {
            DuplicateObject(selectedObject);
        }
        
        if (GUILayout.Button("Delete"))
        {
            if (EditorUtility.DisplayDialog("Delete Object", 
                "Are you sure you want to delete " + selectedObject.name + "?", 
                "Delete", "Cancel"))
            {
                DeleteObject(selectedObject);
            }
        }
        
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.EndVertical();
    }

    private void RefreshObjectList()
    {
        foundObjects.Clear();
        
        // Find all scriptable objects in the project
        string[] guids = AssetDatabase.FindAssets("t:ScriptableObject");
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ScriptableObject obj = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
            
            if (obj != null)
            {
                foundObjects.Add(obj);
            }
        }
        
        // Apply search filter if we have one
        if (!string.IsNullOrEmpty(searchString))
        {
            FilterObjectList();
        }
    }

    private void FilterObjectList()
    {
        if (string.IsNullOrEmpty(searchString))
        {
            RefreshObjectList();
            return;
        }
        
        string searchLower = searchString.ToLower();
        
        // Find all scriptable objects matching the search
        string[] guids = AssetDatabase.FindAssets("t:ScriptableObject");
        foundObjects.Clear();
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string name = Path.GetFileNameWithoutExtension(path);
            
            if (name.ToLower().Contains(searchLower))
            {
                ScriptableObject obj = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
                if (obj != null)
                {
                    foundObjects.Add(obj);
                }
            }
        }
    }

    private void ShowCreateMenu()
    {
        // Get all types that derive from ScriptableObject
        var derivedTypes = GetAllScriptableObjectTypes();
        
        // Create and show dropdown menu
        GenericMenu menu = new GenericMenu();
        
        foreach (var type in derivedTypes)
        {
            // Skip abstract classes and ScriptableObject itself
            if (type.IsAbstract || type == typeof(ScriptableObject))
                continue;
                
            menu.AddItem(new GUIContent(type.Name), false, () => CreateNewScriptableObject(type));
        }
        
        menu.ShowAsContext();
    }

    private System.Type[] GetAllScriptableObjectTypes()
    {
        return System.AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsSubclassOf(typeof(ScriptableObject)))
            .OrderBy(type => type.Name)
            .ToArray();
    }

    private void CreateNewScriptableObject(System.Type type)
    {
        // Create the scriptable object instance
        ScriptableObject newObj = ScriptableObject.CreateInstance(type);
        
        // Create save dialog
        string path = EditorUtility.SaveFilePanelInProject(
            "Save Scriptable Object",
            "New" + type.Name,
            "asset",
            "Save the scriptable object where?"
        );
        
        if (string.IsNullOrEmpty(path))
            return;
            
        // Create the asset file
        AssetDatabase.CreateAsset(newObj, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        // Select the newly created object
        selectedObject = newObj;
        Selection.activeObject = newObj;
        
        // Refresh the list
        RefreshObjectList();
    }

    private void DuplicateObject(ScriptableObject obj)
    {
        string originalPath = AssetDatabase.GetAssetPath(obj);
        string newPath = AssetDatabase.GenerateUniqueAssetPath(originalPath);
        
        if (AssetDatabase.CopyAsset(originalPath, newPath))
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            // Select the duplicated object
            ScriptableObject duplicated = AssetDatabase.LoadAssetAtPath<ScriptableObject>(newPath);
            selectedObject = duplicated;
            Selection.activeObject = duplicated;
            
            // Refresh the list
            RefreshObjectList();
        }
        else
        {
            Debug.LogError("Failed to duplicate object: " + obj.name);
        }
    }

    private void DeleteObject(ScriptableObject obj)
    {
        string path = AssetDatabase.GetAssetPath(obj);
        
        if (AssetDatabase.DeleteAsset(path))
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            // Clear selection
            selectedObject = null;
            Selection.activeObject = null;
            
            // Refresh the list
            RefreshObjectList();
        }
        else
        {
            Debug.LogError("Failed to delete object: " + obj.name);
        }
    }

    private void HandleKeyboardShortcuts()
    {
        Event e = Event.current;
        
        if (e.type == EventType.KeyDown)
        {
            // F5 to refresh
            if (e.keyCode == KeyCode.F5)
            {
                RefreshObjectList();
                e.Use();
            }
            
            // Ctrl+N to create new
            if (e.keyCode == KeyCode.N && e.control)
            {
                ShowCreateMenu();
                e.Use();
            }
        }
    }
}