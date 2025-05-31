using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using ScriptableObjects;
using Managers;
using Utils;

namespace Editor
{
    public class EnemyCreationTool : EditorWindow
    {
        private EnemyTemplate selectedTemplate;
        private EnemyDataSo currentEnemyData;
        private GameObject selectedEnemyPrefab;
        private Vector2 scrollPosition;
        private bool showAdvancedSettings = false;
        private bool autoSizeColliders = true;
        
        private List<EnemyTemplate> availableTemplates;
        private string[] templateNames;
        private int selectedTemplateIndex = 0;
        
        [MenuItem("Tools/Enemy System/Enemy Creation Tool")]
        public static void ShowWindow()
        {
            GetWindow<EnemyCreationTool>("Enemy Creation Tool");
        }
        
        private void OnEnable()
        {
            LoadAvailableTemplates();
        }
        
        private void LoadAvailableTemplates()
        {
            availableTemplates = AssetDatabase.FindAssets("t:EnemyTemplate")
                .Select(guid => AssetDatabase.LoadAssetAtPath<EnemyTemplate>(AssetDatabase.GUIDToAssetPath(guid)))
                .Where(template => template != null)
                .ToList();
            
            // Initialize arrays to prevent null reference
            if (availableTemplates == null)
                availableTemplates = new List<EnemyTemplate>();
                
            templateNames = availableTemplates.Select(t => t?.templateName ?? "Unnamed Template").ToArray();
            
            // Ensure selectedTemplateIndex is valid
            if (selectedTemplateIndex >= templateNames.Length)
                selectedTemplateIndex = 0;
                
            // Set selectedTemplate if we have templates
            if (availableTemplates.Count > 0 && selectedTemplateIndex >= 0)
                selectedTemplate = availableTemplates[selectedTemplateIndex];
        }
        
        private void OnGUI()
        {
            // Wrap everything in try-catch to prevent GUI errors from breaking the window
            try
            {
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                
                GUILayout.Label("Enemy Creation Tool", EditorStyles.boldLabel);
                EditorGUILayout.Space();
                
                DrawTemplateSelection();
                EditorGUILayout.Space();
                
                if (selectedTemplate != null)
                {
                    DrawEnemyConfiguration();
                    EditorGUILayout.Space();
                    DrawStateConfiguration();
                    EditorGUILayout.Space();
                    DrawCreationOptions();
                    EditorGUILayout.Space();
                    DrawCreationButtons();
                }
                
                EditorGUILayout.EndScrollView();
            }
            catch (Exception e)
            {
                // End scroll view if it was started
                if (Event.current.type == EventType.Repaint)
                {
                    EditorGUILayout.EndScrollView();
                }
                GUILayout.Label($"Error: {e.Message}", EditorStyles.helpBox);
            }
        }
        
        private void DrawTemplateSelection()
        {
            GUILayout.Label("Template Selection", EditorStyles.boldLabel);
            
            // Null safety checks
            if (templateNames == null || templateNames.Length == 0)
            {
                EditorGUILayout.HelpBox("No enemy templates found. Create an EnemyTemplate asset first.", MessageType.Warning);
                if (GUILayout.Button("Refresh Templates"))
                {
                    LoadAvailableTemplates();
                }
                return;
            }
            
            // Ensure selectedTemplateIndex is within bounds
            selectedTemplateIndex = Mathf.Clamp(selectedTemplateIndex, 0, templateNames.Length - 1);
            
            int newSelectedIndex = EditorGUILayout.Popup("Enemy Template", selectedTemplateIndex, templateNames);
            if (newSelectedIndex != selectedTemplateIndex)
            {
                selectedTemplateIndex = newSelectedIndex;
                if (selectedTemplateIndex >= 0 && selectedTemplateIndex < availableTemplates.Count)
                {
                    selectedTemplate = availableTemplates[selectedTemplateIndex];
                    // Clear current enemy data when template changes
                    currentEnemyData = null;
                }
            }
            
            // Display template info with null checks
            if (selectedTemplate != null)
            {
                if (selectedTemplate.icon != null && selectedTemplate.icon.texture != null)
                {
                    GUILayout.Label(selectedTemplate.icon.texture, GUILayout.Width(64), GUILayout.Height(64));
                }
                
                EditorGUILayout.LabelField("Description", selectedTemplate.description ?? "No description", EditorStyles.wordWrappedLabel);
                EditorGUILayout.LabelField("Enemy Type", selectedTemplate.enemyType.ToString());
            }
        }
        
