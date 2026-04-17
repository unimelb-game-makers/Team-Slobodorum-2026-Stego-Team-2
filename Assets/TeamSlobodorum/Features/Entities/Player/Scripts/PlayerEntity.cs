namespace TeamSlobodorum.Entities.Player
{
    public class PlayerEntity : LivingEntity
    {
        public float maxHitPoints = 100.0f;
        
        protected override void Awake()
        {
            base.Awake();
            
            MaxHitPoints = maxHitPoints;
        }
    }
}