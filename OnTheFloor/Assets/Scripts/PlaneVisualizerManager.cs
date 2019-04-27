using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;
using GoogleARCore.Examples.Common;//using GoogleARCore.Examples.HelloAR;

public class PlaneVisualizerManager : MonoBehaviour
{
    // A prefab for tracking and visualizing detected planes.
    public GameObject TrackedPlanePrefab;

    private List<TrackedPlane> _newPlanes = new List<TrackedPlane>();

    //called once per frame
    void Update()
    {
        Session.GetTrackables<TrackedPlane>(_newPlanes, TrackableQueryFilter.New);

        // Iterate over planes found in this frame and instantiate corresponding GameObjects to visualize them.
        foreach (var curPlane in _newPlanes)
        {
            // Instantiate a plane visualization prefab and set it to track the new plane.
            // The transform is set to the origin with an identity rotation since the mesh for our prefab is updated in Unity World coordinates.
            var planeObject = Instantiate(TrackedPlanePrefab, Vector3.zero, Quaternion.identity, transform);
            planeObject.GetComponent<DetectedPlaneVisualizer>().Initialize(curPlane);//planeObject.GetComponent<TrackedPlaneVisualizer>().Initialize(curPlane);

            // Apply a random color and grid rotation.
            planeObject.GetComponent<Renderer>().material.SetColor("_GridColor", new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f)));
            planeObject.GetComponent<Renderer>().material.SetFloat("_UvRotation", Random.Range(0.0f, 360.0f));
        }
    }
}
