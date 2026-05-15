using TeamSlobodorum.Dialogue;
using UnityEngine;
namespace TeamSlobodorum.Dialogue
{
    public class NPCInteractable : MonoBehaviour
    {
        [Header("Ink Settings")]
        [Tooltip("The exact name of the Knot in the Ink file")]
        public string startingKnotName; 
        
        public void OnInteract()
        {
            DialogueManager.Instance.StartDialogue(startingKnotName);
        }
    }
}