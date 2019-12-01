namespace GoogleARCore
{
    using System.Collections.Generic;
    using GoogleARCoreInternal;
    using UnityEngine;

    [HelpURL("https://developers.google.com/ar/reference/unity/class/GoogleARCore/ARCoreSession")]
    public class ARCoreSession : MonoBehaviour
    {
        [Tooltip("The direction of the device camera used by the session.")]
        public DeviceCameraDirection DeviceCameraDirection = DeviceCameraDirection.BackFacing;

        [Tooltip("A scriptable object specifying the ARCore session configuration.")]
        public ARCoreSessionConfig SessionConfig;

        private OnChooseCameraConfigurationDelegate m_OnChooseCameraConfiguration;

        public delegate int OnChooseCameraConfigurationDelegate(List<CameraConfig> supportedConfigurations);

        [SuppressMemoryAllocationError(Reason = "Could create new LifecycleManager")]
        public void Awake()
        {
            LifecycleManager.Instance.CreateSession(this);
        }

        [SuppressMemoryAllocationError(IsWarning = true, Reason = "Requires further investigation.")]
        public void OnDestroy()
        {
            LifecycleManager.Instance.ResetSession();
        }

        [SuppressMemoryAllocationError(Reason = "Enabling session creates a new ARSessionConfiguration")]
        public void OnEnable()
        {
            LifecycleManager.Instance.EnableSession();
        }

        [SuppressMemoryAllocationError(IsWarning = true, Reason = "Requires further investigation.")]
        public void OnDisable()
        {
            LifecycleManager.Instance.DisableSession();
        }

        public void RegisterChooseCameraConfigurationCallback(OnChooseCameraConfigurationDelegate onChooseCameraConfiguration)
        {
            m_OnChooseCameraConfiguration = onChooseCameraConfiguration;
        }

        internal OnChooseCameraConfigurationDelegate GetChooseCameraConfigurationCallback()
        {
            return m_OnChooseCameraConfiguration;
        }
    }
}
