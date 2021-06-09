using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/*each mirror's behavior is defined in this scripts. It will control every mirror to reflect and destroy the ray beening interactive
 mirror cool down time and limitation are also implemented in this script*/
public class MirrorBehavior : NetworkBehaviour
{
    // list of source of reflection rays
    [SyncVar]
    public List<GameObject> sources;

    // owner of mirror which should be players
    [SyncVar]
    public GameObject owner;
    
    // prefab for generating rays
    public GameObject rayPrefab;

    // list of created rayBehaviors
    public List<RayBehavior> rayBehaviors;

    // transform for ar marker
    public Transform markerTransform;

    // ray manager for all ray groups
    public RayManager rayManager;

    // paramenter to detect if this mirror is a player mirror or not
    // true: playermirror
    // false: placedmirror
    [SyncVar]
    public bool isFirstPhone = false;

    // player status info
    public PlayerStatus ps;

    // flag for reflection status check
    bool isUpdateCheck = false;

    // detection distance of raycast
    float aimDistance = 4.0f;

    // position of intersection point
    [SyncVar]
    public Vector3 intersectPos;

    // index for identifying mirror and ray groups
    [SyncVar]
    public int ownerIndex = 0;

    [SyncVar]
    public float MirrorLifeTimer;
    [SyncVar]
    public float MirrorRecoverTimer;
    [SyncVar]
    public bool canReflect = true;
    [SyncVar]
    public bool startTurnDown = false;
    [SyncVar]
    public bool startRecover = false;
    public float MirroLifeTime = 5;
    public float MirrorRecoverTime = 3;

    // Start is called before the first frame update
    void Start()
    {
        ps = owner.GetComponent<PlayerStatus>();
        if (isFirstPhone)
        {
            owner.GetComponent<RelativeTransformRespawner>().genPlayer = gameObject;          
        } else {
            owner.GetComponent<RelativeTransformRespawner>().GenPlacedMirror = gameObject;
            owner.GetComponent<RelativeTransformRespawner>().hasNewPMirror = true;
        }
        // assign player index
        ownerIndex = ps.index;
        StartCoroutine(FindMarker());
        StartCoroutine(FindRayManager());
        MirrorLifeTimer = MirroLifeTime;
        MirrorRecoverTimer = MirrorRecoverTime;
    }

    [ServerCallback]
    // Update is called once per frame
    void Update()
    {
        isUpdateCheck = false;
        //turn down mirror function 
        if (startTurnDown) {
            MirrorLifeTimer -= Time.deltaTime;
            if (MirrorLifeTimer <= 0) {
               
                MirrorLifeTimer = MirroLifeTime;
                
                // turn off the reflect function
                canReflect = false;
                // clear all reflect ray
                CmdDestroyAllReflectRay();
                // start recovery process
                startRecover = true;
                startTurnDown = false;
            }
        }
        if (startRecover) {
            MirrorRecoverTimer -= Time.deltaTime;
            startTurnDown = false;
            if (MirrorRecoverTimer <= 0) {
                
                MirrorRecoverTimer = MirrorRecoverTime;
                
                // turn on the reflect function
                canReflect = true;
                //close recovery process
                startRecover = false;
            }
        }
        if (markerTransform == null){
            return;
        }
        // for each source calculate reflection rays
        for (int i = sources.Count - 1; i >= 0; i--)
        {

            if(rayBehaviors.Count - 1 >= i && rayBehaviors[i] == null || sources[i] == null)
            {
                // Remove obsolete source and ray
                sources.RemoveAt(i);
                rayBehaviors.RemoveAt(i);
            }
            else if (rayBehaviors.Count - 1 >= i && rayBehaviors[i] && sources[i] != null)
            {
                Vector3 refl= new Vector3(0, 0, 0);
                if (!isFirstPhone)
                {
                    refl = ReflectNormal(sources[i].transform.position, transform.position, transform.forward);
                }
                else
                {
                    // convex aim
                    rayBehaviors[i].isDirectAiming = true;
                    rayBehaviors[i].intersectPoint = intersectPos;

                    refl = transform.position + transform.forward * aimDistance - intersectPos;

                    isUpdateCheck = true;
                    ps.isReceive = true;
                }
                // calculate relative direction of reflected ray
                Vector3 refDir = markerTransform.InverseTransformDirection(refl);
                rayBehaviors[i].dir = refDir;

            } else if( rayBehaviors.Count - 1 < i) {
                sources.RemoveAt(i);
            }
            else
            {
                Debug.Log("Dirty clear:" + i + gameObject.name);
            }
        }
        // set player status flag
        if (ps != null)
        {
            if (!isUpdateCheck)
            {
                ps.isReceive = false;
            }
        }      
    }
    
