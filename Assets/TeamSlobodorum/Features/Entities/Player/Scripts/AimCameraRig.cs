using UnityEngine;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine.Serialization;

namespace TeamSlobodorum.Entities.Player
{
    /// <summary>
    /// This is a custom camera manager that selects between an aiming camera child and a
    /// non-aiming camera child, depending on the value of some user input.
    ///
    /// The Aiming child is expected to have ThirdPersonFollow and ThirdPersonAim components,
    /// and to have a player as its Follow target.  The player is expected to have a
    /// SimplePlayerAimController behaviour on one of its children, to decouple aiminag and
    /// player rotation.
    /// </summary>
    public class AimCameraRig : CinemachineCameraManagerBase, IInputAxisOwner
    {
        public InputAxis aimMode = InputAxis.DefaultMomentary;

        private PlayerAimController _aimController;
        private CinemachineVirtualCameraBase _aimCamera;
        private CinemachineVirtualCameraBase _freeCamera;

        public bool IsAiming => aimMode.Value > 0.5f;

        /// Report the available input axes to the input axis controller.
        /// We use the Input Axis Controller because it works with both the Input package
        /// and the Legacy input system.  This is sample code and we
        /// want it to work everywhere.
        void IInputAxisOwner.GetInputAxes(List<IInputAxisOwner.AxisDescriptor> axes)
        {
            axes.Add(new() { DrivenAxis = () => ref aimMode, Name = "Aim" });
        }

        void OnValidate() => aimMode.Validate();

        protected override void Start()
        {
            base.Start();

            // Find the player and the aiming camera.
            // We expect to have one camera with a CinemachineThirdPersonAim component
            // whose Follow target is a player with a SimplePlayerAimController child.
            for (int i = 0; i < ChildCameras.Count; ++i)
            {
                var cam = ChildCameras[i];
                if (!cam.isActiveAndEnabled)
                    continue;
                if (_aimCamera == null
                    && cam.TryGetComponent<CinemachineThirdPersonAim>(out var aim)
                    && aim.NoiseCancellation)
                {
                    _aimCamera = cam;
                    var trackingTarget = _aimCamera.Follow;
                    if (trackingTarget != null)
                        _aimController = trackingTarget.GetComponent<PlayerAimController>();
                }
                else if (_freeCamera == null)
                    _freeCamera = cam;
            }
            if (_aimCamera == null)
                Debug.LogError("AimCameraRig: no valid CinemachineThirdPersonAim camera found among children");
            if (_aimController == null)
                Debug.LogError("AimCameraRig: no valid SimplePlayerAimController target found");
            if (_freeCamera == null)
                Debug.LogError("AimCameraRig: no valid non-aiming camera found among children");
        }

        protected override CinemachineVirtualCameraBase ChooseCurrentCamera(Vector3 worldUp, float deltaTime)
        {
            var oldCam = (CinemachineVirtualCameraBase)LiveChild;
            var newCam = IsAiming ? _aimCamera : _freeCamera;
            if (_aimController != null && oldCam != newCam)
            {
                // Set the mode of the player aim controller.
                // We want the player rotation to be coupled to the camera when aiming, otherwise not.
                _aimController.playerRotation = IsAiming
                    ? PlayerAimController.CouplingMode.Coupled
                    : PlayerAimController.CouplingMode.Decoupled;
                _aimController.RecenterPlayer();
            }
            return newCam;
        }
    }
}
