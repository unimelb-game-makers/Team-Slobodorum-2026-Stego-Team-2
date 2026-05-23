using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Serialization;
using Cursor = UnityEngine.Cursor;

namespace TeamSlobodorum.UI.Scripts
{
    public class MainMenuController : MonoBehaviour
    {
        [FormerlySerializedAs("actions")] public InputActionAsset playerActions;
        [SerializeField] private Texture2D inactiveTabBackground;
        [SerializeField] private Texture2D activeTabBackground;
        
        private UIDocument _uiDocument;
        private VisualElement _root;
        
        private List<VisualElement> _tabs;
        private List<VisualElement> _pages;

        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();

            _root = _uiDocument.rootVisualElement;

            if (_root != null) _root.style.display = DisplayStyle.None;
            _tabs = _root.Q<VisualElement>("MainMenuTabs").Children().ToList();
            _pages = _root.Q<VisualElement>("MainMenuPages").Children().ToList();

            for (var i = 0; i < _tabs.Count; i++)
            {
                var index = i;
                _tabs[i].RegisterCallback<ClickEvent>(evt => SwitchToTab(index));
            }
        }

        private void SwitchToTab(int index)
        {
            for (var i = 0; i < _tabs.Count; i++)
            {
                if (i == index)
                {
                    _tabs[i].EnableInClassList("active", true);
                    _tabs[i].style.backgroundImage = new StyleBackground(activeTabBackground);
                    _pages[i].style.display = DisplayStyle.Flex;
                }
                else
                {
                    _tabs[i].EnableInClassList("active", false);
                    _tabs[i].style.backgroundImage = new StyleBackground(inactiveTabBackground);
                    _pages[i].style.display = DisplayStyle.None;
                }
            }
        }

        public void HideUI()
        {
            _root.style.display = DisplayStyle.None;
            playerActions.FindActionMap("UI")?.Disable();
            playerActions.FindActionMap("Player")?.Enable();
            Cursor.lockState = CursorLockMode.Locked;
        }

        public void ShowUI()
        {
            _root.style.display = DisplayStyle.Flex;
            Cursor.lockState = CursorLockMode.None;
            playerActions.FindActionMap("Player")?.Disable();
            playerActions.FindActionMap("UI")?.Enable();
            SwitchToTab(0);
        }
    }
}