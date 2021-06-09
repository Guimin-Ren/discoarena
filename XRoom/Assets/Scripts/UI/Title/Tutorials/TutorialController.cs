using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour
{
    public TutorialPage tutorialPage;
    // tutorial index
    public int index = 0;

    public Image image;

    // img list
    public Sprite[] tutorialImgs;

    // Start is called before the first frame update
    void Start()
    {
        tutorialPage =  GetComponentInChildren<TutorialPage>(true);
        image = tutorialPage.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // open tutorial page
    public void OpenTutorial()
    {
        tutorialPage.gameObject.SetActive(true);
        image.sprite = tutorialImgs[index];
    }

    // open next page
    public void GoNext()
    {
        index++;
        if(index >= tutorialImgs.Length)
        {
            index = 0;
        }
        image.sprite = tutorialImgs[index];
    }

    // open previous page
    public void GoPrevious()
    {
        index--;
        if (index >= -1)
        {
            index = 0;
        }
        image.sprite = tutorialImgs[index];
    }

    // close tutorial page
    public void ClearTutorials()
    {
        tutorialPage.gameObject.SetActive(false);
        index = 0;
    }

}
