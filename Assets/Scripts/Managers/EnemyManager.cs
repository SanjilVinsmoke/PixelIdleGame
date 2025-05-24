using UnityEngine;
using ScriptableObjects;

namespace Managers
{
    public class EnemyManager : MonoBehaviour
    {
        [Header("Registry")]
        public EnemyDataRegistry enemyDataRegistry;
        
        private static EnemyManager _instance;
        public static EnemyManager Instance => _instance;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeRegistry();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeRegistry()
        {
            if (enemyDataRegistry != null)
            {
                enemyDataRegistry.Initialize();
            }
        }

        public EnemyDataSo GetEnemyData(EnemyType enemyType)
        {
            if (enemyDataRegistry == null)
            {
                Debug.LogWarning("Enemy Data Registry is not assigned!");
                return null;
            }

            return enemyDataRegistry.GetData(enemyType);
        }

        public GameObject SpawnEnemy(EnemyType enemyType, Vector3 position, Quaternion rotation = default)
        {
            var enemyData = GetEnemyData(enemyType);
            if (enemyData?.enemyPrefab == null)
            {
                Debug.LogWarning($"No prefab found for enemy type: {enemyType}");
                return null;
            }

            var spawnedEnemy = Instantiate(enemyData.enemyPrefab, position, rotation);
            
            // Ensure the enemy has the correct data assigned
            var enemyComponent = spawnedEnemy.GetComponent<MonoBehaviour>();
            if (enemyComponent != null)
            {
                var enemyDataField = enemyComponent.GetType().GetField("enemyData");
                if (enemyDataField != null)
                {
                    enemyDataField.SetValue(enemyComponent, enemyData);
                }
            }

            return spawnedEnemy;
        }

        public bool IsEnemyTypeAvailable(EnemyType enemyType)
        {
            return enemyDataRegistry?.HasData(enemyType) ?? false;
        }
    }
}
