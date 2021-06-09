using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


/*script used to record the player status and scores*/
public class PlayerStatus : NetworkBehaviour
{
    [SyncVar]
    public int score = 0;

    [SyncVar]
    public int hit = 0;

    [SyncVar]
    public int index = 0;

    [SyncVar]
    public bool isReceive = false;

    public bool isReady = false;

    public delegate void GetHit();

    public event GetHit OnGetHitServer;

    public event GetHit OnGetHitClient;

    [ServerCallback]
    // Start is called before the first frame update
    void Start()
    {
    
    }

    [ClientCallback]
    // Update is called once per frame
    void Update()
    {
        SetClientStatusOnServer(isReady);
    }

    [Server]
    public void HitPlayer(int value)
    {
        ChangeScore(value);
        OnGetHitServer?.Invoke();
        hit++;
    }

    [Server]
    public void ChangeScore(int value)
    {
        score += value;
    }

    [ClientCallback]
    public void SetClientStatusOnServer(bool status)
    {
        CmdSetClientStatusOnServer(status);
    }

    [Command]
    public void CmdSetClientStatusOnServer(bool status)
    {
        isReady = status;
    }
}
