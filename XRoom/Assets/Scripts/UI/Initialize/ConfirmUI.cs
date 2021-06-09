using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ConfirmUI : MonoBehaviour
{
    //public PlayerStatus ps;
    public ScoreManagerNetwork scoreManagerNetwork;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPlayerReady()
    {
        StartCoroutine(FindPlayerAndSet());
    }

    IEnumerator FindPlayerAndSet()
    {
        yield return new WaitUntil(() => FindObjectOfType<ScoreManagerNetwork>() != null);
        scoreManagerNetwork = FindObjectOfType<ScoreManagerNetwork>();
        yield return new WaitUntil(() => scoreManagerNetwork.isLocalSet == true);
        PlayerStatus[] players = scoreManagerNetwork.playerStatuses;
        foreach (PlayerStatus playerStatus in players)
        {
            if (playerStatus.GetComponent<NetworkIdentity>().isLocalPlayer)
            {
                playerStatus.isReady = true;
                break;
            }
        }
    }
}
