using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    [Header("UI Toolkit Menu")]
    public UIDocument uiDocument;
    public InputActionReference toggleMenuAction;

    [Header("Team HUD Integration")]
    [Tooltip("Drag your team's existing HUD GameObject here.")]
    public GameObject teamHUD; // Add this line!

    private VisualElement menuContainer;
    private bool isMenuOpen = false;

    private void OnEnable()
    {
        var root = uiDocument.rootVisualElement;
        menuContainer = root.Q<VisualElement>("MenuContainer");
        menuContainer.style.display = DisplayStyle.None;

        toggleMenuAction.action.performed += OnToggleMenuPressed;
        toggleMenuAction.action.Enable();
    }

    private void OnDisable()
    {
        toggleMenuAction.action.performed -= OnToggleMenuPressed;
        toggleMenuAction.action.Disable();
    }

    private void OnToggleMenuPressed(InputAction.CallbackContext context)
    {
        isMenuOpen = !isMenuOpen;

        if (isMenuOpen)
        {
            // Show your Management UI
            menuContainer.style.display = DisplayStyle.Flex;
            
            // Hide the team's HUD
            if (teamHUD != null) teamHUD.SetActive(false); 
        }
        else
        {
            // Hide your Management UI
            menuContainer.style.display = DisplayStyle.None;
            
            // Bring the team's HUD back
            if (teamHUD != null) teamHUD.SetActive(true); 
        }
    }
}