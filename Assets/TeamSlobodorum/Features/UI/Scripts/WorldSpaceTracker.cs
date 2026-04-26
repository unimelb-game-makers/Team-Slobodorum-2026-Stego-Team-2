using UnityEngine;
using UnityEngine.UIElements;

namespace TeamSlobodorum.UI.Scripts
{
    public class WorldSpaceTracker: MonoBehaviour
    {
        [SerializeField] private VisualTreeAsset template;
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset;
        [SerializeField] private bool occlusion_check = true;
        [SerializeField] private bool registerOnStart = false;

        private VisualElement visualElement;

        public VisualElement VisualElement => visualElement;

        public void RegisterComponent()
        {
            visualElement = UIManager.Instance?.worldSpaceUIController?.RegisterTracker(template, transform, offset, occlusion_check);

        }

        public void UnregisterComponent()
        {
            UIManager.Instance?.worldSpaceUIController?.UnregisterTracker(visualElement);
            visualElement = null;
        }

        void Start()
        {   
            if (registerOnStart) RegisterComponent();
        }

        private void OnDestroy()
        {
            if (visualElement != null)
            {
                UnregisterComponent();
            }
        }
    }
}