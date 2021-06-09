using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/*script to calculateing position of light source in client space*/
public class RelativeDiscoLightTranforms : NetworkBehaviour
{
    // Start is called before the first frame update
   public GameObject genLight;

   [SyncVar] public Vector3 pos;
   [SyncVar] public Quaternion rot;
   
    
    public Transform markerTrans;
    // Start is called before the first frame update
    void Start()
    {
         StartCoroutine(FindMarker());
    }

    // Update is called once per frame
    void Update()
    {
        if(markerTrans != null){
            transform.position = markerTrans.position + markerTrans.TransformDirection(pos);
            transform.rotation = markerTrans.rotation * rot;
        
        }
    }

    IEnumerator FindMarker(){
        yield return new WaitUntil(()=> FindObjectOfType<MarkerIdentity>() != null);
        markerTrans = FindObjectOfType<MarkerIdentity>().transform;
    }
}