    // call this funtion to manipulate the rays that related to this mirror
    [ServerCallback]
    public void ManipulateReflectRay(bool add, GameObject source, int index){
        // add a new reflect ray
        if(add){
            CmdCreateReflectRay(source, index);
        }
        // destroy reflect ray
        else{
            CmdDestroyReflectRay(index);
        }
    }

    // Obsolete: current aiming method is convex aiming
    // Reflect helper function for total reflection
    Vector3 Reflect(Vector3 sourcePos, Vector3 mirPos, Vector3 mirForward)
    {
        Vector3 li = (sourcePos - mirPos);

        Vector3 mid = mirForward;

        Vector3 lo = Vector3.zero;

        float dot = Vector3.Dot(li, mid);
        float scaler = (2 * dot / mid.sqrMagnitude);
        if (dot > 0)
        {
            lo = mid * scaler - li;
        }
        else
        {
            lo = -mid * scaler - li;
        }
        return lo;
    }

    // Obsolete: current aiming method is convex aiming
    // Reflect helper function for total reflection one side
    Vector3 ReflectNormal(Vector3 sourcePos, Vector3 mirPos, Vector3 mirForward)
    {
        Vector3 li = (sourcePos - mirPos);

        Vector3 mid = mirForward;

        Vector3 lo = Vector3.zero;

        float dot = Vector3.Dot(li, mid);
        float scaler = (2 * dot / mid.sqrMagnitude);

        lo = mid * scaler - li;

        return lo;
    }

    // Create new ray for mirror
    [ServerCallback]
    public void CmdCreateReflectRay(GameObject source, int index){
        if(rayManager != null && canReflect)
        {
            AudioManager.instance.MirrorStart();
            // request new ray cluster
            if(index == -1)
            {
                index = rayManager.RequestNewRay();
            }  
            
            GameObject rayReflect = Instantiate(rayPrefab, transform.position, transform.rotation);
            RayBehavior rayBehavior = rayReflect.GetComponent<RayBehavior>();
            
            rayBehavior.source = source;
            rayBehavior.mirror = gameObject;
            rayBehavior.index = index;
            MirrorBehavior mir = source.GetComponent<MirrorBehavior>();
            if (mir != null)
            {
                rayBehavior.ownerIndex = mir.ownerIndex;
            }
            NetworkServer.Spawn(rayReflect);
            // start to turn down the Mirror reflect function
            startTurnDown = true;
        }     
    }

    // Destroy specific existing ray
    [ServerCallback]
    public void CmdDestroyReflectRay(int index){
        // rescursive destroy
        startTurnDown = false;
        AudioManager.instance.MirrorEndPlay();
        foreach(RayBehavior ray in rayBehaviors)
        {
            if (ray != null)
            {
                if(ray.index == index)
                {
                    if(ray.nextMirror != null)
                    {
                        ray.nextMirror.CmdDestroyReflectRay(index);
                    }
                    NetworkServer.Destroy(ray.gameObject);
                    return;
                }  
            }
        }     
    }

    // Destroy all existing ray
    [ServerCallback]
    public void CmdDestroyAllReflectRay(){
        // rescursive destroy
        AudioManager.instance.MirrorEndPlay();
        foreach(RayBehavior ray in rayBehaviors)
        {
            if (ray != null)
            {
                if(ray.nextMirror != null)
                {
                    ray.nextMirror.CmdDestroyAllReflectRay();
                }
                NetworkServer.Destroy(ray.gameObject);
                return;
            }
        }     
    }

    // Help function for finding ar marker
    IEnumerator FindMarker(){
        yield return new WaitUntil(()=> FindObjectOfType<MarkerIdentity>() != null);
        markerTransform = FindObjectOfType<MarkerIdentity>().transform;
    }

    // Help function for finding ray manager
    IEnumerator FindRayManager()
    {
        yield return new WaitUntil(() => FindObjectOfType<RayManager>() != null);
        rayManager = FindObjectOfType<RayManager>();
    }
}
