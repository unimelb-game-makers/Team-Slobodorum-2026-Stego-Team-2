using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using System;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Input")]
    public InputActionReference toggleMenuAction;

    [Header("UI Documents")]
    public UIDocument ManagementMenuUI;
    public UIDocument HUD;

    public event Action OnMenuOpened;
    public event Action OnMenuClosed;

    private VisualElement menuContainer;
    private VisualElement hudRoot;
    
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
        if (ManagementMenuUI != null && ManagementMenuUI.rootVisualElement != null)
        {
            menuContainer = ManagementMenuUI.rootVisualElement;
            if (menuContainer != null) menuContainer.style.display = DisplayStyle.None;
        }

        if (HUD != null && HUD.rootVisualElement != null)
        {
            hudRoot = HUD.rootVisualElement;
        }

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

        if (menuContainer != null) menuContainer.style.display = DisplayStyle.Flex;
        if (hudRoot != null) hudRoot.style.display = DisplayStyle.None;

        OnMenuOpened?.Invoke();
    }

    public void CloseMenu()
    {
        if (!IsMenuOpen) return; 
        
        IsMenuOpen = false;

        if (menuContainer != null) menuContainer.style.display = DisplayStyle.None;
        if (hudRoot != null) hudRoot.style.display = DisplayStyle.Flex;

        OnMenuClosed?.Invoke();
    }
}