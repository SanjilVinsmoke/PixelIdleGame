using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Utils;
using Utils.Managers;

namespace Managers
{
    [Serializable]
    public class GameSaveData
    {
        public Vector3 playerPosition;
        public List<PlayerAbility> unlockedAbilities;
        public List<string> visitedRooms;
        // Add more fields as needed: currentHealth, currentScene, etc.
        
        public GameSaveData()
        {
            unlockedAbilities = new List<PlayerAbility>();
            visitedRooms = new List<string>();
            playerPosition = Vector3.zero;
        }
    }

    public class SaveManager : SingletonMonoBehavior<SaveManager>
    {
        private string saveFilePath;
        private const string SAVE_FILE_NAME = "savegame.json";

        protected override void InitializeSingleton()
        {
            saveFilePath = Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);
            Debug.Log($"SaveManager Initialized. Path: {saveFilePath}");
        }

        public void SaveGame()
        {
            GameSaveData data = new GameSaveData();

            // 1. Save Abilities
            if (PlayerAbilityManager.Instance != null)
            {
                data.unlockedAbilities = PlayerAbilityManager.Instance.GetUnlockedAbilities();
            }

            // 2. Save Map Progress
            if (MapManager.Instance != null)
            {
                data.visitedRooms = MapManager.Instance.GetVisitedRooms();
            }

            // 3. Save Player Position
            Player player = FindObjectOfType<Player>();
            if (player != null)
            {
                data.playerPosition = player.transform.position;
            }

            // Serialize to JSON
            try
            {
                string json = JsonUtility.ToJson(data, true);
                File.WriteAllText(saveFilePath, json);
                Debug.Log("Game Saved Successfully.");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save game: {e.Message}");
            }
        }

        public void LoadGame()
        {
            if (!File.Exists(saveFilePath))
            {
                Debug.LogWarning("No save file found.");
                return;
            }

            try
            {
                string json = File.ReadAllText(saveFilePath);
                GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);

                // 1. Load Abilities
                if (PlayerAbilityManager.Instance != null)
                {
                    PlayerAbilityManager.Instance.LoadAbilities(data.unlockedAbilities);
                }

                // 2. Load Map Progress
                if (MapManager.Instance != null)
                {
                    MapManager.Instance.LoadVisitedRooms(data.visitedRooms);
                }

                // 3. Load Player Position
                Player player = FindObjectOfType<Player>();
                if (player != null)
                {
                     // For MVP, direct assignment
                     player.transform.position = data.playerPosition;
                }

                Debug.Log("Game Loaded Successfully.");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load game: {e.Message}");
            }
        }
        
        public bool HasSaveFile()
        {
            return File.Exists(saveFilePath);
        }
        
        public void DeleteSaveFile()
        {
             if (File.Exists(saveFilePath))
             {
                 File.Delete(saveFilePath);
                 Debug.Log("Save file deleted.");
             }
        }
    }
}
