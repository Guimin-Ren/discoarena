using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundController : MonoBehaviour
{
    public Image bgHit;

    public Image bgReceive;

    bool isFading = false;

    bool foundPlayer = false;

    public PlayerStatus ps;

    public int savedHitCount = 0;

    void Start()
    {
        StartCoroutine(FindPlayerSelf());
    }


    void Update()
    {
        if (foundPlayer)
        {
            if(ps.score > savedHitCount)
            {
                savedHitCount++;
                if (!isFading)
                {
                    StartCoroutine(FadeInAndOut(2.0f));
                }
            }

            if (ps.isReceive)
            {
                bgReceive.gameObject.SetActive(true);
            }
            else
            {
                bgReceive.gameObject.SetActive(false);
            }

        }
    }

    IEnumerator FindPlayerSelf()
    {
        yield return new WaitUntil(() => FindObjectOfType<RayManager>() != null);

        PlayerStatus[] playerStatuses = FindObjectsOfType<PlayerStatus>();
        foreach(PlayerStatus player in playerStatuses)
        {
            if (player.isLocalPlayer)
            {
                ps = player;
                break;
            }
        }
        foundPlayer = true;
    }

    // fade effect
    IEnumerator FadeInAndOut(float time)
    {
        isFading = true;
        float tmp = 0;
        while(tmp < time/2)
        {
            if(bgHit != null)
            {
                bgHit.color = new Color(bgHit.color.r, bgHit.color.g, bgHit.color.b, tmp / (time / 2 - 0.1f)); 
            }
            tmp += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        while (tmp < time)
        {
            if (bgHit != null)
            {
                bgHit.color = new Color(bgHit.color.r, bgHit.color.g, bgHit.color.b,  -2.4f * tmp / time + 2.3f);
            }
            tmp += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        isFading = false;
    }
}
