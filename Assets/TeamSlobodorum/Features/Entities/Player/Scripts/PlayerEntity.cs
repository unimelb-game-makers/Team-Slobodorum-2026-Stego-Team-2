using TeamSlobodorum.UI.Scripts;

namespace TeamSlobodorum.Entities.Player
{
    public class PlayerEntity : LivingEntity
    {
        void Start()
        {
            if (UIManager.Instance != null)
            {
                Died += UIManager.Instance.OnGameOver.Invoke;
            }
        }

    }
}