        private void DrawEnemyConfiguration()
        {
            GUILayout.Label("Enemy Configuration", EditorStyles.boldLabel);
            
            currentEnemyData = EditorGUILayout.ObjectField("Enemy Data", currentEnemyData, typeof(EnemyDataSo), false) as EnemyDataSo;
            
            if (currentEnemyData == null && GUILayout.Button("Create New Enemy Data"))
            {
                CreateNewEnemyData();
            }
            
            if (currentEnemyData != null)
            {
                SerializedObject serializedData = new SerializedObject(currentEnemyData);
                serializedData.Update();
                
                // Safe property field drawing with null checks
                DrawPropertyFieldSafe(serializedData, "enemyName");
                DrawPropertyFieldSafe(serializedData, "maxHealth");
                DrawPropertyFieldSafe(serializedData, "moveSpeed");
                DrawPropertyFieldSafe(serializedData, "attackDamage");
                DrawPropertyFieldSafe(serializedData, "attackRange");
                DrawPropertyFieldSafe(serializedData, "detectionRange");
                DrawPropertyFieldSafe(serializedData, "colliderType");
                
                showAdvancedSettings = EditorGUILayout.Foldout(showAdvancedSettings, "Advanced Settings");
                if (showAdvancedSettings)
                {
                    EditorGUI.indentLevel++;
                    DrawPropertyFieldSafe(serializedData, "animatorController");
                    DrawPropertyFieldSafe(serializedData, "animationSpeed");
                    DrawPropertyFieldSafe(serializedData, "rotationSpeed");
                    DrawPropertyFieldSafe(serializedData, "attackCooldown");
                    DrawPropertyFieldSafe(serializedData, "regenerateHealth");
                    
                    var regenProperty = serializedData.FindProperty("regenerateHealth");
                    if (regenProperty != null && regenProperty.boolValue)
                    {
                        DrawPropertyFieldSafe(serializedData, "healthRegenRate");
                    }
                    
                    // Touch damage settings
                    var hasTouchDamageProperty = serializedData.FindProperty("hasTouchDamage");
                    if (hasTouchDamageProperty != null)
                    {
                        EditorGUILayout.PropertyField(hasTouchDamageProperty);
                        if (hasTouchDamageProperty.boolValue)
                        {
                            DrawPropertyFieldSafe(serializedData, "touchDamage");
                        }
                    }
                    
                    EditorGUI.indentLevel--;
                }
                
                serializedData.ApplyModifiedProperties();
            }
        }
        
        private void DrawPropertyFieldSafe(SerializedObject serializedObject, string propertyName)
        {
            var property = serializedObject.FindProperty(propertyName);
            if (property != null)
            {
                EditorGUILayout.PropertyField(property);
            }
            else
            {
                EditorGUILayout.LabelField($"Property '{propertyName}' not found", EditorStyles.helpBox);
            }
        }
        
        private void DrawStateConfiguration()
        {
            if (currentEnemyData == null) return;
            
            GUILayout.Label("State Machine Configuration", EditorStyles.boldLabel);
            
            SerializedObject serializedData = new SerializedObject(currentEnemyData);
            serializedData.Update();
            
            DrawPropertyFieldSafe(serializedData, "defaultState");
            DrawPropertyFieldSafe(serializedData, "availableStates");
            
            if (GUILayout.Button("Reset to Template States"))
            {
                ResetToTemplateStates();
            }
            
            serializedData.ApplyModifiedProperties();
        }
        
        private void DrawCreationOptions()
        {
            GUILayout.Label("Creation Options", EditorStyles.boldLabel);
            
            autoSizeColliders = EditorGUILayout.Toggle("Auto-size Colliders to Sprite", autoSizeColliders);
            
            EditorGUILayout.HelpBox("Auto-size will automatically adjust collider sizes based on sprite bounds.", MessageType.Info);
        }
        
