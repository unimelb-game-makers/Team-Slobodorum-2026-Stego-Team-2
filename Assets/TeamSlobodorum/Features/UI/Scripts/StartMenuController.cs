using System.Collections.Generic;
using TeamSlobodorum.DataPersistence;
using TeamSlobodorum.Entities.Player;
using TeamSlobodorum.Spells.Core;
using TeamSlobodorum.Spells.Player;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

namespace TeamSlobodorum.UI.Scripts
{
    public class StartMenuController : MonoBehaviour
    {
        [SerializeField] private UIDocument _uiDocument;
        private VisualElement root;
        void Start()
        {   
            root = _uiDocument.rootVisualElement;
            root.Q<Button>("Start").clicked += StartGame;
            root.Q<Button>("Continue").clicked += ContinueGame;
            root.Q<Button>("Quit").clicked += QuitGame;

        }


        public void StartGame()
        {
        
            SaveManager.instance.NewGame();
            SaveManager.instance.LoadLevelAsync("Level1");
        }

        public void ContinueGame()
        {   
            //start game if no save detected
            if (!SaveManager.instance.TryLoadMostRecentSave())
            {
                StartGame();
            }
            

        }


        public void QuitGame()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif        
        }


    }
}   