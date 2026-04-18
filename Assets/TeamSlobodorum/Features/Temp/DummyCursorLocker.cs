using Unity.TeamSlobodorum.Utility;
using UnityEngine;

public class DummyCursorLocker: MonoBehaviour
{
    public void Start()
    {
        if (!TryGetComponent<CursorLockManager>(out var manager))
        {
            Debug.LogWarning("CursorLockManager not found on the same GameObject.");
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            return;
        }

        manager.LockCursor();
    }
}
