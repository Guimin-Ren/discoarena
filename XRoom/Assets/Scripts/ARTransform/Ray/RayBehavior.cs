using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RayBehavior : NetworkBehaviour
{
    // source of ray
    [SyncVar]
    public GameObject source;

    // hit target of ray
    [SyncVar]
    public GameObject target;

    // direction of ray
    [SyncVar]
    public Vector3 dir;

    // mirror of reflecting the ray
    [SyncVar]
    public GameObject mirror;

    // index for identifying ray cluster
    [SyncVar]
    public int index;

    // index of owner player
    [SyncVar]
    public int ownerIndex = 0;

    // flag for bulb ray check
    [SyncVar]
    public bool isBulb = false;
    
    // previous hit object
    public GameObject savedObject;

    // mirror behavior of target mirror
    public MirrorBehavior mirrorBehavior;

    // next mirror of hit
    public MirrorBehavior nextMirror;

    // temporary help variable
    public MirrorBehavior tmp;

    // flag for check player first mirror
    [SyncVar]
    public bool isDirectAiming = false;

    // position of intersection point
    [SyncVar]
    public Vector3 intersectPoint;

    // round manager for cast valid status check
    public RoundManagerNet roundManagerNet;

    // distance for last hit distance
    [SyncVar]
    public float tmpDistance = 5;


    [ServerCallback]
    void Start()
    {      
        if(mirror!=null){
            mirrorBehavior = mirror.GetComponent<MirrorBehavior>();
            mirrorBehavior.rayBehaviors.Add(this);
        }
        roundManagerNet = FindObjectOfType<RoundManagerNet>();     
    }

    // Destory this ray
    [ServerCallback]
    void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }    

    // Hit target main function
    [ServerCallback]
    public void HitTarget(bool hit, GameObject hitObject, Vector3 intersectPoint){
        if(hit){
            if (isBulb)
            {
                if (savedObject == null)
                {

                    // update savedObj
                    // only run if hitobject is the phone mirror
                    savedObject = hitObject;
                    nextMirror = savedObject.GetComponent<MirrorBehavior>();
                    nextMirror.intersectPos = intersectPoint;
                    tmp = savedObject.GetComponent<MirrorBehavior>();
                    // add new ray
                    tmp.ManipulateReflectRay(true, nextMirror.gameObject, index);
                    nextMirror.sources.Add(source);
                }
                else
                {
                    if (hitObject == savedObject)
                    {
                        return;
                    }
                    else
                    {
                        // notify savedObj not receiving
                        tmp = savedObject.GetComponent<MirrorBehavior>();
                        tmp.ManipulateReflectRay(false, null, index);
                        // update savedObj
                        savedObject = hitObject;
                        nextMirror = savedObject.GetComponent<MirrorBehavior>();
                        nextMirror.intersectPos = intersectPoint;
                        tmp = savedObject.GetComponent<MirrorBehavior>();
                        // add new ray
                        tmp.ManipulateReflectRay(true, nextMirror.gameObject, index);
                        nextMirror.sources.Add(source);
                    }
                }
            }
            // hit and this is not bulb
            else{
                // hit self
                if(mirrorBehavior.gameObject == hitObject){           
                    return;
                }
                // hit other
                else{
                    // next is null for previous hit
                    if (nextMirror == null)
                    {
                        nextMirror = hitObject.GetComponent<MirrorBehavior>();
                        if(nextMirror.owner != mirrorBehavior.owner)
                        {
                            if (nextMirror.isFirstPhone)
                            {
                                if (roundManagerNet.canHit)
                                {
                                    roundManagerNet.canHit = false;
                                    roundManagerNet.timer = 0;
                                    nextMirror.owner.GetComponent<PlayerStatus>().HitPlayer(1);
                                }
                            }                   
                        }
                        // hit owner mirror
                        else
                        {
                            // update savedObj
                            savedObject = hitObject;
                            //nextMirror = savedObject.GetComponent<MirrorBehavior>();
                            tmp = savedObject.GetComponent<MirrorBehavior>();
                            // add new ray
                            tmp.ManipulateReflectRay(true, nextMirror.gameObject, index);
                            nextMirror.sources.Add(source);
                        }
                    }
                    // have next mirror before
                    else
                    {
                        // same target
                        if (hitObject == savedObject)
                        {
                            return;
                        }
                        // new hit target
                        else
                        {
                            nextMirror = hitObject.GetComponent<MirrorBehavior>();
                            // enemy mirror
                            if (nextMirror.owner != mirrorBehavior.owner)
                            {
                                if (nextMirror.isFirstPhone)
                                {
                                    if (roundManagerNet.canHit)
                                    {
                                        roundManagerNet.canHit = false;
                                        roundManagerNet.timer = 0;
                                        nextMirror.owner.GetComponent<PlayerStatus>().HitPlayer(1);
                                    }                                        
                                }
                            }
                            // hit own mirror
                            else
                            {
                                // notify savedObj not receiving
                                tmp = savedObject.GetComponent<MirrorBehavior>();
                                tmp.ManipulateReflectRay(false, null, index);
                                // update savedObj
                                savedObject = hitObject;
                                nextMirror = savedObject.GetComponent<MirrorBehavior>();
                                tmp = savedObject.GetComponent<MirrorBehavior>();
                                // add new ray
                                tmp.ManipulateReflectRay(true, nextMirror.gameObject, index);
                                nextMirror.sources.Add(source);
                            }         
                        }
                    }
                }
            }           
        }else{
            nextMirror = null;
            if(savedObject == null){
                return;
            }
            else{
                // notify savedObj not receiving this ray
                tmp = savedObject.GetComponent<MirrorBehavior>();
                tmp.ManipulateReflectRay(false, null, index);
                nextMirror = null;
                savedObject = null;
            }
        }
    }
}
