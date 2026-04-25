using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using System;
namespace TeamSlobodorum.UI.Scripts
{
    public class ManagementMenuController : MonoBehaviour
    {
        private UIDocument _uiDocument;
        private VisualElement root;
        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();

            root = _uiDocument.rootVisualElement;

            if (root != null) root.style.display = DisplayStyle.None;

        }

        public void HideUI()
        {
            root.style.display = DisplayStyle.None;
        }

        public void ShowUI()
        {
            root.style.display = DisplayStyle.Flex;
        }
    }
}