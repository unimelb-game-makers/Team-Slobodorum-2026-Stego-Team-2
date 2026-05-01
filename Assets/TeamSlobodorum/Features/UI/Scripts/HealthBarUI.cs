using TeamSlobodorum.Entities;
using UnityEngine;
using UnityEngine.UIElements;

namespace TeamSlobodorum.UI.Scripts
{
    [RequireComponent(typeof(WorldSpaceTracker))]
    public class HealthBarUI : MonoBehaviour
    {
        private WorldSpaceTracker _worldSpaceTracker;
        [SerializeField] private LivingEntity entity;

        void Start()
        {
            _worldSpaceTracker = GetComponent<WorldSpaceTracker>();
            entity.HealthManager.Damaged += UpdateHitPoints;
            _worldSpaceTracker.OnComponentReady += UpdateHitPoints;
        }


        private void UpdateHitPoints()
        {
            var healthBar = _worldSpaceTracker.VisualElement.Q<ProgressBar>("HealthBar");
            healthBar.value = entity.HealthManager.HitPoints / entity.HealthManager.maxHitPoints;
        }
    }
}