        private void DrawCreationButtons()
        {
            GUILayout.Label("Actions", EditorStyles.boldLabel);
            
            selectedEnemyPrefab = EditorGUILayout.ObjectField("Target Prefab", selectedEnemyPrefab, typeof(GameObject), false) as GameObject;
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Prefab Creation", EditorStyles.boldLabel);
            
            EditorGUI.BeginDisabledGroup(currentEnemyData == null);
            if (GUILayout.Button("Create New Enemy Prefab"))
            {
                CreateEnemyPrefab();
            }
            EditorGUI.EndDisabledGroup();
            
            EditorGUI.BeginDisabledGroup(selectedEnemyPrefab == null || currentEnemyData == null);
            if (GUILayout.Button("Apply Data to Selected Prefab"))
            {
                ApplyDataToPrefab();
            }
            EditorGUI.EndDisabledGroup();
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Scene Creation", EditorStyles.boldLabel);
            
            EditorGUI.BeginDisabledGroup(currentEnemyData == null);
            if (GUILayout.Button("Create Enemy in Scene"))
            {
                CreateEnemyInScene();
            }
            EditorGUI.EndDisabledGroup();
            
            EditorGUI.BeginDisabledGroup(currentEnemyData?.enemyPrefab == null);
            if (GUILayout.Button("Instantiate from Prefab"))
            {
                var instance = PrefabUtility.InstantiatePrefab(currentEnemyData.enemyPrefab) as GameObject;
                Selection.activeGameObject = instance;
            }
            EditorGUI.EndDisabledGroup();
            
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Basic enemy structure includes:\n• Rigidbody2D and Collider2D on parent\n• Sprite child with SpriteRenderer\n• Hitbox child with trigger collider", MessageType.Info);
        }
        
