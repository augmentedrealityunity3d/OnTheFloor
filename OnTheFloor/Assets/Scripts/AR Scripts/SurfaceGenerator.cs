using System.Collections.Generic;
using GoogleARCore;
using UnityEngine;

public class SurfaceGenerator : MonoBehaviour
{
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
            planeObject.GetComponent<Surface>().Initialize(m_NewPlanes[i]);
        }
    }
}

