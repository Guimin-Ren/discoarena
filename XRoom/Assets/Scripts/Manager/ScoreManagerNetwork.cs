using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ScoreManagerNetwork : NetworkBehaviour
{
    NetworkFlashLight networkManager;

    public PlayerStatus[] playerStatuses;

    [SyncVar]
    public bool isInitialized = false;

    public bool isLocalSet = false;


    void Start()
    {
        networkManager = GetComponentInParent<NetworkFlashLight>();

        if (netIdentity.isClient)
        {
            InitialClient();
        }
        else
        {
            InitialServer();
        }

    }
    [ServerCallback]
    public void InitialServer()
    {
        StartCoroutine(InitializeServerIE());
    }

    [ClientCallback]
    public void InitialClient()
    {
        StartCoroutine(InitializeClientIE());
    }

    [ServerCallback]
    public void ResetAllScores()
    {
        foreach (PlayerStatus ps in playerStatuses)
        {
            ps.score = 0;
        }
    }

    [ServerCallback]
    IEnumerator InitializeServerIE()
    {
        yield return new WaitUntil(()=>networkManager.numReady == true);
        
        isInitialized = true;

        playerStatuses = FindObjectsOfType<PlayerStatus>();

        isLocalSet = true;
    }

    [ClientCallback]
    IEnumerator InitializeClientIE()
    {
        yield return new WaitUntil(() => isInitialized == true);

        playerStatuses = FindObjectsOfType<PlayerStatus>();

        isLocalSet = true;
    }

}
