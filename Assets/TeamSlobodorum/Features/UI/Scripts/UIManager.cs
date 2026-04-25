using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using System;
using UnityEngine.Events;
namespace TeamSlobodorum.UI.Scripts
{

    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("UI Input")]
        public InputActionReference toggleMenuAction;

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



            if (toggleMenuAction != null)
            {
                toggleMenuAction.action.performed += HandleToggleInput;
                toggleMenuAction.action.Enable();
            }
        }

        private void OnDisable()
        {
            if (toggleMenuAction != null)
            {
                toggleMenuAction.action.performed -= HandleToggleInput;
                toggleMenuAction.action.Disable();
            }
        }

        private void HandleToggleInput(InputAction.CallbackContext context)
        {
            ToggleMenu();
        }


        // --- PUBLIC API ---
        public void ToggleMenu()
        {
            if (IsMenuOpen) CloseMenu();
            else OpenMenu();
        }

        public void OpenMenu()
        {
            if (IsMenuOpen) return;

            IsMenuOpen = true;

            OnMenuOpened?.Invoke();
        }

        public void CloseMenu()
        {
            if (!IsMenuOpen) return;

            IsMenuOpen = false;

            OnMenuClosed?.Invoke();
        }
    }
}