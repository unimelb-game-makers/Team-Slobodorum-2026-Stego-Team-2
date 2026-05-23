using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Collections;
using System;
using System.IO;
namespace TeamSlobodorum.DataPersistence
{
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager instance { get; private set; }

        [Header("File Storage Config")]
        private string fileName = ".save";
        private string selectedProfileId = "Slot_1";
        private GameData gameData;

        public GameData GameData => gameData;
        private FileDataHandler dataHandler;
        public event Action<GameData> OnSaveRequested;
        public event Action<GameData> OnLoadRequested;
        public event Action OnManifestSlotUpdate;

        [Header("Manifest Config")]
        private string manifestFileName = "manifest.json";
        private FileDataHandler manifestHandler;
        private SaveManifest currentManifest;
        public SaveManifest CurrentManifest => currentManifest;


        private void Awake()
        {
            if (instance != null) { Destroy(gameObject); return; }
            instance = this;
            DontDestroyOnLoad(gameObject);
            manifestHandler = new FileDataHandler(Application.persistentDataPath, manifestFileName);
            LoadManifest();
            UpdateDataHandler();
        }

        public void NewGame()
        {
            
            gameData = new GameData();
            dataHandler.Save(gameData);
            GenerateNewProfileId();
            UpdateManifestSlot();
        }
        public void AutoSave()
        {
            if (gameData == null) return;

            string previousProfile = selectedProfileId;

            ChangeProfile("AutoSave");
            SaveGame();

            ChangeProfile(previousProfile);
        }
        public bool TryLoadMostRecentSave()
        {
            if (currentManifest == null || currentManifest.slots.Count == 0)
            {
                Debug.LogWarning("No save data found.");
                return false;
            }

            SaveSlotMeta mostRecentSlot = currentManifest.slots
                .OrderByDescending(slot => slot.lastPlayedDate)
                .First();

            ChangeProfile(mostRecentSlot.profileId);
            LoadSavedLevel();

            return true;
        }


        public void ChangeProfile(string newProfileId)
        {
            selectedProfileId = newProfileId;
            UpdateDataHandler();
        }
        private void UpdateDataHandler()
        {
            string profilePath = Path.Combine(Application.persistentDataPath, selectedProfileId);
            dataHandler = new FileDataHandler(profilePath, selectedProfileId + fileName);
        }

        private void LoadManifest()
        {
            currentManifest = manifestHandler.Load<SaveManifest>() ?? new SaveManifest();
        }

        private void UpdateManifestSlot()
        {
            SaveSlotMeta slotMeta = currentManifest.slots.FirstOrDefault(s => s.profileId == selectedProfileId);

            if (slotMeta == null)
            {
                slotMeta = new SaveSlotMeta { profileId = selectedProfileId };
                currentManifest.slots.Add(slotMeta);
            }

            slotMeta.currentLevel = gameData.currentLevel;
            slotMeta.lastPlayedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            manifestHandler.Save(currentManifest);
            OnManifestSlotUpdate?.Invoke();
        }

        public void LoadGame()
        {
            gameData = dataHandler.Load<GameData>() ?? new GameData();
            LoadLevelAsync(gameData.currentLevel);
        }

        public void SaveGame()
        {
            if (gameData == null) return;

            OnSaveRequested?.Invoke(gameData);
            gameData.currentLevel = SceneManager.GetActiveScene().name;
            dataHandler.Save(gameData);
            UpdateManifestSlot();
        }

        public void LoadSavedLevel()
        {
            gameData = dataHandler.Load<GameData>() ?? new GameData();
            LoadLevelAsync(gameData.currentLevel);
        }
        public void LoadLevelAsync(string sceneName)
        {
            StartCoroutine(LoadLevelCoroutine(sceneName));
        }
        
        private IEnumerator LoadLevelCoroutine(string sceneName)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            LoadLevel();
        }
        private void LoadLevel()
        {
            gameData = dataHandler.Load<GameData>() ?? new GameData();
            OnLoadRequested?.Invoke(gameData);
        }



        public void GenerateNewProfileId()
        {
            int highestSlot = 0;

            if (currentManifest != null)
            {
                foreach (var slot in currentManifest.slots)
                {
                    if (slot.profileId.StartsWith("Slot_"))
                    {
                        if (int.TryParse(slot.profileId.Replace("Slot_", ""), out int slotNum))
                        {
                            if (slotNum > highestSlot)
                            {
                                highestSlot = slotNum;
                            }
                        }
                    }
                }
            }

            string newProfileId = $"Slot_{highestSlot + 1}";
            ChangeProfile(newProfileId);
            
        }


        public List<SaveSlotMeta> GetSlotsSortedByNewest()
        {
            if (currentManifest == null) return null;

            List<SaveSlotMeta> sortedList = currentManifest.slots
                .OrderByDescending(slot => slot.lastPlayedDate)
                .ToList();

            return sortedList;
        }
    }


}

