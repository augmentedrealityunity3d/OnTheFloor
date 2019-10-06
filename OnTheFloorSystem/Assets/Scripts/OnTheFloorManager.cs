using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;
using UnityEngine.UI;

public class OnTheFloorManager : MonoBehaviour
{
    public static OnTheFloorManager instance;

    public Text toastText;
    public int lostTrackingSleepTimeout;
    public int toastDisplayTime;
    public GameObject tile;
    public GameObject toastBox;
    public GameObject toastYes, toastNo;
    public Text tileCount;

    private int count;

    // A list to hold new planes ARCore began tracking in the current frame
    private List<DetectedPlane> m_NewPlanes = new List<DetectedPlane>();

    void Awake()
    {
        //Create singleton for OnTheFloor Manager
        if(instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        //Check for connection errors
        DetectConnectionError();

        toastYes.SetActive(false);
        toastNo.SetActive(false);
        toastBox.SetActive(false);

        count = 0;
    }

    void Update()
    {
        // The session status must be Tracking in order to access the Frame.
        if (Session.Status != SessionStatus.Tracking)
        {
            Screen.sleepTimeout = lostTrackingSleepTimeout;
            return;
        }

        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        tileCount.text = count.ToString();

        Session.GetTrackables<DetectedPlane>(m_NewPlanes, TrackableQueryFilter.New);
        
        //Initiate tiles on the detected planes
        foreach (var curPlane in m_NewPlanes)
        {
            GameObject planeObject = Instantiate(tile, Vector3.zero, Quaternion.identity, transform);
            count++;
            planeObject.GetComponent<DetectedPlaneVisualizer>().Initialize(curPlane);
        }
    }

    //Check for connection errors
    private void DetectConnectionError()
    {
        if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
        {
            // This is for camera permission error
            StartCoroutine(ShowToastMessage("Camera permission is needed to run this application", toastDisplayTime));
        }
        else if (Session.Status.IsError())
        {
            // This is for variety of errors
            StartCoroutine(ShowToastMessage("ARCore encountered a problem connecting. Please restart the app", toastDisplayTime));
        }
    }

    //Create toast message
    private IEnumerator ShowToastMessage(string text, int duration)
    {
        Color orginalColor = toastText.color;

        toastText.text = text;
        toastBox.SetActive(true);

        //Fade in
        yield return FadeInAndOut(toastText, true, 0.5f);

        //Wait for the duration
        float counter = 0;
        while (counter < duration)
        {
            counter += Time.deltaTime;
            yield return null;
        }

        //Fade out
        yield return FadeInAndOut(toastText, false, 0.5f);

        toastBox.SetActive(false);
        toastText.color = orginalColor;

        Application.Quit();
    }

    //Animate toast message
    public IEnumerator FadeInAndOut(Text targetText, bool fadeIn, float duration)
    {
        //Set Values depending on if fadeIn or fadeOut
        float a, b;
        if (fadeIn)
        {
            a = 0f;
            b = 1f;
        }
        else
        {
            a = 1f;
            b = 0f;
        }

        Color currentColor = Color.clear;
        float counter = 0f;

        while (counter < duration)
        {
            counter += Time.deltaTime;
            float alpha = Mathf.Lerp(a, b, counter / duration);

            targetText.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
            yield return null;
        }
    }
}
