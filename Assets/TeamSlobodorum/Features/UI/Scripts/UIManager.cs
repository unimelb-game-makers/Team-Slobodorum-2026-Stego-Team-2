using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEngine.Events;
using TeamSlobodorum.Dialogue;

namespace TeamSlobodorum.UI.Scripts
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("UI Input")]
        public InputActionReference openMenuAction;
        public InputActionReference closeMenuAction;
        
        public UnityEvent OnMenuOpened;
        public UnityEvent OnMenuClosed;
        public UnityEvent OnGameOver;
        public UnityEvent OnGameWin;

        public UnityEvent OnDialogueOpened;
        public UnityEvent OnDialogueClosed;
        public bool IsMenuOpen { get; private set; } = false;

        public WorldSpaceUIController worldSpaceUIController;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            openMenuAction.action.performed += OpenMenu;
            closeMenuAction.action.performed += CloseMenu;
            DialogueManager.Instance.OnDialogueStarted +=  OnDialogueOpened.Invoke;
            DialogueManager.Instance.OnDialogueEnded += OnDialogueClosed.Invoke;
        }

        public void OpenMenu(InputAction.CallbackContext context)
        {
            if (IsMenuOpen) return;

            IsMenuOpen = true;


            OnMenuOpened?.Invoke();
        }

        public void CloseMenu(InputAction.CallbackContext context)
        {
            if (!IsMenuOpen) return;

            IsMenuOpen = false;

            OnMenuClosed?.Invoke();
        }

        public void OnDestroy()
        {
            openMenuAction.action.performed -= OpenMenu;
            closeMenuAction.action.performed -= CloseMenu;
            DialogueManager.Instance.OnDialogueStarted -= OnDialogueOpened.Invoke;
        }

    }
}