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

        ResetTileSprite();
        GenerateDynamicButtonForSideMenu();
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
            Databse.Instance.selectedTileId = tileId;

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
    #endregion

    #region COMMON FUNCTIONS
    internal void IncreaseTileCount()
    {
        tileCountNumber++;
        tileCount.text = tileCountNumber.ToString("0,0");
    }

    public void AddImageInDb()
    {
        List<string> results = new List<string>();
        HashSet<string> allowedExtesions = new HashSet<string>() { ".png", ".jpg", ".jpeg" };

        try
        {
            AndroidJavaClass mediaClass = new AndroidJavaClass("android.provider.MediaStore$Images$Media");

            const string dataTag = "_data";

            string[] projection = new string[] { dataTag };
            AndroidJavaClass player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = player.GetStatic<AndroidJavaObject>("currentActivity");

            string[] urisToSearch = new string[] { "EXTERNAL_CONTENT_URI", "INTERNAL_CONTENT_URI" };
            foreach (string uriToSearch in urisToSearch)
            {
                AndroidJavaObject externalUri = mediaClass.GetStatic<AndroidJavaObject>(uriToSearch);
                AndroidJavaObject finder = currentActivity.Call<AndroidJavaObject>("managedQuery", externalUri, projection, null, null, null);
                bool foundOne = finder.Call<bool>("moveToFirst");
                while (foundOne)
                {
                    int dataIndex = finder.Call<int>("getColumnIndex", dataTag);
                    string data = finder.Call<string>("getString", dataIndex);
                    if (allowedExtesions.Contains(Path.GetExtension(data).ToLower()))
                    {
                        string path = @"file:///" + data;
                        results.Add(path);
                    }

                    foundOne = finder.Call<bool>("moveToNext");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error try to call phone gallery: " + ex);
        }
    }
    #endregion
}
