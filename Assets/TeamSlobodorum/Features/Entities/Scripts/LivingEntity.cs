using TeamSlobodorum.Health;
using UnityEngine;

namespace TeamSlobodorum.Entities
{
    [RequireComponent(typeof(HealthManager))]
    public class LivingEntity : Entity
    {
        public Transform eyes;
        [SerializeField] private float voidThreshold = -500f;

        public HealthManager HealthManager { get; private set; }
        private Flammable.Flammable _flammable;

        protected override void Awake()
        {   
            base.Awake();
            // put in awake to ensure it get initialised before call
            HealthManager = GetComponent<HealthManager>();
            HealthManager.Died += OnDied;
        }
        
        protected virtual void Start()
        {
            if (TryGetComponent(out _flammable))
            {
                _flammable.StopBurning += OnStopBurning;
            }
        }

        protected virtual void Update()
        {
            // Void Fall
            if (transform.position.y < voidThreshold && HealthManager.IsAlive)
            {
                HealthManager.Kill();
            }
        }
        
        protected virtual void OnDied() {}

        private void OnStopBurning()
        {
            _flammable.ResetStates();
        }
    }
}