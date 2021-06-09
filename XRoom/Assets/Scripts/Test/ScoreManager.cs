using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static ScoreManager instance;
    //text field for player 2
    [SerializeField] private Text score1Text;
    //Text field for player 2
    [SerializeField] private Text score2Text;
    private int currScore1;
    private int currScore2;
    void Awake() {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
            return;
        } else {
            instance = this;
        }
    }
    void Start()
    {
        currScore1 = 0;
        currScore2 = 0;
        score1Text.text = currScore1.ToString();
        score2Text.text = currScore2.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    //call this when player 1 get scores.
    public void GetPoint1(int score) {
        currScore1 += score;
        score1Text.text = currScore1.ToString();
    }

    //call this when player1 lose scores
    public void LosePoint1(int score) {
        currScore1 -= score;
        if (currScore1 < 0) {
            currScore1 = 0;
        }
        score1Text.text = currScore1.ToString();
    }
    
    //call thie when player two get scores
    public void GetPoint2(int score) {
        currScore2 += score;
        score2Text.text = currScore2.ToString();
    }

    //call thie when player two lose scores
    public void LosePoint2(int score) {
        if (currScore2 < 0) {
            currScore2 = 0;
        }          
        currScore2 -= score;
        score2Text.text = currScore2.ToString();
    }
}
