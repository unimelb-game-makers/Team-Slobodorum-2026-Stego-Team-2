using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using System;
using Cursor = UnityEngine.Cursor;

namespace TeamSlobodorum.UI.Scripts
{
    public class ManagementMenuController : MonoBehaviour
    {
        private UIDocument _uiDocument;
        private VisualElement root;
        public InputActionAsset actions;

        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();

            root = _uiDocument.rootVisualElement;

            if (root != null) root.style.display = DisplayStyle.None;

        }
        private void Start()
        {

        
        }
        public void HideUI()
        {
            root.style.display = DisplayStyle.None;
            actions.FindActionMap("UI")?.Disable();
            actions.FindActionMap("Player")?.Enable();

        }

        public void ShowUI()
        {
            root.style.display = DisplayStyle.Flex;
            Cursor.lockState = CursorLockMode.None;
            actions.FindActionMap("Player")?.Disable();
            actions.FindActionMap("UI")?.Enable();

        }
    }
}