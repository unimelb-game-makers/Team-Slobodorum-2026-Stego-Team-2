using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEngine.Events;

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

        public bool IsMenuOpen { get; private set; } = false;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void OnEnable()
        {   
            openMenuAction.action.performed+=OpenMenu;
            closeMenuAction.action.performed+=CloseMenu;
        }

        private void OnDisable()
        {
            openMenuAction.action.performed-=OpenMenu;
            closeMenuAction.action.performed-=CloseMenu;

            var playerMap = InputSystem.actions.FindActionMap("Player");
            if (playerMap != null && !playerMap.enabled)
            {
                InputSystem.actions.FindActionMap("UI")?.Disable();
                playerMap.Enable();
            }
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
    }
}