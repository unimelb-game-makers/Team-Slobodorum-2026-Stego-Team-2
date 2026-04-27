using System;
using TeamSlobodorum.Entities;
using UnityEngine;
using UnityEngine.UIElements;

namespace TeamSlobodorum.UI.Scripts
{
    [RequireComponent(typeof(WorldSpaceTracker))]
    public class HealthBarUI: MonoBehaviour
    {
        private WorldSpaceTracker worldSpaceTracker;
        [SerializeField] private LivingEntity entity;
        void Start()
        {
            worldSpaceTracker = GetComponent<WorldSpaceTracker>();
            entity.Damaged += UpdateHitPoints;
            worldSpaceTracker.OnComponentReady += UpdateHitPoints;
        }

        
        private void UpdateHitPoints()
        {   
            ProgressBar _healthBar = worldSpaceTracker.VisualElement.Q<ProgressBar>("HealthBar");

            _healthBar.value = entity.HitPoints / entity.maxHitPoints;
        }

    }
}