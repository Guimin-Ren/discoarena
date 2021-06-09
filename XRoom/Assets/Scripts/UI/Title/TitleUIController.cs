using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleUIController : MonoBehaviour
{
    public Camera fakeCamera;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CloseTitle()
    {
        fakeCamera.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}
