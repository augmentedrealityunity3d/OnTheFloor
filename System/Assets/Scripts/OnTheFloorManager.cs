using System;
using System.Collections.Generic;
using System.IO;
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
    public Text tileCount;
    public GameObject tilePrefab;

    internal int selectedTileId;
    internal List<GameObject> createdTiles;

    private int sideMenuClickedCount;
    private int tileCountNumber;
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
        float pixelPerUnit = Screen.height / 20; //Camera Z = 10
        float sideNavWidthPix = Screen.width * 0.25f;
        float widthUnits = sideNavWidthPix / pixelPerUnit;

        sideNavigation.transform.localScale = new Vector3(widthUnits / 8, sideNavigation.transform.localScale.y, sideNavigation.transform.localScale.z);

        sideMenuClickedCount = 0;
        tileCountNumber = 0;
        tileCount.text = tileCountNumber.ToString();
        selectedTileId = 1;

        ResetTileSprite();
        GenerateDynamicButtonForSideMenu();

        createdTiles = new List<GameObject>();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    #region SIDE MENU FUNCTONS
    private void GenerateDynamicButtonForSideMenu()
    {
        int dbLength = 3;// Databse.Instance.GetDatabaseLength();

        for (int i = 0; i < dbLength; i++)
        {
            Button btn = Instantiate(tileButtonPrefab, tileButtonsParent);
            btn.GetComponent<Image>().sprite = Resources.Load<Sprite>("Tiles/" + (i + 1));
            btn.GetComponent<TileButtonId>().SetTileId(i + 1);
        }

        Databse.Instance.CloseDbConnection(Databse.Instance.dbconn);
    }

    public void ChangeTileSize(int val)
    {
        float size = 0;

        //30cm * 30cm
        if(val == 0)
        {
            size = 0.3f;
        }
        //60cm * 60cm
        else if(val == 1)
        {
            size = 0.6f;
        }
        //80cm * 80cm
        else if (val == 2)
        {
            size = 0.8f;
        }
        //1m * 1m
        else if (val == 3)
        {
            size = 1f;
        }

        for(int i = 0; i < tilesParent.transform.childCount; i++)
        {
            tilesParent.transform.GetChild(i).localScale = new Vector2(size, size);
        }
    }
    
    internal void SetTileSprite(int tileId)
    {
        if (tilesParent.childCount > 0)
        {
            selectedTileId = tileId;

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
            sideNavigation.localPosition = sideNavigation.localPosition + new Vector3(10000, 0, 0);
            sideMenuClickedCount = 1;
        }
        //Move Out
        else if (sideMenuClickedCount == 1)
        {
            sideNavigation.localPosition = sideNavigation.localPosition + new Vector3(-10000, 0, 0);
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
    
    public void AddNewTileSprite()
    {
        try
        {
            //Open Gallery
            AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
            AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");

            intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_GET_CONTENT"));

            AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
            AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", "content://media/internal/images/media");
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);

            intentObject.Call<AndroidJavaObject>("setType", "image/jpeg");
            AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
            currentActivity.Call("startActivity", intentObject);

            //Select Tile Images
            //TODO:Add algorithm to select images from gallery
        }
        catch (Exception ex)
        {
            Debug.LogError("Open gallery error" + ex);
        }
    }
    #endregion

    #region COMMON FUNCTIONS
    internal void IncreaseTileCount()
    {
        tileCountNumber++;
        tileCount.text = tileCountNumber.ToString();
    }
    
    internal void AddTilesIntoList(GameObject tile)
    {
        createdTiles.Add(tile);
    }

    public void RemoveAllTiles()
    {
        for(int i = 0; i < createdTiles.Count; i++)
        {
            Destroy(createdTiles[i]);
        }

        createdTiles.Clear();

        tileCountNumber = 0;
        tileCount.text = "0";
    }
    #endregion
}
