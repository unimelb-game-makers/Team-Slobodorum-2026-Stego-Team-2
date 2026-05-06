using System.Collections.Generic;
using TeamSlobodorum.Entities.Player;
using TeamSlobodorum.UI.Scripts;
using UnityEngine;

namespace TeamSlobodorum.Dialogue
{
    public class NPCInteractableCollision : NPCInteractable
    {
        [SerializeField] private List<WorldSpaceTracker> trackers;
        private bool onCollision = false;
        
        private void RegisterTrackers()
        {
            foreach (var tracker in trackers)
            {
                tracker.RegisterComponent();
            }
        }
        private void UnregisterTrackers()
        {
            foreach (var tracker in trackers)
            {
                tracker.UnregisterComponent();
            }
        }
        
        private void OnTriggerEnter(Collider other) {
            if (other.CompareTag("Player"))
            {   
                RegisterTrackers();
                onCollision = true;
                other.GetComponent<PlayerDialogueInteraction>()?.interactables.Add(this);
            }
        }

        private void OnTriggerExit(Collider other) {
            if (other.CompareTag("Player"))
            {   
                UnregisterTrackers();
                onCollision = false;
                other.GetComponent<PlayerDialogueInteraction>()?.interactables.Remove(this);

            }
        }
    }
}