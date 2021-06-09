using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;
using UnityEngine;
using Mirror;

public class RelativeTransformRespawner : NetworkBehaviour
{
    // prefab for generation
    public GameObject prefab;

    public GameObject placedMirror;
    
    // generated object
    public GameObject genPlayer;
    
    // marker transform
    public Transform markerTrans;

    [SerializeField] private float placedMirrorOffSet = 0.5f;
    
    public GameObject GenPlacedMirror;
    
    public bool hasNewPMirror = false;

    // Start is called before the first frame update
    void Start()
    {    
        SpawnGenPlayer();
        StartCoroutine(FindMarker());
    }

    // Update is called once per frame
    void Update()
    {
        if(markerTrans != null && genPlayer != null){
            // set player relative position and rotation
            genPlayer.transform.position = markerTrans.position + markerTrans.TransformDirection(transform.position);
            genPlayer.transform.rotation = markerTrans.rotation * transform.rotation;
        
        }

        if(markerTrans != null && GenPlacedMirror != null && hasNewPMirror){
            // set player relative position and rotation
            GenPlacedMirror.transform.position = markerTrans.position + markerTrans.TransformDirection(transform.position);
            GenPlacedMirror.transform.position += transform.forward * placedMirrorOffSet;
            GenPlacedMirror.transform.rotation = markerTrans.rotation * transform.rotation;
            hasNewPMirror = false;
        
        }
    }

    IEnumerator FindMarker(){
        yield return new WaitUntil(()=> FindObjectOfType<MarkerIdentity>() != null);
        markerTrans = FindObjectOfType<MarkerIdentity>().transform;
    }

    // command for server respawn generated player pivot and mirror
    [Command]
    public void SpawnGenPlayer(){
        GameObject tmp = Instantiate(prefab);
        tmp.GetComponent<MirrorBehavior>().owner = gameObject;
        tmp.GetComponent<MirrorBehavior>().isFirstPhone = true;
        NetworkServer.Spawn(tmp); 
    }

    //call this to generate a placed mirror on server
    [ServerCallback]
    public void SpawmPlacedMirror() {
        GameObject pMirror = Instantiate(placedMirror);
       
        pMirror.GetComponent<MirrorBehavior>().owner = gameObject;
        pMirror.GetComponent<MirrorBehavior>().isFirstPhone = false;
        NetworkServer.Spawn(pMirror);
        
    }
}
