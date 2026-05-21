using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Collections;
using System;
namespace TeamSlobodorum.DataPersistence
{
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager instance { get; private set; }

        [Header("File Storage Config")]
        [SerializeField] private string fileName = "data.game";

        private GameData gameData;
        private FileDataHandler dataHandler;
        public static event Action<GameData> OnSaveRequested;
        public static event Action<GameData> OnLoadRequested;

        private void Awake()
        {
            if (instance != null) { Destroy(gameObject); return; }
            instance = this;
            DontDestroyOnLoad(gameObject);

            dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        }

        public void NewGame() => gameData = new GameData();

        public void LoadGame()
        {
            gameData = dataHandler.Load() ?? new GameData();
            OnLoadRequested?.Invoke(gameData);
        }

        public void SaveGame()
        {
            if (gameData == null) return;

            OnSaveRequested?.Invoke(gameData);
            gameData.currentLevel = SceneManager.GetActiveScene().name;
            dataHandler.Save(gameData);
        }

        public void LoadLevelAsync(string sceneName)
        {
            SaveGame(); 
            StartCoroutine(LoadLevelCoroutine(sceneName));
        }

        private IEnumerator LoadLevelCoroutine(string sceneName)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            LoadGame();
        }

        private void OnApplicationQuit() => SaveGame();
    }
}

