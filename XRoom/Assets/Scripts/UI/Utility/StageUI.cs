using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageUI : MonoBehaviour
{
    Text text;

    public RoundManagerNet roundManager;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
        StartCoroutine(FindGameManagerIE());
    }

    // Update is called once per frame
    void Update()
    {
        if (roundManager != null)
        {
            if (!roundManager.stageReady)
            {
                text.enabled = true;
            }
            else
            {
                text.enabled = false;
            }
        }
    }

    IEnumerator FindGameManagerIE()
    {
        yield return new WaitUntil(() => FindObjectOfType<RoundManagerNet>() != null);
        roundManager = FindObjectOfType<RoundManagerNet>();
    }
}
