using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using System;
using Cursor = UnityEngine.Cursor;
using TeamSlobodorum.DataPersistence;
using Unity.VisualScripting;

namespace TeamSlobodorum.UI.Scripts
{

    public class SystemMenuController : MonoBehaviour
    {
        public UIDocument menuDocument;
        private VisualElement root;
        private SystemOperationStatus status = SystemOperationStatus.Save;
        [SerializeField] private VisualTreeAsset SaveSlotTemplate;    
        private ScrollView slotContainer;
        private VisualElement systemComponent;
        private Label SaveButton;
        private Label LoadButton;
        private void Awake()
        {

            root = menuDocument.rootVisualElement;
            systemComponent = root.Q<VisualElement>("SystemComponent");
            slotContainer = root.Q<ScrollView>("SlotComponent");
            SaveButton = systemComponent.Q<Label>("Save");
            SaveButton.RegisterCallback<ClickEvent>(evt => ToggleSave());
            LoadButton = systemComponent.Q<Label>("Load");
            LoadButton.RegisterCallback<ClickEvent>(evt => ToggleLoad());
            systemComponent.Q<VisualElement>("Quit").RegisterCallback<ClickEvent>(evt => BackToStartMenu());
            
        }

        private void Start()
        {
            Refresh();
            if (SaveManager.instance != null)
            {
                SaveManager.instance.OnManifestSlotUpdate += Refresh;

            }
        }

        private void ToggleSave()
        {
            status = SystemOperationStatus.Save;
            SaveButton.EnableInClassList("system-tab-active", true);
            LoadButton.EnableInClassList("system-tab-active", false);

            Refresh();
        }

        private void ToggleLoad()
        {
            SaveButton.EnableInClassList("system-tab-active", false);
            LoadButton.EnableInClassList("system-tab-active", true);

            status = SystemOperationStatus.Load;
            Refresh();
        }
        public void Refresh()
        {      
            slotContainer.Clear();
            slotContainer.scrollOffset = Vector2.zero;
            var slots = SaveManager.instance?.GetSlotsSortedByNewest();
            if (status == SystemOperationStatus.Save)
            {
                TemplateContainer slotInstance = SaveSlotTemplate.Instantiate();
                Label title = slotInstance.Q<Label>("SaveSlotName");
                title.text = "Create New Slot";
                slotInstance.RegisterCallback<ClickEvent>(evt => AddNewSave());
                slotContainer.Add(slotInstance);
                
            }
            if (slots != null)
            {
                Debug.Log("slot");

                foreach(SaveSlotMeta slot in slots)
                {
                    TemplateContainer slotInstance = SaveSlotTemplate.Instantiate();
                    slotInstance.RegisterCallback<ClickEvent>(evt => OnSlotClick(slot));
                    Label title = slotInstance.Q<Label>("SaveSlotName");
                    title.text = slot.profileId;
                    Label date = slotInstance.Q<Label>("LastPlayedDate");
                    date.text = slot.lastPlayedDate;

                    Debug.Log("slot.profileId");
                    slotContainer.Add(slotInstance);
                }
            }


        }  
        private void OnSlotClick(SaveSlotMeta slot)
        {
            SaveManager.instance?.ChangeProfile(slot.profileId);

            if (status == SystemOperationStatus.Save)
            {
                SaveManager.instance.SaveGame();
            }
            else if (status == SystemOperationStatus.Load)
            {
                SaveManager.instance.LoadGame();

            }
            Refresh();
        }

        private void AddNewSave()
        {
            SaveManager.instance?.GenerateNewProfileId();
            SaveManager.instance.SaveGame();

        }

        public void BackToStartMenu()
        {
            SaveManager.instance?.LoadLevelAsync("StartMenu");
        }

        void Oestroy()
        {
            if (SaveManager.instance != null)
            {
                SaveManager.instance.OnManifestSlotUpdate -= Refresh;

            }

        }
    }

    public enum SystemOperationStatus
    {
        Save,
        Load
    }
}