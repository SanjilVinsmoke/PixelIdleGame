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
    
    // Folder filtering
    private bool useProjectFilter = true;
    private bool useFolderFilter = false;
    private string selectedFolderPath = "";
    private string folderDisplayName = "No folder selected";
    private List<string> recentFolders = new List<string>();
    private const int MAX_RECENT_FOLDERS = 5;

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

        // Load recent folders from EditorPrefs
        LoadRecentFolders();

        // Load all scriptable objects on startup
        RefreshObjectList();
    }

    private void OnGUI()
    {
        DrawHeader();
        DrawFilterOptions();
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

    private void DrawFilterOptions()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        GUILayout.Label("Filter Settings", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        
        // Project filter toggle
        bool newUseProjectFilter = EditorGUILayout.ToggleLeft("Entire Project", useProjectFilter, GUILayout.Width(120));
        if (newUseProjectFilter != useProjectFilter)
        {
            useProjectFilter = newUseProjectFilter;
            if (useProjectFilter)
            {
                useFolderFilter = false;
            }
            RefreshObjectList();
        }
        
        // Folder filter toggle
        bool newUseFolderFilter = EditorGUILayout.ToggleLeft("Specific Folder", useFolderFilter, GUILayout.Width(120));
        if (newUseFolderFilter != useFolderFilter)
        {
            useFolderFilter = newUseFolderFilter;
            if (useFolderFilter)
            {
                useProjectFilter = false;
                if (string.IsNullOrEmpty(selectedFolderPath))
                {
                    SelectFolder();
                }
            }
            RefreshObjectList();
        }

        EditorGUILayout.EndHorizontal();

        // Show folder selection if folder filter is enabled
        if (useFolderFilter)
        {
            EditorGUILayout.BeginHorizontal();
            
            EditorGUILayout.LabelField("Current Folder:", GUILayout.Width(100));
            EditorGUILayout.LabelField(folderDisplayName, EditorStyles.boldLabel);
            
            if (GUILayout.Button("Change", GUILayout.Width(80)))
            {
                SelectFolder();
            }
            
            EditorGUILayout.EndHorizontal();

            // Recent folders
            if (recentFolders.Count > 0)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Recent Folders:", EditorStyles.boldLabel);
                
                for (int i = 0; i < recentFolders.Count; i++)
                {
                    string path = recentFolders[i];
                    string folderName = Path.GetFileName(path);
                    if (string.IsNullOrEmpty(folderName))
                        folderName = path; // For root folders
                        
                    EditorGUILayout.BeginHorizontal();
                    
                    if (GUILayout.Button(folderName, EditorStyles.linkLabel))
                    {
                        selectedFolderPath = path;
                        UpdateFolderDisplayName();
                        RefreshObjectList();
                        
                        // Move to top of recent list
                        if (i > 0)
                        {
                            recentFolders.RemoveAt(i);
                            recentFolders.Insert(0, path);
                            SaveRecentFolders();
                        }
                    }
                    
                    // Show full path on hover
                    if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                    {
                        Rect tooltipRect = GUILayoutUtility.GetLastRect();
                        GUI.Label(tooltipRect, new GUIContent("", path));
                    }
                    
                    EditorGUILayout.EndHorizontal();
                }
            }
        }
        
        EditorGUILayout.EndVertical();
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
        string locationText = useProjectFilter ? "Project" : (useFolderFilter ? "Folder" : "");
        GUILayout.Label($"Found Objects ({locationText}): {foundObjects.Count}", EditorStyles.boldLabel);
        
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
        UnityEditor.Editor editor = UnityEditor.Editor.CreateEditor(selectedObject);
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
        
        if (useProjectFilter)
        {
            // Find all scriptable objects in the project
            LoadScriptableObjectsFromProject();
        }
        else if (useFolderFilter && !string.IsNullOrEmpty(selectedFolderPath))
        {
            // Find all scriptable objects in the selected folder
            LoadScriptableObjectsFromFolder(selectedFolderPath);
        }
        
        // Apply search filter if we have one
        if (!string.IsNullOrEmpty(searchString))
        {
            FilterObjectList();
        }
    }

    private void LoadScriptableObjectsFromProject()
    {
        string[] guids = AssetDatabase.FindAssets("t:ScriptableObject");
        LoadObjectsFromGuids(guids);
    }

    private void LoadScriptableObjectsFromFolder(string folderPath)
    {
        string[] guids = AssetDatabase.FindAssets("t:ScriptableObject", new[] { folderPath });
        LoadObjectsFromGuids(guids);
    }

    private void LoadObjectsFromGuids(string[] guids)
    {
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ScriptableObject obj = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
            
            if (obj != null)
            {
                foundObjects.Add(obj);
            }
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
        List<ScriptableObject> filteredList = new List<ScriptableObject>();
        
        foreach (var obj in foundObjects)
        {
            if (obj != null && obj.name.ToLower().Contains(searchLower))
            {
                filteredList.Add(obj);
            }
        }
        
        foundObjects = filteredList;
    }

    private void SelectFolder()
    {
        string startFolder = string.IsNullOrEmpty(selectedFolderPath) ? "Assets" : selectedFolderPath;
        string folderPath = EditorUtility.OpenFolderPanel("Select Folder for ScriptableObjects", startFolder, "");
        
        if (string.IsNullOrEmpty(folderPath))
            return;
            
        // Convert to project relative path if needed
        if (folderPath.StartsWith(Application.dataPath))
        {
            folderPath = "Assets" + folderPath.Substring(Application.dataPath.Length);
        }
        else if (!folderPath.StartsWith("Assets"))
        {
            // Not in the project
            EditorUtility.DisplayDialog("Invalid Folder", 
                "Please select a folder inside your Unity project.", "OK");
            return;
        }
        
        selectedFolderPath = folderPath;
        UpdateFolderDisplayName();
        
        // Add to recent folders
        AddToRecentFolders(folderPath);
        
        // Refresh the list
        RefreshObjectList();
    }

    private void UpdateFolderDisplayName()
    {
        if (string.IsNullOrEmpty(selectedFolderPath))
        {
            folderDisplayName = "No folder selected";
            return;
        }
        
        folderDisplayName = selectedFolderPath;
    }

    private void AddToRecentFolders(string folderPath)
    {
        // Remove if already exists
        recentFolders.Remove(folderPath);
        
        // Add to beginning of list
        recentFolders.Insert(0, folderPath);
        
        // Trim list if needed
        if (recentFolders.Count > MAX_RECENT_FOLDERS)
        {
            recentFolders.RemoveAt(recentFolders.Count - 1);
        }
        
        // Save to EditorPrefs
        SaveRecentFolders();
    }

    private void SaveRecentFolders()
    {
        for (int i = 0; i < MAX_RECENT_FOLDERS; i++)
        {
            string key = "SOEditor_RecentFolder_" + i;
            
            if (i < recentFolders.Count)
            {
                EditorPrefs.SetString(key, recentFolders[i]);
            }
            else
            {
                EditorPrefs.DeleteKey(key);
            }
        }
    }

    private void LoadRecentFolders()
    {
        recentFolders.Clear();
        
        for (int i = 0; i < MAX_RECENT_FOLDERS; i++)
        {
            string key = "SOEditor_RecentFolder_" + i;
            
            if (EditorPrefs.HasKey(key))
            {
                string folderPath = EditorPrefs.GetString(key);
                
                if (!string.IsNullOrEmpty(folderPath))
                {
                    recentFolders.Add(folderPath);
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
        
        // Determine default save path
        string initialPath = "Assets";
        if (useFolderFilter && !string.IsNullOrEmpty(selectedFolderPath))
        {
            initialPath = selectedFolderPath;
        }
        
        // Create save dialog
        string path = EditorUtility.SaveFilePanelInProject(
            "Save Scriptable Object",
            "New" + type.Name,
            "asset",
            "Save the scriptable object where?",
            initialPath
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