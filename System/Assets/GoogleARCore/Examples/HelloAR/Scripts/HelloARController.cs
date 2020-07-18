namespace GoogleARCore.Examples.HelloAR
{
    using System.Collections.Generic;
    using GoogleARCore;
    using GoogleARCore.Examples.Common;
    using UnityEngine;
    using UnityEngine.UI;

#if UNITY_EDITOR
    // Set up touch input propagation while using Instant Preview in the editor.
    using Input = InstantPreviewInput;
#endif

    public class HelloARController : MonoBehaviour
    {
        public Camera FirstPersonCamera;
        public GameObject DetectedPlanePrefab;
        public GameObject AndyPlanePrefab;
        public GameObject AndyPointPrefab;

        private const float k_ModelRotation = 180.0f;
        private bool m_IsQuitting = false;
        private List<DetectedPlane> newPlanes = new List<DetectedPlane>();

        public void Update()
        {
            _UpdateApplicationLifecycle();

            if (Session.Status != SessionStatus.Tracking)
            {
                return;
            }

            Session.GetTrackables<DetectedPlane>(newPlanes, TrackableQueryFilter.New);

            for (int i = 0; i < newPlanes.Count; i++)
            {
                GameObject andyObject = Instantiate(AndyPointPrefab, newPlanes[i].CenterPose.position, AndyPointPrefab.transform.rotation, OnTheFloorManager.Instance.tilesParent);
                andyObject.GetComponent<MeshRenderer>().material.mainTexture = Resources.Load<Texture>("Tiles/" + OnTheFloorManager.Instance.selectedTileId);
                OnTheFloorManager.Instance.IncreaseTileCount();
                OnTheFloorManager.Instance.AddTilesIntoList(andyObject);
            }
        }

        private void _UpdateApplicationLifecycle()
        {
            // Only allow the screen to sleep when not tracking.
            if (Session.Status != SessionStatus.Tracking)
            {
                const int lostTrackingSleepTimeout = 15;
                Screen.sleepTimeout = lostTrackingSleepTimeout;
            }
            else
            {
                Screen.sleepTimeout = SleepTimeout.NeverSleep;
            }

            if (m_IsQuitting)
            {
                return;
            }

            if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
            {
                _ShowAndroidToastMessage("Camera permission is needed to run this application.");
                m_IsQuitting = true;
                Invoke("_DoQuit", 0.5f);
            }
            else if (Session.Status.IsError())
            {
                _ShowAndroidToastMessage("ARCore encountered a problem connecting.  Please start the app again.");
                m_IsQuitting = true;
                Invoke("_DoQuit", 0.5f);
            }
        }

        private void _ShowAndroidToastMessage(string message)
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            if (unityActivity != null)
            {
                AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
                unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity,
                        message, 0);
                    toastObject.Call("show");
                }));
            }
        }
    }
}
