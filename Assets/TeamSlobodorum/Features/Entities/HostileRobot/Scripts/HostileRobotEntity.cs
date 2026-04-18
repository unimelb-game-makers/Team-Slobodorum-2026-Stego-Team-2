namespace TeamSlobodorum.Entities.HostileRobot
{
    public class HostileRobotEntity : LivingEntity
    {
        public float maxHitPoints = 100.0f;

        private float _followCounter;
        
        protected override void Awake()
        {
            base.Awake();
            MaxHitPoints = maxHitPoints;
            Died += OnDied;
        }

        private void OnDied()
        {
            Destroy(gameObject);
        }
    }
}