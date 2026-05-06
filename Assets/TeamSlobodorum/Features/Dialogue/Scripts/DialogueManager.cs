using UnityEngine;
using Ink.Runtime;
using System;
using System.Collections.Generic;

namespace TeamSlobodorum.Dialogue
{
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance { get; private set; }

        [Header("Story Asset")]
        public TextAsset defaultInkJSON; 

        private Story story;

        public event Action<string, string> OnLineRead; // Sends: (Dialogue Text, Speaker Name)
        public event Action<List<Choice>> OnChoicesReady; // Sends: List of available choices
        public event Action OnDialogueEnded; // Fired when the conversation is over

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void StartDialogue(string knotName = "", TextAsset specificInkJSON = null)
        {
            TextAsset targetJSON = specificInkJSON != null ? specificInkJSON : defaultInkJSON;

            story = new Story(targetJSON.text);

            if (!string.IsNullOrEmpty(knotName))
            {
                story.ChoosePathString(knotName);
            }

            ContinueStory();
        }

        public void ContinueStory()
        {
            if (story.canContinue)
            {
                string rawText = story.Continue();
                string cleanText = rawText.Trim();
                
                string currentSpeaker = "Unknown";

                // Parse the speaker from the prefix (e.g., "Old lady: Hello")
                int colonIndex = cleanText.IndexOf(':');
                if (colonIndex != -1)
                {
                    // Everything before the first colon is the speaker
                    currentSpeaker = cleanText.Substring(0, colonIndex).Trim();
                    
                    // Everything after the first colon is the actual dialogue
                    cleanText = cleanText.Substring(colonIndex + 1).Trim();
                }

                OnLineRead?.Invoke(cleanText, currentSpeaker);

                ParseCommandTags(story.currentTags);
            }
            else if (story.currentChoices.Count > 0)
            {
                OnChoicesReady?.Invoke(story.currentChoices);
            }
            else
            {
                OnDialogueEnded?.Invoke();
            }
        }

        public void SelectChoice(int choiceIndex)
        {
            story.ChooseChoiceIndex(choiceIndex);
            ContinueStory();
        }

        private void ParseCommandTags(List<string> currentTags)
        {
            foreach (string tag in currentTags)
            {
                string[] splitTag = tag.Split(':');
                if (splitTag.Length != 2) continue;

                string tagKey = splitTag[0].Trim().ToLower();
                string tagValue = splitTag[1].Trim();

                switch (tagKey)
                {
                    default:
                        Debug.Log($"{tagKey}: {tagValue}");
                        break;
                }
            }
        }
    }
}