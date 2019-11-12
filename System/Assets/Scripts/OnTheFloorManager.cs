using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OnTheFloorManager : MonoBehaviour
{
    #region PROPERTIES
    public static OnTheFloorManager Instance { get; private set; }

    [Header("MENU PROPERTIES")]
    public RectTransform sideNavigation;
    public Transform tileButtonsParent;
    public Button tileButtonPrefab;
    public Transform tilesParent;
    public int defaultTileSizeOption;
    public Transform tileResizeParent;
    private int sideMenuClickedCount;
    private float sideNavigationWidth = 200;
    #endregion

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        sideMenuClickedCount = 0;

        ResetTileSprite();

        //TODO:REPLACE LOOP COUNT WITH DB COUNT
        GenerateDynamicButtonForSideMenu();
    }

    #region SIDE MENU FUNCTONS
    private void GenerateDynamicButtonForSideMenu()
    {
        for (int i = 0; i < 3; i++)
        {
            Button btn = Instantiate(tileButtonPrefab, tileButtonsParent);
            btn.GetComponent<Image>().sprite = Resources.Load<Sprite>("Tiles/" + (i + 1));
            btn.GetComponent<TileButtonId>().SetTileId(i+1);
        }
    }

    public void ChangeTileSize(int val)
    {
        //30cm * 30cm
        if(val == 0)
        {
            tilesParent.localScale = new Vector2(0.3f, 0.3f);
        }
        //60cm * 60cm
        else if(val == 1)
        {
            tilesParent.localScale = new Vector2(0.6f, 0.6f);
        }
        //80cm * 80cm
        else if (val == 2)
        {
            tilesParent.localScale = new Vector2(0.8f, 0.8f);
        }
        //1m * 1m
        else if (val == 3)
        {
            tilesParent.localScale = new Vector2(1, 1);
        }
    }
    
    internal void SetTileSprite(int tileId)
    {
        if (tilesParent.childCount > 0)
        {
            for (int i = 0; i < tilesParent.childCount; i++)
            {
                tilesParent.GetChild(i).GetComponent<MeshRenderer>().material.mainTexture = Resources.Load<Texture>("Tiles/" + tileId);
            }
        }
        else
        {
            Debug.Log("No Tiles On the Floor");
        }
        
    }
    #endregion

    #region MAIN MENU FUNCTIONS
    public void PopUpSideMenu()
    {
        //Move In
        if (sideMenuClickedCount == 0)
        {
            sideNavigation.localPosition = sideNavigation.localPosition + new Vector3(10000 + sideNavigationWidth * 0.5f, 0, 0);
            sideMenuClickedCount = 1;
        }
        //Move Out
        else if (sideMenuClickedCount == 1)
        {
            sideNavigation.localPosition = sideNavigation.localPosition + new Vector3(-(10000 + sideNavigationWidth * 0.5f), 0, 0);
            sideMenuClickedCount = 0;
        }
    }
    
    public void ResetTileSprite()
    {
        if(tilesParent.childCount > 0)
        {
            for (int i = 0; i < tilesParent.childCount; i++)
            {
                tilesParent.GetChild(i).GetComponent<MeshRenderer>().material.mainTexture = Resources.Load<Texture>("Tiles/1");
            }
        }
        else
        {
            Debug.Log("No Tiles On the Floor");
        }

        tileResizeParent.GetChild(defaultTileSizeOption - 1).GetComponent<Toggle>().isOn = true;
    }
    #endregion
}
