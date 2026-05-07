using System.Collections.Generic;
using Ink.Runtime;
using TeamSlobodorum.Dialogue;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;
using DG.Tweening; // Required for DOTween

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
        [Header("Animation & Timing")]
        [SerializeField] private float fadeDuration = 0.3f;
        [SerializeField] private float timePerCharacter = 0.02f;
        [SerializeField] private float minCooldown = 0.5f;

        public InputActionAsset actions;
        public InputActionReference continueAction;
        public InputActionReference scrollUpAction;
        public InputActionReference scrollDownAction;

        private UIDocument uiDoc;
        private VisualElement root;
        private Label speakerName;
        private Label dialogueText;
        private VisualElement choicesContainer;
        private VisualElement choicesPanel;
        
        private List<Choice> lastChoices;
        private int activeChoice = -1;
        private string lastPersonSpeakTo;

        private bool canContinue = false;
        private Tween cooldownTween;
        private Tween rootFadeTween;
        private Tween textFadeTween;
        private Tween choicesFadeTween;
        private bool dialogueOpened = false;
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
            canContinue = false;
            cooldownTween?.Kill();

            dialogueText.text = $"{speaker}: {text}";
            if (speaker != playerName)
            {
                lastPersonSpeakTo = speaker;
            }
            choicesContainer.Clear();
            choicesPanel.style.display = DisplayStyle.None;
            activeChoice = -1;

            // Fade in text
            textFadeTween?.Kill();
            dialogueText.style.opacity = 0f;
            textFadeTween = DOTween.To(() => dialogueText.style.opacity.value, x => dialogueText.style.opacity = x, 1f, fadeDuration);

            // Calculate dynamic cooldown
            float calculatedDelay = Mathf.Max(minCooldown, text.Length * timePerCharacter);
            cooldownTween = DOVirtual.DelayedCall(calculatedDelay, () => canContinue = true);
        }

        private void HandleChoicesReady(List<Choice> choices)
        {
            canContinue = false;
            cooldownTween?.Kill();

            choicesPanel.style.display = DisplayStyle.Flex;
            lastChoices = choices;
            activeChoice = 0;
            RefreshChoice();

            // Fade in choices
            choicesFadeTween?.Kill();
            choicesContainer.style.opacity = 0f;
            choicesFadeTween = DOTween.To(() => choicesContainer.style.opacity.value, x => choicesContainer.style.opacity = x, 1f, fadeDuration)
                .OnComplete(() => canContinue = true); 
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
            if (!dialogueOpened || !canContinue) return;

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
            if (!dialogueOpened || !canContinue) return;

            if (activeChoice > 0)
            {
                activeChoice -= 1;
                RefreshChoice();
            }
        }

        private void OnScrollDown(InputAction.CallbackContext context)
        {
            if (!dialogueOpened || !canContinue) return;

            if (activeChoice != -1 && activeChoice < lastChoices.Count - 1)
            {
                activeChoice += 1;
                RefreshChoice();
            }
        }

        public void HideUI()
        {
            dialogueOpened = false;
            actions.FindActionMap("Dialogue")?.Disable();
            actions.FindActionMap("Player")?.Enable();
            Cursor.lockState = CursorLockMode.Locked;
            rootFadeTween?.Kill();
            rootFadeTween = DOTween.To(() => root.style.opacity.value, x => root.style.opacity = x, 0f, fadeDuration)
                .OnComplete(() => root.style.display = DisplayStyle.None);
        }

        public void ShowUI()
        {   
            dialogueOpened = true;
            root.style.display = DisplayStyle.Flex;
            Cursor.lockState = CursorLockMode.None;
            actions.FindActionMap("Player")?.Disable();
            actions.FindActionMap("Dialogue")?.Enable();
            rootFadeTween?.Kill();
            root.style.opacity = 0f;
            rootFadeTween = DOTween.To(() => root.style.opacity.value, x => root.style.opacity = x, 1f, fadeDuration);
        }

        private void OnDestroy()
        {
            // Clean up tweens to prevent memory leaks
            cooldownTween?.Kill();
            rootFadeTween?.Kill();
            textFadeTween?.Kill();
            choicesFadeTween?.Kill();

            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.OnLineRead -= HandleLineRead;
                DialogueManager.Instance.OnChoicesReady -= HandleChoicesReady;
            }
        }
    }
}