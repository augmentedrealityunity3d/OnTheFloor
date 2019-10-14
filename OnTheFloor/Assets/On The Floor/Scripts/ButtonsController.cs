using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonsController : MonoBehaviour
{
    public void CloseOnTheFloor(int option)
    {
        OnTheFloorManager.instance.CloseToastMessage();

        //Click yes option
        if (option == 0)
        {
            Application.Quit();
        }
    }
}
