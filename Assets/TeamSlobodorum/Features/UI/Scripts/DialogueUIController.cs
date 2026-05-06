using System;
using System.Collections.Generic;
using Ink.Runtime;
using TeamSlobodorum.Dialogue;
using TeamSlobodorum.UI.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;
namespace TeamSlobodorum.UI.Scripts
{

    public class DialogueUIController : MonoBehaviour
    {
        public class Dialogue
        {
            string speaker;
            string text;
        }
        [SerializeField] private string playerName = "Witch";

        private UIDocument _uiDocument;
        private VisualElement root;
        public InputActionAsset actions;
        public InputActionReference continueAction;
        public InputActionReference scrollUpAction;
        public InputActionReference scrollDownAction;

        private UIDocument uiDoc;
        private Label speakerName;
        private Label dialogueText;
        private VisualElement choicesContainer;
        private VisualElement choicesPanel;
        private Dialogue lastDialogue;
        private List<Choice> lastChoices;
        private int activeChoice = -1;

        private string lastPersonSpeakTo;
        private void Awake()
        {
            uiDoc = GetComponent<UIDocument>();
            root = uiDoc.rootVisualElement;
            choicesPanel = root.Q<VisualElement>("ChoicePanel");
            speakerName = root.Q<Label>("ChoiceSpeakerNameLabel");
            dialogueText = root.Q<Label>("DialogueTextLabel");
            choicesContainer = root.Q<VisualElement>("ChoicesContainer");
            continueAction.action.performed += OnContinueClicked;
            scrollUpAction.action.performed += OnScrollUp;
            scrollDownAction.action.performed += OnScrollDown;
            if (root != null) root.style.display = DisplayStyle.None;

        }

        private void Start()
        {

            DialogueManager.Instance.OnLineRead += HandleLineRead;
            DialogueManager.Instance.OnChoicesReady += HandleChoicesReady;
            actions.FindActionMap("Dialogue").Disable();

        }



        private void HandleLineRead(string text, string speaker)
        {
            dialogueText.text = $"{speaker}: {text}";
            if (speaker != playerName)
            {
                lastPersonSpeakTo = speaker;
            }
            choicesContainer.Clear();
            choicesPanel.style.display = DisplayStyle.None;
            activeChoice = -1;
        }

        private void HandleChoicesReady(List<Choice> choices)
        {
            choicesPanel.style.display = DisplayStyle.Flex;
            lastChoices = choices;
            activeChoice = 0;
            RefreshChoice();
        }


        private void RefreshChoice()
        {
            choicesContainer.Clear();
            speakerName.text = lastPersonSpeakTo;
            for (int i = 0; i < lastChoices.Count; i++)
            {
                int capturedIndex = i; 
                Choice choice = lastChoices[i];

                VisualElement newChoice = new VisualElement();
                Label newChoiceContent = new Label();

                if (capturedIndex == activeChoice)
                {
                    newChoice.AddToClassList("choice-active");
                    newChoiceContent.AddToClassList("choice-text-active");
                }
                else
                {
                    newChoice.AddToClassList("choice-inactive");
                    newChoiceContent.AddToClassList("choice-text-inactive");
                }

                newChoiceContent.text = choice.text;
                newChoice.Add(newChoiceContent);
                newChoice.RegisterCallback<PointerEnterEvent>(evt =>
                {
                    if (activeChoice != capturedIndex)
                    {
                        activeChoice = capturedIndex;
                        RefreshChoice(); 
                    }
                });

                choicesContainer.Add(newChoice);
            }
        }


        private void OnContinueClicked(InputAction.CallbackContext context)
        {   
            if (root.style.display == DisplayStyle.None)
            {
                return;
            }
            if (activeChoice == -1)
            {   
                DialogueManager.Instance.ContinueStory();
            }
            else
            {
                DialogueManager.Instance.SelectChoice(activeChoice);
            }
        }

        private void OnScrollUp(InputAction.CallbackContext context)
        {

            if (activeChoice != -1)
            {

                if (activeChoice > 0)
                {
                    activeChoice -= 1;
                }

            }
            RefreshChoice();

        }

        private void OnScrollDown(InputAction.CallbackContext context)
        {

            if (activeChoice != -1)
            {
                if (activeChoice < lastChoices.Count - 1)
                {
                    activeChoice += 1;
                }
            }
            RefreshChoice();

        }
        public void HideUI()
        {
            root.style.display = DisplayStyle.None;
            actions.FindActionMap("Dialogue")?.Disable();
            actions.FindActionMap("Player")?.Enable();
            Cursor.lockState = CursorLockMode.Locked;


        }

        public void ShowUI()
        {
            root.style.display = DisplayStyle.Flex;
            Cursor.lockState = CursorLockMode.None;
            actions.FindActionMap("Player")?.Disable();
            actions.FindActionMap("Dialogue")?.Enable();

        }
        private void OnDestroy()
        {

            DialogueManager.Instance.OnLineRead -= HandleLineRead;
            DialogueManager.Instance.OnChoicesReady -= HandleChoicesReady;
        }
    }
}