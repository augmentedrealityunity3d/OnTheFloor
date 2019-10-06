using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerConnectionObject : NetworkBehaviour
{
    public GameObject playerUnitPrefab;

    [SyncVar (hook = "OnPlayerNameChange")]
    public string playerName = "Anonymous";

    void Start()
    {
        if(isLocalPlayer == false)
        {
            return;
        }

        CmdSpawnMyUnit();
    }

    void Update()
    {
        if (isLocalPlayer == false)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            CmdSpawnMyUnit();
        }

        if (Input.GetKey(KeyCode.Q))
        {
            string n = "Quill" + Random.Range(1, 100);
            Debug.Log("Request Server to change the player Name from " + playerName);
            CmdChangePlayerName(n);
        }
    }

    private void OnPlayerNameChange(string newName)
    {
        Debug.Log("Old Name: " + playerName + " & New name: " + newName);
        //playerName = newName;
        gameObject.name = "PlayerConnectionObject [" + newName +"]";
    }

    [Command]
    private void CmdSpawnMyUnit()
    {
        GameObject go = Instantiate(playerUnitPrefab);
        NetworkServer.SpawnWithClientAuthority(go, connectionToClient);
    }

    [Command]
    private void CmdChangePlayerName(string s)
    {
        playerName = s;
        Debug.Log("Player Name change to " + playerName);

        //RpcChangeClientName(playerName);
    }

    //[ClientRpc]
    //private void RpcChangeClientName(string n)
    //{
    //    Debug.Log("Mr." + n + ". We are request to change your name");
    //    playerName = n;
    //}
}
