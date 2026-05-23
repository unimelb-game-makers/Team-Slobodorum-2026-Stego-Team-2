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
        private VisualElement spellComponent;
        private VisualElement systemComponent;
        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();

            root = _uiDocument.rootVisualElement;

            if (root != null) root.style.display = DisplayStyle.None;
            Label spellTitle = root.Q<Label>("SpellTitle");
            Label systemTitle = root.Q<Label>("SystemTitle");
            spellComponent = root.Q<VisualElement>("SpellManagementComponent");
            systemComponent = root.Q<VisualElement>("SystemComponent");

            spellTitle.RegisterCallback<ClickEvent>(evt => SwitchToTab(spellComponent));
            systemTitle.RegisterCallback<ClickEvent>(evt => SwitchToTab(systemComponent));
        }

        private void SwitchToTab(VisualElement activeComponent)
        {
            spellComponent.style.display = DisplayStyle.None;
            systemComponent.style.display = DisplayStyle.None;

            activeComponent.style.display = DisplayStyle.Flex;
        }
        public void HideUI()
        {
            root.style.display = DisplayStyle.None;
            actions.FindActionMap("UI")?.Disable();
            actions.FindActionMap("Player")?.Enable();
            Cursor.lockState = CursorLockMode.Locked;


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