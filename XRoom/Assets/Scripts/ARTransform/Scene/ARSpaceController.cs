using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARSpaceController : MonoBehaviour
{
    public GameObject lightSource;
    public GameObject target;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetLightSource(){
        lightSource.SetActive(true);
    }

}
