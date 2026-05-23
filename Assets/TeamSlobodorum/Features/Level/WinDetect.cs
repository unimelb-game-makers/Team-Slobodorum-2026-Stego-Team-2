using TeamSlobodorum.DataPersistence;
using TeamSlobodorum.UI.Scripts;
using UnityEngine;

public class WinDetect : MonoBehaviour
{
    [SerializeField] private string nextLevel = "";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (nextLevel == "")
            {
                UIManager.Instance.OnGameWin?.Invoke();
            }
            else
            {
                SaveManager.instance?.LoadLevelAsync(nextLevel, false);
            }
        }
    }
}
