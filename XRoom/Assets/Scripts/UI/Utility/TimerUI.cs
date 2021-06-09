using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour
{
    Image image;

    public RoundManagerNet roundManager;

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        StartCoroutine(FindGameManagerIE());
    }

    // Update is called once per frame
    void Update()
    {
        if(roundManager != null){
            image.fillAmount = roundManager.timerRatio;
        }
    }

    IEnumerator FindGameManagerIE()
    {
        yield return new WaitUntil(() => FindObjectOfType<RoundManagerNet>() != null);
        roundManager = FindObjectOfType<RoundManagerNet>();
    }
}