        private void CreateNewEnemyData()
        {
            if (selectedTemplate == null)
            {
                Debug.LogError("No template selected. Cannot create enemy data.");
                return;
            }
            
            currentEnemyData = CreateInstance<EnemyDataSo>();
            currentEnemyData.name = $"New {selectedTemplate.templateName} Data";
            
            // Copy default values from template
            if (selectedTemplate.defaultData != null)
            {
                EditorUtility.CopySerialized(selectedTemplate.defaultData, currentEnemyData);
            }
            
            // Set default states with null check
            if (selectedTemplate.defaultStates != null)
            {
                currentEnemyData.availableStates = new List<EnemyStateData>(selectedTemplate.defaultStates);
            }
            else
            {
                currentEnemyData.availableStates = new List<EnemyStateData>();
            }
            
            string path = EditorUtility.SaveFilePanelInProject(
                "Save Enemy Data",
                currentEnemyData.name,
                "asset",
                "Save enemy data asset");
            
            if (!string.IsNullOrEmpty(path))
            {
                AssetDatabase.CreateAsset(currentEnemyData, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
        
        private void ResetToTemplateStates()
        {
            if (currentEnemyData != null && selectedTemplate != null && selectedTemplate.defaultStates != null)
            {
                currentEnemyData.availableStates = new List<EnemyStateData>(selectedTemplate.defaultStates);
                EditorUtility.SetDirty(currentEnemyData);
            }
        }
        
        private void CreateEnemyPrefab()
        {
            if (currentEnemyData == null || selectedTemplate == null) return;
            
            GameObject prefab = selectedTemplate.basePrefab != null ? 
                Instantiate(selectedTemplate.basePrefab) : 
                new GameObject(currentEnemyData.enemyName ?? "New Enemy");
            
            ApplyDataToGameObject(prefab);
            
            string path = EditorUtility.SaveFilePanelInProject(
                "Save Enemy Prefab",
                currentEnemyData.enemyName ?? "New Enemy",
                "prefab",
                "Save enemy prefab");
            
            if (!string.IsNullOrEmpty(path))
            {
                GameObject savedPrefab = PrefabUtility.SaveAsPrefabAsset(prefab, path);
                currentEnemyData.enemyPrefab = savedPrefab;
                EditorUtility.SetDirty(currentEnemyData);
                AssetDatabase.SaveAssets();
                
                Debug.Log($"Enemy prefab created at: {path}");
            }
            
            DestroyImmediate(prefab);
        }
        
        private void ApplyDataToPrefab()
        {
            if (selectedEnemyPrefab == null || currentEnemyData == null) return;
            
            string prefabPath = AssetDatabase.GetAssetPath(selectedEnemyPrefab);
            if (string.IsNullOrEmpty(prefabPath))
            {
                Debug.LogError("Selected prefab is not a valid asset.");
                return;
            }
            
            GameObject instance = PrefabUtility.LoadPrefabContents(prefabPath);
            ApplyDataToGameObject(instance);
            PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
            PrefabUtility.UnloadPrefabContents(instance);
            
            Debug.Log($"Applied data to prefab: {selectedEnemyPrefab.name}");
        }
        
        private void CreateEnemyInScene()
        {
            if (currentEnemyData == null) return;
            
            GameObject enemy = currentEnemyData.enemyPrefab != null ? 
                Instantiate(currentEnemyData.enemyPrefab) : 
                new GameObject(currentEnemyData.enemyName ?? "New Enemy");
            
            if (currentEnemyData.enemyPrefab == null)
            {
                ApplyDataToGameObject(enemy);
            }
            
            Selection.activeGameObject = enemy;
            SceneView.FrameLastActiveSceneView();
            
            Debug.Log($"Created enemy in scene: {enemy.name}");
        }
        
        private void ApplyDataToGameObject(GameObject target)
        {
            if (target == null || selectedTemplate == null) return;
            
            // Ensure basic enemy structure exists
            SetupBasicEnemyStructure(target);
            
            // Add enemy type-specific script
            AttachEnemyTypeScript(target);
            
            // Add required components if missing
            if (selectedTemplate.requiredComponents != null)
            {
                foreach (string componentName in selectedTemplate.requiredComponents)
                {
                    if (string.IsNullOrEmpty(componentName)) continue;
                    
                    System.Type componentType = GetComponentType(componentName);
                    if (componentType != null && target.GetComponent(componentType) == null)
                    {
                        target.AddComponent(componentType);
                    }
                }
            }
            
            // Apply enemy data reference
            ApplyEnemyDataToComponents(target);
        }

        private void AttachEnemyTypeScript(GameObject target)
        {
            if (selectedTemplate == null) return;
            
            // Generate enemy script name based on type
            string enemyScriptName = $"{selectedTemplate.enemyType}";
            
            // Check if enemy script already exists
            var existingEnemyComponent = target.GetComponents<MonoBehaviour>()
                .FirstOrDefault(c => c != null && c.GetType().BaseType?.IsGenericType == true && 
                                c.GetType().BaseType.GetGenericTypeDefinition() == typeof(Enemy<>));
            
            if (existingEnemyComponent != null && existingEnemyComponent.GetType().Name == enemyScriptName)
                return; // Already has correct enemy script
            
            // Remove existing enemy script if different
            if (existingEnemyComponent != null)
            {
                DestroyImmediate(existingEnemyComponent);
            }
            
            // Try to add the enemy type script
            System.Type enemyType = GetEnemyTypeScript(enemyScriptName);
            if (enemyType != null)
            {
                target.AddComponent(enemyType);
                Debug.Log($"Added {enemyScriptName} to {target.name}");
            }
            else
            {
                Debug.LogWarning($"Enemy script '{enemyScriptName}' not found. Make sure it exists and inherits from Enemy<T>.");
                // Fallback: try to add a generic enemy script
                var genericEnemyType = GetEnemyTypeScript("Enemy");
                if (genericEnemyType != null)
                {
                    target.AddComponent(genericEnemyType);
                }
            }
        }

        private System.Type GetEnemyTypeScript(string scriptName)
        {
            if (string.IsNullOrEmpty(scriptName)) return null;
            
            // Search for enemy scripts in all assemblies
            try
            {
                foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
                {
                    var type = assembly.GetTypes().FirstOrDefault(t => 
                        t.Name == scriptName && 
                        t.BaseType?.IsGenericType == true &&
                        t.BaseType.GetGenericTypeDefinition() == typeof(Enemy<>));
                    
                    if (type != null) return type;
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Error searching for enemy type script: {e.Message}");
            }
            
            return null;
        }

        private void ApplyEnemyDataToComponents(GameObject target)
        {
            if (target == null || currentEnemyData == null) return;
            
            var enemyComponents = target.GetComponents<MonoBehaviour>()
                .Where(c => c != null && c.GetType().BaseType?.IsGenericType == true && 
                           c.GetType().BaseType.GetGenericTypeDefinition() == typeof(Enemy<>));
            
            foreach (var enemy in enemyComponents)
            {
                try
                {
                    SerializedObject serializedEnemy = new SerializedObject(enemy);
                    var enemyDataProperty = serializedEnemy.FindProperty("enemyData");
                    if (enemyDataProperty != null)
                    {
                        enemyDataProperty.objectReferenceValue = currentEnemyData;
                        serializedEnemy.ApplyModifiedProperties();
                    }
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"Error applying enemy data to component {enemy.GetType().Name}: {e.Message}");
                }
            }
        }

        private void SetupBasicEnemyStructure(GameObject target)
        {
            if (target == null) return;
            
            // Setup parent with Rigidbody2D and Collider2D
            if (target.GetComponent<Rigidbody2D>() == null)
            {
                var rb = target.AddComponent<Rigidbody2D>();
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.gravityScale = 0f; // Assuming top-down or platformer
                rb.freezeRotation = true;
            }

            if (target.GetComponent<Collider2D>() == null)
            {
                AddCollider(target);
            }

            // Create sprite child if it doesn't exist
            Transform spriteChild = target.transform.Find("Sprite");
            GameObject spriteObject;
            
            if (spriteChild == null)
            {
                spriteObject = new GameObject("Sprite");
                spriteObject.transform.SetParent(target.transform);
                spriteObject.transform.localPosition = Vector3.zero;
                spriteObject.transform.localScale = Vector3.one;
                
                var spriteRenderer = spriteObject.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = currentEnemyData.enemyIcon;
                // Check if sorting layer exists before assigning
                if (SortingLayer.IsValid(SortingLayer.NameToID("Enemies")))
                {
                    spriteRenderer.sortingLayerName = "Enemies";
                }
                spriteRenderer.sortingOrder = 0;
                
                // Try to assign sprite from template or enemy data
                if (selectedTemplate?.icon != null)
                {
                    spriteRenderer.sprite = selectedTemplate.icon;
                }
            }
            else
            {
                spriteObject = spriteChild.gameObject;
            }
            
            // Add Animator to sprite child
            if (spriteObject.GetComponent<Animator>() == null)
            {
                var animator = spriteObject.AddComponent<Animator>();
                
                // Try to assign animator controller from template or enemy data
                if (currentEnemyData?.animatorController != null)
                {
                    animator.runtimeAnimatorController = currentEnemyData.animatorController;
                }
                else if (selectedTemplate?.defaultData?.animatorController != null)
                {
                    animator.runtimeAnimatorController = selectedTemplate.defaultData.animatorController;
                }
            }

            // Create hitbox child if it doesn't exist
            Transform hitboxChild = target.transform.Find("Hitbox");
            if (hitboxChild == null)
            {
                GameObject hitbox = new GameObject("Hitbox");
                hitbox.transform.SetParent(target.transform);
                hitbox.transform.localPosition = Vector3.zero;
                hitbox.transform.localScale = Vector3.one;
                
                var hitboxCollider = hitbox.AddComponent<BoxCollider2D>();
                hitboxCollider.isTrigger = true;
                hitboxCollider.size = new Vector2(1.2f, 1.2f);
                
                // Add hitbox tag and layer (check if they exist first)
                if (UnityEditorInternal.InternalEditorUtility.tags.Contains("EnemyHitbox"))
                    hitbox.tag = "EnemyHitbox";
                
                int hitboxLayer = LayerMask.NameToLayer("EnemyHitbox");
                if (hitboxLayer != -1)
                    hitbox.layer = hitboxLayer;
            }

            // Set enemy tag and layer (check if they exist first)
            if (UnityEditorInternal.InternalEditorUtility.tags.Contains("Enemy"))
                target.tag = "Enemy";
            
            int enemyLayer = LayerMask.NameToLayer("Enemy");
            if (enemyLayer != -1)
                target.layer = enemyLayer;
                
            // Auto-size colliders if enabled
            if (autoSizeColliders)
            {
                SetColliderSizeOnSprite(target);
            }
        }

        private System.Type GetComponentType(string componentName)
        {
            if (string.IsNullOrEmpty(componentName)) return null;
            
            try
            {
                // First try the Component namespace
                var type = System.Type.GetType($"Component.{componentName}");
                if (type != null) return type;
                
                // Try without namespace
                type = System.Type.GetType(componentName);
                if (type != null) return type;
                
                // Search in all assemblies
                foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
                {
                    type = assembly.GetType(componentName);
                    if (type != null) return type;
                    
                    // Try to find by simple name
                    type = assembly.GetTypes().FirstOrDefault(t => t.Name == componentName);
                    if (type != null) return type;
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Error getting component type '{componentName}': {e.Message}");
            }
            
            Debug.LogWarning($"Component type '{componentName}' not found.");
            return null;
        }
        
        public void AddCollider(GameObject target)
        {
            if (target == null || currentEnemyData == null) return;
            
            Type typeToAdd = GetColliderType(currentEnemyData.colliderType);
            if (typeToAdd != null)
            {
                target.AddComponent(typeToAdd);
            }
            else
            {
                Debug.LogError("Invalid Collider2DType or type not supported.");
                // Fallback to BoxCollider2D
                target.AddComponent<BoxCollider2D>();
            }
        }
        
        private Type GetColliderType(Enums.Collider2DType type)
        {
            switch (type)
            {
                case Enums.Collider2DType.BoxCollider2D:
                    return typeof(BoxCollider2D);
                case Enums.Collider2DType.CircleCollider2D:
                    return typeof(CircleCollider2D);
                case Enums.Collider2DType.CapsuleCollider2D:
                    return typeof(CapsuleCollider2D);
                case Enums.Collider2DType.PolygonCollider2D:
                    return typeof(PolygonCollider2D);
                default:
                    return typeof(BoxCollider2D); // Safe fallback
            }
        }

        private void SetColliderSizeOnSprite(GameObject target)
        {
            if (target == null) return;
            
            // Get the sprite renderer from the sprite child
            Transform spriteChild = target.transform.Find("Sprite");
            if (spriteChild == null) return;

            SpriteRenderer spriteRenderer = spriteChild.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null || spriteRenderer.sprite == null) return;

            // Get the main collider on the parent
            Collider2D mainCollider = target.GetComponent<Collider2D>();
            if (mainCollider == null) return;

            // Get sprite bounds
            Bounds spriteBounds = spriteRenderer.bounds;
            Vector2 spriteSize = new Vector2(spriteBounds.size.x, spriteBounds.size.y);
            
            // Apply a slight padding (90% of sprite size for better gameplay feel)
            float paddingFactor = 0.9f;
            spriteSize *= paddingFactor;

            // Size the collider based on its type
            switch (mainCollider)
            {
                case BoxCollider2D boxCollider:
                    boxCollider.size = spriteSize;
                    break;
                    
                case CircleCollider2D circleCollider:
                    // Use the smaller dimension to ensure the circle fits within the sprite
                    circleCollider.radius = Mathf.Min(spriteSize.x, spriteSize.y) * 0.5f;
                    break;
                    
                case CapsuleCollider2D capsuleCollider:
                    capsuleCollider.size = spriteSize;
                    // Set direction based on sprite aspect ratio
                    capsuleCollider.direction = spriteSize.x > spriteSize.y ? 
                        CapsuleDirection2D.Horizontal : CapsuleDirection2D.Vertical;
                    break;
                    
                case PolygonCollider2D polygonCollider:
                    // For polygon colliders, we'll use the sprite's physics shape if available
                    try
                    {
                        if (spriteRenderer.sprite.GetPhysicsShapeCount() > 0)
                        {
                            List<Vector2> physicsShape = new List<Vector2>();
                            spriteRenderer.sprite.GetPhysicsShape(0, physicsShape);
                            polygonCollider.points = physicsShape.ToArray();
                        }
                        else
                        {
                            // Fallback: create a simple rectangle
                            Vector2 halfSize = spriteSize * 0.5f;
                            polygonCollider.points = new Vector2[]
                            {
                                new Vector2(-halfSize.x, -halfSize.y),
                                new Vector2(halfSize.x, -halfSize.y),
                                new Vector2(halfSize.x, halfSize.y),
                                new Vector2(-halfSize.x, halfSize.y)
                            };
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning($"Error setting polygon collider points: {e.Message}");
                    }
                    break;
            }

            // Also resize the hitbox collider if it exists
            Transform hitboxChild = target.transform.Find("Hitbox");
            if (hitboxChild != null)
            {
                BoxCollider2D hitboxCollider = hitboxChild.GetComponent<BoxCollider2D>();
                if (hitboxCollider != null)
                {
                    // Make hitbox slightly larger than main collider for better hit detection
                    hitboxCollider.size = spriteSize * 1.2f;
                }
            }

            Debug.Log($"Auto-sized colliders for {target.name} based on sprite bounds: {spriteSize}");
        }
    }
}