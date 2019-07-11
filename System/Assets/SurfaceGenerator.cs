using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GoogleARCore;

public class SurfaceGenerator : MonoBehaviour
{
    //public GameObject surfacePrefab;

    //void Update()
    //{
    //    if(Frame.TrackingState != FrameTrackingState.Tracking)
    //    {
    //        return;
    //    }

    //    var newPlanes = new List<TrackedPlane>();

    //    Frame.GetNewPlanes(ref newPlanes);

    //    foreach(var p in newPlanes)
    //    {
    //        var go = GameObject.Instantiate(surfacePrefab, Vector3.zero, Quaternion.identity);
    //        go.GetComponent<Surface>().trackedPlane = p;
    //    }
    //}

    public GameObject DetectedPlanePrefab;
    private List<DetectedPlane> m_NewPlanes = new List<DetectedPlane>();

    public void Update()
    {
        if (Session.Status != SessionStatus.Tracking)
        {
            return;
        }

        Session.GetTrackables<DetectedPlane>(m_NewPlanes, TrackableQueryFilter.New);
        for (int i = 0; i < m_NewPlanes.Count; i++)
        {
            GameObject planeObject = Instantiate(DetectedPlanePrefab, Vector3.zero, Quaternion.identity, transform);
            planeObject.GetComponent<DetectedPlaneVisualizer>().Initialize(m_NewPlanes[i]);
        }
    }
}
