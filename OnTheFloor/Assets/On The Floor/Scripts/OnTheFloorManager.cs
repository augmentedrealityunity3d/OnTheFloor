using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;
using UnityEngine.UI;

public class OnTheFloorManager : MonoBehaviour
{
    public static OnTheFloorManager instance;

    public GameObject toastBox;

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

    }

    void Update()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        //Clarify the user to close the app or to continue
        if (Input.GetKey("escape"))
        {
            StartCoroutine("ShowToastMessage");
        }
    }

    //Create toast message
    private IEnumerator ShowToastMessage()
    {
        toastBox.SetActive(true);

        //Fade in
        yield return FadeInAndOut(true, 0.5f);
    }

    //Close toast message
    public void CloseToastMessage()
    {
        //Fade out
        yield return FadeInAndOut(false, 0.5f);

        toastBox.SetActive(false);
    }

    //Animate toast message
    private IEnumerator FadeInAndOut(bool fadeIn, float duration)
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

        float counter = 0f;

        while (counter < duration)
        {
            counter += Time.deltaTime;
            yield return null;
        }
    }
}
