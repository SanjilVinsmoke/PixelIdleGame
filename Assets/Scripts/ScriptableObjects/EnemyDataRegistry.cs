using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Enemy Data Registry", menuName = "Enemy System/Enemy Data Registry")]
    public class EnemyDataRegistry : ScriptableObject
    {
        [Tooltip("Drag in all of your EnemyDataSo assets here")]
        public List<EnemyDataSo> allEnemyData = new List<EnemyDataSo>();

        // Quick lookup by enum:
        private Dictionary<EnemyType, EnemyDataSo> _lookup;

        public void Initialize()
        {
            _lookup = new Dictionary<EnemyType, EnemyDataSo>(allEnemyData.Count);
            foreach (var data in allEnemyData)
            {
                if (data != null)
                {
                    _lookup[data.enemyType] = data;
                }
            }
        }

        public EnemyDataSo GetData(EnemyType type)
        {
            if (_lookup == null) Initialize();
            return _lookup.TryGetValue(type, out var data) ? data : null;
        }

        public List<EnemyDataSo> GetAllData()
        {
            return new List<EnemyDataSo>(allEnemyData);
        }

        public bool HasData(EnemyType type)
        {
            if (_lookup == null) Initialize();
            return _lookup.ContainsKey(type);
        }

        public void RefreshRegistry()
        {
            _lookup?.Clear();
            Initialize();
        }

        private void OnValidate()
        {
            // Auto-refresh when data changes in editor
            if (_lookup != null)
            {
                RefreshRegistry();
            }
        }
    }
}
