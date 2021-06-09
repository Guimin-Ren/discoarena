using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class ResultUI : MonoBehaviour
{
    public Image winner;

    public Image loser;

    public Text draw;

    public Text scoreSelf;

    public Text scoreEnemy;

    public Image roundOver;

    public Image prep1;
    public Image prep2;
    public Image prep3;
    public Image prepGo;

    public RoundManagerNet roundManager;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FindGameManagerIE());
    }

    // Update is called once per frame
    void Update()
    {
        if(roundManager != null)
        {
            if (roundManager.roundOver)
            {
                roundOver.enabled = true;
            }
            else
            {
                roundOver.enabled = false;
            }

            switch (roundManager.prepStatus)
            {
                case PrepStatus.three:
                    ClearPrep();
                    prep3.enabled = true;
                    break;
                case PrepStatus.two:
                    ClearPrep();
                    prep2.enabled = true;
                    break;
                case PrepStatus.one:
                    ClearPrep();
                    prep1.enabled = true;
                    break;
                case PrepStatus.go:
                    ClearPrep();
                    prepGo.enabled = true;
                    break;
                default:
                    ClearPrep();
                    break;
            }

            if (roundManager.gameOver)
            {
                if(float.Parse(scoreSelf.text) > float.Parse(scoreEnemy.text))
                {
                    winner.enabled = true;
                    loser.enabled = false;
                    draw.enabled = false;

                }
                else if(float.Parse(scoreSelf.text) == float.Parse(scoreEnemy.text))
                {
                    winner.enabled = false;
                    loser.enabled = false;
                    draw.enabled = true;
                }
                else
                {
                    winner.enabled = false;
                    loser.enabled = false;
                    loser.enabled = true;
                }
            }
        }
    }

    IEnumerator FindGameManagerIE()
    {
        yield return new WaitUntil(() => FindObjectOfType<RoundManagerNet>() != null);
        roundManager = FindObjectOfType<RoundManagerNet>();
    }

    public void ClearPrep()
    {
        prep1.enabled = false;
        prep2.enabled = false;
        prep3.enabled = false;
        prepGo.enabled = false;
    }
}
