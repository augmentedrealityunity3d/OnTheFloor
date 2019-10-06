using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInput : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        //Clarify the user to close the app or to continue
        if (Input.GetKey("escape"))
        {
            OnTheFloorManager.instance.toastYes.SetActive(true);
            OnTheFloorManager.instance.toastNo.SetActive(true);

            StartCoroutine(ShowToastMessage("Do you really need to close it?", OnTheFloorManager.instance.toastDisplayTime));
        }
    }

    //Create toast message
    private IEnumerator ShowToastMessage(string text, int duration)
    {
        Color orginalColor = OnTheFloorManager.instance.toastText.color;

        OnTheFloorManager.instance.toastText.text = text;
        OnTheFloorManager.instance.toastBox.SetActive(true);

        //Fade in
        yield return OnTheFloorManager.instance.FadeInAndOut(OnTheFloorManager.instance.toastText, true, 0.5f);
    }

    //Close toast message
    private IEnumerator CloseToastMessage(int option)
    {
        Color orginalColor = OnTheFloorManager.instance.toastText.color;

        //Fade out
        yield return OnTheFloorManager.instance.FadeInAndOut(OnTheFloorManager.instance.toastText, false, 0.5f);

        OnTheFloorManager.instance.toastYes.SetActive(false);
        OnTheFloorManager.instance.toastNo.SetActive(false);

        OnTheFloorManager.instance.toastBox.SetActive(false);
        OnTheFloorManager.instance.toastText.color = orginalColor;

        //Yes Button - Quit application
        if(option == 0)
        {
            Application.Quit();
        }        
    }

    //Function to call from Toast box button clicks
    public void ToastOptionButtons(int option)
    {
        StartCoroutine(CloseToastMessage(option));
    }
}
