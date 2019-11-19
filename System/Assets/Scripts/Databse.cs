using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System;
using System.IO;

public class Databse : MonoBehaviour
{
    public static Databse Instance { get; private set; }

    private string conn;
    private IDbConnection dbconn;
    private IDataReader reader;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        conn = "URI=file:" + Application.dataPath + "/On_the_floor.s3db";
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open();
    }

    internal byte[] LoadImage()
    {
        IDbCommand dbcmd = dbconn.CreateCommand();
        string sqlQuery = "SELECT image FROM TileList";
        dbcmd.CommandText = sqlQuery;

        byte[] tileImagesBytes = null;

        try
        {

            reader = dbcmd.ExecuteReader();

            while (reader.Read())
            {
                tileImagesBytes = (System.Byte[])reader[0];
            }

            dbcmd.Dispose();
            dbcmd = null;
        }
        catch (Exception e)
        {
            Debug.LogError("Load Image Error: " + e);
        }


        return tileImagesBytes;
    }

    public void RemoveTileImage(string Delete_by_id)
    {
        using (dbconn = new SqliteConnection(conn))
        {
            IDbCommand dbcmd = dbconn.CreateCommand();

            string sqlQuery = "DELETE FROM TileList where id =" + Delete_by_id;
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();

            dbcmd.Dispose();
            dbcmd = null;
        }
    }

    internal int GetDatabaseLength()
    {
        IDbCommand dbcmd = dbconn.CreateCommand();
        string sqlQuery = "SELECT * FROM TileList";
        dbcmd.CommandText = sqlQuery;
        reader = dbcmd.ExecuteReader();

        int length = reader.FieldCount - 1;

        dbcmd.Dispose();
        dbcmd = null;

        return length;
    }
    
    internal void CloseDatabse()
    {
        reader.Close();
        reader = null;

        dbconn.Close();
        dbconn = null;
    }
}
