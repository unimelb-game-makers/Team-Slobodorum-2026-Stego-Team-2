using UnityEngine;
using Unity.Cinemachine;
using TeamSlobodorum.Dialogue;
using System.Collections.Generic;

public class DialogueCameraController : MonoBehaviour
{
    [System.Serializable]
    public struct DialogueCamera
    {
        public string cameraID; // e.g., "NPC_CloseUp"
        public CinemachineVirtualCameraBase vCam;
    }

    public List<DialogueCamera> dialogueCameras;

    private void Start()
    {       
        ResetCameras();
        DialogueManager.Instance.OnDialogueStarted += InitializeCamera;
        DialogueManager.Instance.OnDialogueEnded += ResetCameras;
    }

    private void OnDestroy()
    {
        if (DialogueManager.Instance == null) return;
        DialogueManager.Instance.OnDialogueStarted -= InitializeCamera;
        DialogueManager.Instance.OnDialogueEnded -= ResetCameras;
    }

    private void InitializeCamera()
    {
        foreach (var cam in dialogueCameras)
        {
            cam.vCam.gameObject.SetActive(true);
        }

        dialogueCameras[0].vCam.Priority = 20; 
    }

    private void HandleCameraCommand(string targetCameraID)
    {
        foreach (var cam in dialogueCameras)
        {
            if (cam.cameraID.ToLower() == targetCameraID.ToLower())
            {
                cam.vCam.Priority = 20; 
            }
            else
            {
                cam.vCam.Priority = 0;
            }
        }
    }

    private void ResetCameras()
    { 
        foreach (var cam in dialogueCameras)
        {
            cam.vCam.gameObject.SetActive(false);
            cam.vCam.Priority = 0;

        }
    }
}