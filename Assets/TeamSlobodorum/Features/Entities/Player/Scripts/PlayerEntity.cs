namespace TeamSlobodorum.Entities.Player
{
    public class PlayerEntity : LivingEntity
    {
        public float maxHitPoints = 100.0f;
        
        private Flammable.Flammable _flammable;
        
        private void Awake()
        {
            MaxHitPoints = maxHitPoints;
            
            _flammable = GetComponent<Flammable.Flammable>();
        }

        protected override void Start()
        {
            base.Start();
            _flammable.StopBurning += OnStopBurning;
        }

        private void OnStopBurning()
        {
            _flammable.ResetStates();
        }
    }
}