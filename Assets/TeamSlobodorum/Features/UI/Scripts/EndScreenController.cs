using System.Collections.Generic;
using TeamSlobodorum.Entities.Player;
using TeamSlobodorum.Spells.Core;
using TeamSlobodorum.Spells.Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

namespace TeamSlobodorum.UI.Scripts
{
    public class EndScreenController : MonoBehaviour
    {
        [SerializeField] private UIDocument _uiDocument;
        private Label title;
        public InputActionAsset actions;
        private VisualElement root;
        void Start()
        {   
            root = _uiDocument.rootVisualElement;
            root.Q<Button>("Restart").clicked += Restart;
            title = root.Q<Label>("Title");
            root.style.display = DisplayStyle.None;
        }


        public void Restart()
        {

            Time.timeScale = 1;
            SceneManager.LoadScene(0);
        }
        public void GameOver()
        {
            title.text = "GameOver";
            actions.FindActionMap("Player")?.Disable();
            root.style.display = DisplayStyle.Flex;

            Time.timeScale = 0;
        }

        public void Win()
        {
            title.text = "Demo end";
            actions.FindActionMap("Player")?.Disable();
            root.style.display = DisplayStyle.Flex;
            Time.timeScale = 0 ;
        }


    }
}