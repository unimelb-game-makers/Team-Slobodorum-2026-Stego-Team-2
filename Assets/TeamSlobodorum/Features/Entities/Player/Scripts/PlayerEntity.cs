namespace TeamSlobodorum.Entities.Player
{
    public class PlayerEntity : LivingEntity
    {
        public float maxHitPoints = 100.0f;
        
        private void Awake()
        {
            MaxHitPoints = maxHitPoints;
        }
    }
}