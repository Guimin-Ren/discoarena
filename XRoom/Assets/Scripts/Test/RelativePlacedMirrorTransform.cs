using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


/*script to calculateing position of placed mirror in client space*/
public class RelativePlacedMirrorTransform : NetworkBehaviour
{
    public GameObject genPlayer;

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
        if(markerTrans != null && genPlayer != null){
            // set player relative position and rotation
            
            transform.position = markerTrans.position + markerTrans.TransformDirection(pos);
            transform.rotation = markerTrans.rotation * rot;
        
        }
    }

    IEnumerator FindMarker(){
        yield return new WaitUntil(()=> FindObjectOfType<MarkerIdentity>() != null);
        markerTrans = FindObjectOfType<MarkerIdentity>().transform;
    }
}
