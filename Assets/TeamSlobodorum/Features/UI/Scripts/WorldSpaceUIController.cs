using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace TeamSlobodorum.UI.Scripts
{
    public class WorldSpaceUIController : MonoBehaviour
    {
        [SerializeField] private UIDocument worldUIDocument;
        private Camera mainCamera;
        private class TrackerEntry
        {
            public Transform Target;
            public VisualElement Element;
            public Vector3 Offset;
            public bool occlusion_check;
        }

        private List<TrackerEntry> activeTrackers = new List<TrackerEntry>();
        private List<TrackerEntry> removalQueue = new List<TrackerEntry>();
        [SerializeField] private LayerMask occlusionLayerMask;
        private void Awake()
        {
            mainCamera = Camera.main;
        }

        /// <summary>
        /// Logic for the UIManager to inject new widgets into the system.
        /// </summary>
        public VisualElement RegisterTracker(VisualTreeAsset template, Transform target, Vector3 offset, bool occlusion_check)
        {
            var instance = template.Instantiate();

            var element = instance;

            element.style.position = Position.Absolute;

            worldUIDocument.rootVisualElement.Add(element);

            activeTrackers.Add(new TrackerEntry
            {
                Target = target,
                Element = element,
                Offset = offset,
                occlusion_check = occlusion_check,
            });

            return element;
        }

        public void UnregisterTracker(VisualElement element)
        {
            if (element == null) return;

            // Find the entry associated with this VisualElement
            var entry = activeTrackers.Find(t => t.Element == element);
            if (entry != null && !removalQueue.Contains(entry))
            {
                removalQueue.Add(entry);
            }
        }

        private void LateUpdate()
        {
            if (activeTrackers.Count == 0) return;

            for (int i = 0; i < activeTrackers.Count; i++)
            {
                var tracker = activeTrackers[i];

                // prevention clean up
                if (tracker.Target == null)
                {
                    removalQueue.Add(tracker);
                    continue;
                }

                UpdateElementPosition(tracker);
            }

            // Process removals after the loop to avoid collection modification errors
            CleanUpTrackers();
        }

        private void UpdateElementPosition(TrackerEntry tracker)
        {
            Vector3 targetPos = tracker.Target.position + tracker.Offset;
            Vector3 cameraPos = mainCamera.transform.position;

            if (Physics.Linecast(cameraPos, targetPos, out RaycastHit hit, occlusionLayerMask))
            {
                if (hit.transform != tracker.Target)
                {
                    tracker.Element.style.display = DisplayStyle.None;
                    return;
                }
            }

            tracker.Element.style.display = DisplayStyle.Flex;
            Vector3 worldPos = tracker.Target.position + tracker.Offset;
            Vector3 screenPoint = mainCamera.WorldToScreenPoint(worldPos);

            // Culling logic: Only show if in front of camera
            if (screenPoint.z > 0)
            {
                tracker.Element.style.display = DisplayStyle.Flex;

                Vector2 panelPos = RuntimePanelUtils.CameraTransformWorldToPanel(
                    worldUIDocument.rootVisualElement.panel,
                    worldPos,
                    mainCamera
                );

                tracker.Element.style.left = panelPos.x;
                tracker.Element.style.top = panelPos.y;
            }
            else
            {
                tracker.Element.style.display = DisplayStyle.None;
            }
        }

        private void CleanUpTrackers()
        {
            if (removalQueue.Count == 0) return;

            foreach (var tracker in removalQueue)
            {
                tracker.Element?.RemoveFromHierarchy();
                activeTrackers.Remove(tracker);
            }
            removalQueue.Clear();
        }
    }
}