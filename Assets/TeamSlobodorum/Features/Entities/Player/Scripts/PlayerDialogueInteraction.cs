using System.Collections.Generic;
using TeamSlobodorum.Dialogue;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TeamSlobodorum.Entities.Player
{
    public class PlayerDialogueInteraction : MonoBehaviour
    {
        public InputActionReference interactAction;
        [HideInInspector] public List<NPCInteractable> interactables = new List<NPCInteractable>();

        public void Start()
        {
            interactAction.action.performed += TryInitiateDialogue;
        }
        private void TryInitiateDialogue(InputAction.CallbackContext context)
        {
            if (interactables.Count != 0 ) {
                interactables[0].OnInteract();
            }
        }

        public void OnDestroy()
        {
            interactAction.action.performed -= TryInitiateDialogue;

        }
    }
}
