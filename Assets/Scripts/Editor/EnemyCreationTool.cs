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
                .ToList();
            
            templateNames = availableTemplates.Select(t => t.templateName).ToArray();
        }
        
        private void OnGUI()
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
                DrawCreationButtons();
            }
            
            EditorGUILayout.EndScrollView();
        }
        
        private void DrawTemplateSelection()
        {
            GUILayout.Label("Template Selection", EditorStyles.boldLabel);
            
            if (templateNames.Length > 0)
            {
                selectedTemplateIndex = EditorGUILayout.Popup("Enemy Template", selectedTemplateIndex, templateNames);
                selectedTemplate = availableTemplates[selectedTemplateIndex];
                
                if (selectedTemplate.icon != null)
                {
                    GUILayout.Label(selectedTemplate.icon.texture, GUILayout.Width(64), GUILayout.Height(64));
                }
                
                EditorGUILayout.LabelField("Description", selectedTemplate.description, EditorStyles.wordWrappedLabel);
            }
            else
            {
                EditorGUILayout.HelpBox("No enemy templates found. Create an EnemyTemplate asset first.", MessageType.Warning);
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
                
                EditorGUILayout.PropertyField(serializedData.FindProperty("enemyName"));
                EditorGUILayout.PropertyField(serializedData.FindProperty("maxHealth"));
                EditorGUILayout.PropertyField(serializedData.FindProperty("moveSpeed"));
                EditorGUILayout.PropertyField(serializedData.FindProperty("attackDamage"));
                EditorGUILayout.PropertyField(serializedData.FindProperty("attackRange"));
                EditorGUILayout.PropertyField(serializedData.FindProperty("detectionRange"));
                
                showAdvancedSettings = EditorGUILayout.Foldout(showAdvancedSettings, "Advanced Settings");
                if (showAdvancedSettings)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(serializedData.FindProperty("animatorController"));
                    EditorGUILayout.PropertyField(serializedData.FindProperty("animationSpeed"));
                    EditorGUILayout.PropertyField(serializedData.FindProperty("rotationSpeed"));
                    EditorGUILayout.PropertyField(serializedData.FindProperty("attackCooldown"));
                    EditorGUILayout.PropertyField(serializedData.FindProperty("regenerateHealth"));
                    if (serializedData.FindProperty("regenerateHealth").boolValue)
                    {
                        EditorGUILayout.PropertyField(serializedData.FindProperty("healthRegenRate"));
                    }
                    EditorGUI.indentLevel--;
                }
                
                serializedData.ApplyModifiedProperties();
            }
        }
        
        private void DrawStateConfiguration()
        {
            if (currentEnemyData == null) return;
            
            GUILayout.Label("State Machine Configuration", EditorStyles.boldLabel);
            
            SerializedObject serializedData = new SerializedObject(currentEnemyData);
            serializedData.Update();
            
            EditorGUILayout.PropertyField(serializedData.FindProperty("defaultState"));
            EditorGUILayout.PropertyField(serializedData.FindProperty("availableStates"), true);
            
            if (GUILayout.Button("Reset to Template States"))
            {
                ResetToTemplateStates();
            }
            
            serializedData.ApplyModifiedProperties();
        }
        
        private void DrawCreationButtons()
        {
            GUILayout.Label("Actions", EditorStyles.boldLabel);
            
            selectedEnemyPrefab = EditorGUILayout.ObjectField("Target Prefab", selectedEnemyPrefab, typeof(GameObject), false) as GameObject;
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Prefab Creation", EditorStyles.boldLabel);
            
            if (GUILayout.Button("Create New Enemy Prefab"))
            {
                CreateEnemyPrefab();
            }
            
            if (selectedEnemyPrefab != null && GUILayout.Button("Apply Data to Selected Prefab"))
            {
                ApplyDataToPrefab();
            }
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Scene Creation", EditorStyles.boldLabel);
            
            if (GUILayout.Button("Create Enemy in Scene"))
            {
                CreateEnemyInScene();
            }
            
            if (currentEnemyData?.enemyPrefab != null && GUILayout.Button("Instantiate from Prefab"))
            {
                var instance = PrefabUtility.InstantiatePrefab(currentEnemyData.enemyPrefab) as GameObject;
                Selection.activeGameObject = instance;
            }
            
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Basic enemy structure includes:\n• Rigidbody2D and Collider2D on parent\n• Sprite child with SpriteRenderer\n• Hitbox child with trigger collider", MessageType.Info);
        }
        
        private void CreateNewEnemyData()
        {
            currentEnemyData = CreateInstance<EnemyDataSo>();
            currentEnemyData.name = $"New {selectedTemplate.templateName} Data";
            
            // Copy default values from template
            if (selectedTemplate.defaultData != null)
            {
                EditorUtility.CopySerialized(selectedTemplate.defaultData, currentEnemyData);
            }
            
            // Set default states
            currentEnemyData.availableStates = new List<EnemyStateData>(selectedTemplate.defaultStates);
            
            string path = EditorUtility.SaveFilePanelInProject(
                "Save Enemy Data",
                currentEnemyData.name,
                "asset",
                "Save enemy data asset");
            
            if (!string.IsNullOrEmpty(path))
            {
                AssetDatabase.CreateAsset(currentEnemyData, path);
                AssetDatabase.SaveAssets();
            }
        }
        
        private void ResetToTemplateStates()
        {
            if (currentEnemyData != null && selectedTemplate != null)
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
                new GameObject(currentEnemyData.enemyName);
            
            ApplyDataToGameObject(prefab);
            
            string path = EditorUtility.SaveFilePanelInProject(
                "Save Enemy Prefab",
                currentEnemyData.enemyName,
                "prefab",
                "Save enemy prefab");
            
            if (!string.IsNullOrEmpty(path))
            {
                PrefabUtility.SaveAsPrefabAsset(prefab, path);
                currentEnemyData.enemyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                EditorUtility.SetDirty(currentEnemyData);
            }
            
            DestroyImmediate(prefab);
        }
        
        private void ApplyDataToPrefab()
        {
            if (selectedEnemyPrefab == null || currentEnemyData == null) return;
            
            GameObject instance = PrefabUtility.LoadPrefabContents(AssetDatabase.GetAssetPath(selectedEnemyPrefab));
            ApplyDataToGameObject(instance);
            PrefabUtility.SaveAsPrefabAsset(instance, AssetDatabase.GetAssetPath(selectedEnemyPrefab));
            PrefabUtility.UnloadPrefabContents(instance);
        }
        
        private void CreateEnemyInScene()
        {
            if (currentEnemyData == null) return;
            
            GameObject enemy = currentEnemyData.enemyPrefab != null ? 
                Instantiate(currentEnemyData.enemyPrefab) : 
                new GameObject(currentEnemyData.enemyName);
            
            ApplyDataToGameObject(enemy);
            Selection.activeGameObject = enemy;
        }
        
        private void ApplyDataToGameObject(GameObject target)
        {
            // Ensure basic enemy structure exists
            SetupBasicEnemyStructure(target);
            
            // Add enemy type-specific script
            AttachEnemyTypeScript(target);
            
            // Add required components if missing
            foreach (string componentName in selectedTemplate.requiredComponents)
            {
                System.Type componentType = GetComponentType(componentName);
                if (componentType != null && target.GetComponent(componentType) == null)
                {
                    target.AddComponent(componentType);
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
                .FirstOrDefault(c => c.GetType().BaseType?.IsGenericType == true && 
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
            // Search for enemy scripts in all assemblies
            foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetTypes().FirstOrDefault(t => 
                    t.Name == scriptName && 
                    t.BaseType?.IsGenericType == true &&
                    t.BaseType.GetGenericTypeDefinition() == typeof(Enemy<>));
                
                if (type != null) return type;
            }
            
            return null;
        }

        private void ApplyEnemyDataToComponents(GameObject target)
        {
            var enemyComponents = target.GetComponents<MonoBehaviour>()
                .Where(c => c.GetType().BaseType?.IsGenericType == true && 
                           c.GetType().BaseType.GetGenericTypeDefinition() == typeof(Enemy<>));
            
            foreach (var enemy in enemyComponents)
            {
                SerializedObject serializedEnemy = new SerializedObject(enemy);
                var enemyDataProperty = serializedEnemy.FindProperty("enemyData");
                if (enemyDataProperty != null)
                {
                    enemyDataProperty.objectReferenceValue = currentEnemyData;
                    serializedEnemy.ApplyModifiedProperties();
                }
            }
        }

        private void SetupBasicEnemyStructure(GameObject target)
        {
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
                var collider = target.GetComponent<Collider2D>();
               
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
                spriteRenderer.sortingLayerName = "Enemies";
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
        }

        private System.Type GetComponentType(string componentName)
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
            
            Debug.LogWarning($"Component type '{componentName}' not found.");
            return null;
        }
        
        public void AddCollider(GameObject target)
        {
            Type typeToAdd = GetColliderType(currentEnemyData.colliderType);
            if (typeToAdd != null)
            {
                target.AddComponent(typeToAdd);
            }
            else
            {
                Debug.LogError("Invalid Collider2DType or type not supported.");
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
                    return null;
            }
        }
    }
   
    
    
}
