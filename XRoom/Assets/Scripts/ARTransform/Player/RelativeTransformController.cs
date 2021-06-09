using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Mirror;

public struct PosAndRot{
    public Vector3 pos;
    public Quaternion rot;
}

public class RelativeTransformController : NetworkBehaviour
{
    // transform for camera
    public Transform camTransform;

    Vector3 markerPos;

    Quaternion markerRot;

    Vector3 playerPos;

    Quaternion playerRot;

    Quaternion rotRelative;

    Vector3 posRelative;
    
    public bool markerSet = false;

    Transform markerTrans;

    public PosAndRot posAndRotRelative;
    

    void Start()
    {
        camTransform = FindObjectOfType<ARCameraManager>().transform;
        StartCoroutine(GetRelativeTransform());
    }

    // Update is called once per frame
    void Update()
    {
        // relative transform calculation
        if(markerSet && netIdentity.isClient && netIdentity.isLocalPlayer){
            GetRelativePosAndRot(markerTrans, camTransform, out posAndRotRelative);
            transform.position = posAndRotRelative.pos;
            transform.rotation = posAndRotRelative.rot;
        }
    }

    public void GetRelativePosAndRot(Transform transMarker, Transform tranObject, out PosAndRot posAndRot){
        Vector3 markerPos = transMarker.position;
        Quaternion markerRot = transMarker.rotation;

        Vector3 playerPos = tranObject.position;
        Quaternion playerRot = tranObject.rotation;

        Vector3 posRelative = playerPos - markerPos;
            
        posRelative = transMarker.InverseTransformDirection(posRelative);
        Quaternion rotRelative = Quaternion.Inverse(markerRot) * playerRot;

        posAndRot.pos = posRelative;
        posAndRot.rot = rotRelative;
    }

    public void SetRelativePosAndRot(Transform transMarker, Transform tranObject, out PosAndRot posAndRot){
        Vector3 markerPos = transMarker.position;
        Quaternion markerRot = transMarker.rotation;

        Vector3 playerPos = tranObject.position;
        Quaternion playerRot = tranObject.rotation;

        Vector3 posRelative = transMarker.TransformDirection(playerPos) + markerPos;
        Quaternion rotRelative = markerRot * playerRot;

        posAndRot.pos = posRelative;
        posAndRot.rot = rotRelative;
    }


    IEnumerator GetRelativeTransform(){
        yield return new WaitUntil(() => FindObjectOfType<MarkerIdentity>() != null);
        if(gameObject.activeSelf == false){
            yield return null;
        }

        if(netIdentity.isServer){
            yield return null;
        }
        
        markerTrans = FindObjectOfType<MarkerIdentity>().transform;

        markerPos = markerTrans.position;
        markerRot = markerTrans.rotation;
        playerPos = camTransform.position;
        playerRot = camTransform.rotation;

        posRelative = playerPos - markerPos;

        posRelative = markerTrans.InverseTransformDirection(posRelative);
        rotRelative = Quaternion.Inverse(markerRot) * playerRot;

        posAndRotRelative.pos = posRelative;
        posAndRotRelative.rot = rotRelative;

        markerSet = true;
    }

}

public class HelperFunction : MonoBehaviour{
    public static void GetRelativePosAndRot(Transform transMarker, Transform tranObject, out PosAndRot posAndRot){
        Vector3 markerPos = transMarker.position;
        Quaternion markerRot = transMarker.rotation;

        Vector3 playerPos = tranObject.position;
        Quaternion playerRot = tranObject.rotation;

        Vector3 posRelative = playerPos - markerPos;
            
        posRelative = transMarker.InverseTransformDirection(posRelative);
        Quaternion rotRelative = Quaternion.Inverse(markerRot) * playerRot;

        posAndRot.pos = posRelative;
        posAndRot.rot = rotRelative;
        //return posAndRot;
    }

    public static void SetRelativePosAndRot(Transform transMarker, Transform tranObject, out PosAndRot posAndRot){
        Vector3 markerPos = transMarker.position;
        Quaternion markerRot = transMarker.rotation;

        Vector3 playerPos = tranObject.position;
        Quaternion playerRot = tranObject.rotation;

        Vector3 posRelative = transMarker.TransformDirection(playerPos) + markerPos;
        Quaternion rotRelative = markerRot * playerRot;

        posAndRot.pos = posRelative;
        posAndRot.rot = rotRelative;
    }
}