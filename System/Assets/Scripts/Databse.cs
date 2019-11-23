using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System;
using System.IO;

public class Databse : MonoBehaviour
{
    public static Databse Instance { get; private set; }

    private string conn;

    internal IDbConnection dbconn;
    private IDataReader reader;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        conn = "URI=file:" + Application.dataPath + "/On_the_floor.s3db";

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

    //internal byte[] LoadImage()
    //{
    //    IDbCommand dbcmd = dbconn.CreateCommand();
    //    string sqlQuery = "SELECT image FROM TileList";
    //    dbcmd.CommandText = sqlQuery;

    //    byte[] tileImagesBytes = null;

    //    try
    //    {

    //        reader = dbcmd.ExecuteReader();

    //        while (reader.Read())
    //        {
    //            tileImagesBytes = (System.Byte[])reader[0];
    //        }

    //        dbcmd.Dispose();
    //        dbcmd = null;
    //    }
    //    catch (Exception e)
    //    {
    //        Debug.LogError("Load Image Error: " + e);
    //    }


    //    return tileImagesBytes;
    //}

    public void DeleteTileImage(string id)
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
                        dbcmd.CommandText = string.Format("DELETE FROM TileList WHERE id = \"{0}\"", id);
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

    //internal int GetDatabaseLength()
    //{
    //    IDbCommand dbcmd = dbconn.CreateCommand();
    //    string sqlQuery = "SELECT * FROM TileList";
    //    dbcmd.CommandText = sqlQuery;
    //    reader = dbcmd.ExecuteReader();

    //    int length = reader.FieldCount - 1;

    //    dbcmd.Dispose();
    //    dbcmd = null;

    //    return length;
    //}

    internal void CloseDbConnection(IDbConnection dbconn)
    {
        dbconn.Close();
    }
}
