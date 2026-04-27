using TeamSlobodorum.UI.Scripts;
using UnityEngine;

public class WinDetect : MonoBehaviour
{
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                UIManager.Instance.OnGameWin?.Invoke();
            }
        }
}
