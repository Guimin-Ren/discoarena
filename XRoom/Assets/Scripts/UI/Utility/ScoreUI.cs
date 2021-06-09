using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour
{
    public bool isSelf = true;

    public ScoreManagerNetwork scoreManagerNetwork;

    Text text;

    public PlayerStatus ps;

    public string savedScore;

    RectTransform rectTransform;

    //0: normal
    //1: shinning
    public Font[] fonts;

    public float effectTime = 1.0f;

    // Start is called before the first frame update
    void Start()
    {      
        text = GetComponent<Text>();
        rectTransform = GetComponent<RectTransform>();
        StartCoroutine(WaitPlayers());
    }

    // Update is called once per frame
    void Update()
    {
        if(ps!= null)
        {
            text.text = ps.score.ToString();
            if (savedScore != text.text)
            {
                savedScore = text.text;
                StartCoroutine(ScoreEffectIE(effectTime));      
            }   
        }
    }

    IEnumerator ScoreEffectIE(float effectTime)
    {
        // scale: 1 -> 2
        float timer = 0;
        text.font = fonts[1];
        while(timer < effectTime)
        {
            timer += Time.deltaTime;
            float scale = 1 + timer / effectTime;
            rectTransform.parent.localScale = new Vector3(scale, scale, scale);
            yield return new WaitForEndOfFrame();
        }

        // scale: 2 -> 1
        timer = 0;
        while (timer < effectTime)
        {
            timer += Time.deltaTime;
            float scale = 2 - timer / effectTime;
            rectTransform.parent.localScale = new Vector3(scale, scale, scale);
            yield return new WaitForEndOfFrame();
        }
        text.font = fonts[0];
    }

    IEnumerator WaitPlayers()
    {
        yield return new WaitUntil(() => FindObjectOfType<ScoreManagerNetwork>() != null);
        scoreManagerNetwork = FindObjectOfType<ScoreManagerNetwork>();
        yield return new WaitUntil(() => scoreManagerNetwork.isLocalSet == true);
        PlayerStatus[] players = scoreManagerNetwork.playerStatuses;
        foreach(PlayerStatus playerStatus in players)
        {
            if(isSelf && playerStatus.GetComponent<NetworkIdentity>().isLocalPlayer)
            {
                ps = playerStatus;
                break;
            }
            else if(!isSelf && !playerStatus.GetComponent<NetworkIdentity>().isLocalPlayer)
            {
                ps = playerStatus;
                break;
            }
        }
    }

}
