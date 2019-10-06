using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerUnit : NetworkBehaviour
{
    private Vector3 velocity;
    private Vector3 bestGuessPosition;
    float ourLatency; //How many seconds it takes to receive a one -way message
    float latencySmoothingFactor = 10; //The higher the value, the faster our local position will match the best guess position

    void Start()
    {
        
    }

    void Update()
    {
        if (hasAuthority == false)
        {
            bestGuessPosition = bestGuessPosition + (velocity * Time.deltaTime);
            transform.position = Vector3.Lerp(transform.position, bestGuessPosition, Time.deltaTime * latencySmoothingFactor); //To move smoothly without TELEPORTING
            return;
        }

        transform.Translate(velocity * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            this.transform.Translate(0, 1, 0);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            Destroy(gameObject);
        }

        if (true)
        {
            velocity = new Vector3(1, 0, 0);
            CmdChangeVelocity(velocity, transform.position);
        }
    }

    [Command]
    private void CmdChangeVelocity(Vector3 v, Vector3 p)
    {
        velocity = v;
        transform.position = p;
        RpcUpdateVelocity(velocity, transform.position);
    }

    [ClientRpc]
    private void RpcUpdateVelocity(Vector3 v, Vector3 p)
    {
        if (hasAuthority)
        {
            return;
        }

        velocity = v;
        bestGuessPosition = p + (velocity * ourLatency);
    }
}
