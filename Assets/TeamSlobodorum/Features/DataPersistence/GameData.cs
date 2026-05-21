

using System.Collections.Generic;
using TeamSlobodorum.Spells.Core;
using UnityEngine;

namespace TeamSlobodorum.DataPersistence
{
    [System.Serializable]
    public class GameData
    {
        public string currentLevel = "Level_01";
        public Vector3 playerPosition = Vector3.zero;
        public Vector3 SpawnPoint = Vector3.zero;
        public List<SpellSaveData> spells = new List<SpellSaveData>();
    }

    public interface IDataPersistence
    {
        void LoadData(GameData data);
        void SaveData(GameData data);
    }

    [System.Serializable]
    public struct SpellSaveData 
    {
        public SpellId spell;
        public bool isCollected;
    }
}
