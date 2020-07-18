using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System;
using System.IO;
using TMPro;

public class Databse : MonoBehaviour
{
    public static Databse Instance { get; private set; }

    private string conn;
    internal IDbConnection dbconn;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        conn = "URI=file:" + Application.dataPath + "/Assets/On_the_floor.s3db";

        try
        {
            using (dbconn = new SqliteConnection(conn))
            {
                dbconn.Open();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Tiles Load Connection Error: " + ex);
        }
    }

    public void DeleteTileImage()
    {
        try
        {
            using (IDbConnection dbconnection = new SqliteConnection(conn))
            {
                dbconnection.Open();

                try
                {
                    using (IDbCommand dbcmd = dbconnection.CreateCommand())
                    {
                        dbcmd.CommandText = string.Format("DELETE FROM TileList WHERE id = \"{0}\"", OnTheFloorManager.Instance.selectedTileId);
                        dbcmd.ExecuteScalar();
                        Debug.Log("Tile Succesfully Deleted");
                    }
                }
                catch(Exception ex)
                {
                    Debug.LogError("Delete Db Command Error: " + ex);
                }
                finally
                {
                    dbconnection.Close();
                }
            }
        }
        catch(Exception ex)
        {
            Debug.LogError("Delete Db Connection Error: " + ex);
        }
    }

    internal void CloseDbConnection(IDbConnection dbconn)
    {
        dbconn.Close();
    }
}
