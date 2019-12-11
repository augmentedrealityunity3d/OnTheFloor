using GoogleARCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectedPlaneTileVisualizer : MonoBehaviour
{
    private DetectedPlane m_DetectedPlane;

    private void Update()
    {
        if (m_DetectedPlane == null)
        {
            return;
        }
        else if (m_DetectedPlane.SubsumedBy != null)
        {
            Destroy(gameObject);
            return;
        }
        else if (m_DetectedPlane.TrackingState != TrackingState.Tracking)
        {
            return;
        }
    }

    internal void Initialize(DetectedPlane plane)
    {
        m_DetectedPlane = plane;
    }
}
