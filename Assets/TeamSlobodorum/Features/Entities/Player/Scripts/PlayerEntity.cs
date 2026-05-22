using TeamSlobodorum.UI.Scripts;

namespace TeamSlobodorum.Entities.Player
{
    public class PlayerEntity : LivingEntity
    {
        protected override void Start()
        {
            base.Start();
            if (UIManager.Instance != null)
            {
                HealthManager.Died += UIManager.Instance.OnGameOver.Invoke;
            }
        }
    }
}