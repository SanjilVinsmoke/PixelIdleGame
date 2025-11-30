using System.Collections.Generic;
using UnityEngine;
using Utils;
using Utils.Managers;

namespace Managers
{
    public class MapManager : SingletonMonoBehavior<MapManager>
    {
        // Store visited room IDs
        private HashSet<string> visitedRooms = new HashSet<string>();

        protected override void InitializeSingleton()
        {
            Debug.Log("MapManager Initialized");
        }

        public void RegisterRoomVisit(string roomId)
        {
            if (!visitedRooms.Contains(roomId))
            {
                visitedRooms.Add(roomId);
                Debug.Log($"New Room Visited: {roomId}");
                // Notify UI to update map
                // OnMapUpdated?.Invoke();
            }
        }

        public bool IsRoomVisited(string roomId)
        {
            return visitedRooms.Contains(roomId);
        }

        // For Save System
        public List<string> GetVisitedRooms()
        {
            return new List<string>(visitedRooms);
        }

        public void LoadVisitedRooms(List<string> rooms)
        {
            visitedRooms = new HashSet<string>(rooms);
        }
    }
}

