using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelativeTransformSetter : MonoBehaviour
{
    PosAndRot posAndRot;

    Transform pivot;

    Transform markerTransform;

    // Start is called before the first frame update
    void Start()
    {
        posAndRot = new PosAndRot();
        pivot = transform.parent;
        StartCoroutine(FindMarker());
    }

    // Update is called once per frame
    void Update()
    {
        if(markerTransform != null){
            HelperFunction.SetRelativePosAndRot( markerTransform, pivot, out posAndRot);
            transform.position = posAndRot.pos;
            transform.rotation = posAndRot.rot;
        }
    }
    
    IEnumerator FindMarker(){
        yield return new WaitUntil(()=> FindObjectOfType<MarkerIdentity>() != null);
        markerTransform = FindObjectOfType<MarkerIdentity>().transform;
    }
}
