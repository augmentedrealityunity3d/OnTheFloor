using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileButtonId : MonoBehaviour
{
    internal int tileId;

    internal void SetTileId(int id)
    {
        tileId = id;
    }

    public void CallSetTileSprite()
    {
        OnTheFloorManager.Instance.SetTileSprite(tileId);
    }
}
