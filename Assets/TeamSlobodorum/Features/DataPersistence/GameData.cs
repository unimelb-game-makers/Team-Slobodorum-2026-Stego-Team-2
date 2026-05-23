

using System.Collections.Generic;
using TeamSlobodorum.Spells.Core;
using UnityEngine;

namespace TeamSlobodorum.DataPersistence
{
    [System.Serializable]
    public class GameData
    {
        public string currentLevel = "Level1";
        public Vector3 playerPosition = Vector3.zero;
        public bool hasSavedPosition = false;
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
        public string spellID;
        public bool isCollected;
        public bool isEquipped;
    }

    [System.Serializable]
    public class SaveSlotMeta
    {
        public string profileId;
        public string currentLevel;
        public float playTimeSeconds;
        public string lastPlayedDate;
        
    }

    [System.Serializable]
    public class SaveManifest
    {
        public List<SaveSlotMeta> slots = new List<SaveSlotMeta>();
    }
}
