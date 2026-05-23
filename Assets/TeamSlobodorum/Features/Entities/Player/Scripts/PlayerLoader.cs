using TeamSlobodorum.DataPersistence;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TeamSlobodorum.Entities.Player
{
    public class PlayerLoader : MonoBehaviour, IDataPersistence
    {
        void Awake()
        {
            if (SaveManager.instance != null)
            {

                SaveManager.instance.OnSaveRequested += SaveData;
                SaveManager.instance.OnLoadRequested += LoadData;
            }
        }

        public void LoadData(GameData data)
        {   
            if (data.hasSavedPosition)
            {
                gameObject.transform.position = data.playerPosition;
            }
        }

        public void SaveData(GameData data)
        {
            data.playerPosition = gameObject.transform.position;
            data.hasSavedPosition = true;
        }

        public void OnDestroy()
        {
            if (SaveManager.instance != null)
            {
                SaveManager.instance.OnSaveRequested -= SaveData;
                SaveManager.instance.OnLoadRequested -= LoadData;
            }

        }

    }
}