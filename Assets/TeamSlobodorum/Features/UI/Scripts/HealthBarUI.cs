using TeamSlobodorum.Entities;
using TeamSlobodorum.Entities.Enemy;
using TeamSlobodorum.Entities.HostileRobot.Behaviour;
using UnityEngine;
using UnityEngine.UIElements;

namespace TeamSlobodorum.UI.Scripts
{
    [RequireComponent(typeof(WorldSpaceTracker))]
    public class HealthBarUI : MonoBehaviour
    {
        private WorldSpaceTracker _worldSpaceTracker;
        [SerializeField] private LivingEntity entity;
        
        [SerializeField] private Texture2D alertIcon;
        [SerializeField] private Texture2D searchIcon;
        [SerializeField] private Texture2D attackIcon;
        
        private ProgressBar _healthBar;
        private Image _stateIcon;
        private EnemyEntity _enemyEntity;

        void Start()
        {
            _worldSpaceTracker = GetComponent<WorldSpaceTracker>();
            
            entity.HealthManager.Damaged += UpdateHitPoints;
            _worldSpaceTracker.OnComponentReady += UpdateHitPoints;
            if (TryGetComponent(out _enemyEntity))
            {
                _enemyEntity.EnemyStateChanged += UpdateEnemyState;
            }
        }
        
        private void UpdateHitPoints()
        {
            _healthBar ??= _worldSpaceTracker.VisualElement.Q<ProgressBar>("HealthBar");
            _healthBar.value = entity.HealthManager.HitPoints / entity.HealthManager.maxHitPoints;
        }

        private void UpdateEnemyState()
        {
            _stateIcon ??= _worldSpaceTracker.VisualElement.Q<Image>("StateIcon");
            switch (_enemyEntity.EnemyState)
            {
                case EnemyState.Normal:
                    _stateIcon.image = null;
                    break;
                case EnemyState.Alert:
                    _stateIcon.image = alertIcon;
                    break;
                case EnemyState.Search:
                    _stateIcon.image = searchIcon;
                    break;
                case EnemyState.Attack:
                    _stateIcon.image = attackIcon;
                    break;
            }
        }
    }
}