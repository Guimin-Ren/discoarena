using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARMarkerAdjustController : MonoBehaviour
{
    public Vector3 rotateOffset = Vector3.zero;

    public float adjustSpeed = 20.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AdjustRotation(bool left){
        if(left){
            rotateOffset.y -= adjustSpeed * Time.deltaTime;
        }
        else{
            rotateOffset.y += adjustSpeed * Time.deltaTime;
        }
    }
}
