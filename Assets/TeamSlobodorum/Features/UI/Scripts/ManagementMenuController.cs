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

        private void Start()
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.OnMenuOpened += ShowUI;
                UIManager.Instance.OnMenuClosed += HideUI;
            }
        }

        private void OnDestroy()
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.OnMenuOpened -= ShowUI;
                UIManager.Instance.OnMenuClosed -= HideUI;
            }
        }

        private void HideUI()
        {
            root.style.display = DisplayStyle.None;
        }

        private void ShowUI()
        {
            root.style.display = DisplayStyle.Flex;
        }
    }
}