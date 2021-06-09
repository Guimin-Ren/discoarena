using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RelativeRaySetter : NetworkBehaviour
{
    // reference of ray 
    public RayBehavior rayBehavior;

    // transform of marker
    Transform markerTransform;

    // reference of source object
    public GameObject source;

    // reference of target object
    public GameObject target;

    PosAndRot parS;
    PosAndRot parE;
    
    // relative direction of ray
    public Vector3 dir;

    // distance of ray cast detection
    public float distance = 1;

    // distance of ray display
    public float tmpDistance;

    // layer mask for ray cast
    public LayerMask checkLayer;

    // reference of hit object
    public GameObject hitObj;

    // reference of flash
    public GameObject flash;

    // sync stat of falsh particle
    [SyncVar]
    public FlashStat flashStat;

    // sclae of flash particle system
    public Vector3 flashScale;

    // particle system of indicator
    public GameObject indicatorPS;

    // scale of indicator
    public Vector3 indicatorScale;

    // flag for checking color status
    public bool isColorSet = false;

    // index of color
    public int colorIndex;

    // colors for particle system
    public Color[] colorsParticleSystem;

    // materials for internal ray
    public Material[] matsInternal;

    // materials for external ray
    public Material[] matsExternal;

    // colors for line renderer
    public Color[] colorsLineRenderer;

    // mesh renderer for internal ray
    public MeshRenderer mrInternal;

    // mesh renderer for external ray
    public MeshRenderer mrExtrernal;

    // reference of line renderer
    public LineRenderer lineRenderer;

    // particle system of ray
    public ParticleSystem lightBeamPS;

    // Start is called before the first frame update
    void Start()
    {
        flashStat.hit = false;
        flashScale = flash.transform.lossyScale;
        indicatorScale = flash.transform.lossyScale;

        rayBehavior = GetComponentInParent<RayBehavior>();
        
        StartCoroutine(FindMarker());
        tmpDistance = distance;
        // Retreive color from ray behavior and set particle system color
        StartCoroutine(CheckOwnerAndSetColorIndex());
        StartCoroutine(SetColor());
    }

    // Use relative position and rotation direction to calculate local world position and direction of ray
    void Update()
    {
        source = rayBehavior.source;
        target = rayBehavior.target;
        dir = rayBehavior.dir;
        tmpDistance = rayBehavior.tmpDistance;
        if(markerTransform == null){
            return;
        }
        if(target != null){
            HelperFunction.SetRelativePosAndRot( markerTransform, source.transform, out parS);
            HelperFunction.SetRelativePosAndRot( markerTransform, target.transform, out parE);

            SetLS(parS.pos, parE.pos);
        }
        else{
            Vector3 dirLocal = new Vector3(0, 0, 0);

            if (rayBehavior.isDirectAiming)
            {
                dirLocal = markerTransform.TransformDirection(dir);
                Vector3 posIntersection = markerTransform.TransformDirection(rayBehavior.intersectPoint) + markerTransform.position;
                SetLS(posIntersection, posIntersection + dirLocal * tmpDistance, 0.999f);
            }
            else
            {
                dirLocal = markerTransform.TransformDirection(dir);
                SetLS(source.transform.position, source.transform.position + dirLocal * tmpDistance);
            }  
        }
        // set falsh ps
        SetFlash();
        // set indicator ps
        SetIndicator();
        // raycast check
        RaycastTest();
    }

    // Help function for setting cylinder
    void SetLS(Vector3 startP, Vector3 finalP, float scaleRatio)
    {
        Vector3 rightPosition = (startP + finalP) / 2;
        Vector3 rightRotation = finalP - startP;
        float HalfLength = Vector3.Distance(startP, finalP) / 2;
        float LThickness = 0.1f;

        transform.position = rightPosition;
        transform.rotation = Quaternion.FromToRotation(Vector3.up, rightRotation);
        transform.localScale = new Vector3(LThickness, HalfLength * scaleRatio, LThickness);
    }

    // Help function for setting cylinder
    void SetLS(Vector3 startP, Vector3 finalP)
    {
        Vector3 rightPosition = (startP + finalP) / 2;
        Vector3 rightRotation = finalP - startP;
        float HalfLength = Vector3.Distance(startP, finalP) / 2;
        float LThickness = 0.05f;
    
        transform.position = rightPosition;   
        transform.rotation = Quaternion.FromToRotation(Vector3.up, rightRotation);
        transform.localScale = new Vector3(LThickness, HalfLength * 0.95f, LThickness);
    }

    // Help function for finding ar marker
    IEnumerator FindMarker(){
        yield return new WaitUntil(()=> FindObjectOfType<MarkerIdentity>() != null);
        markerTransform = FindObjectOfType<MarkerIdentity>().transform;
    }

    // raycast check
    [ServerCallback]
    public void RaycastTest(){
        Ray ray = new Ray();
        if (rayBehavior.mirrorBehavior != null)
        {
            if (rayBehavior.mirrorBehavior.isFirstPhone)
            {
                ray.origin = rayBehavior.mirrorBehavior.intersectPos;
            }
            else
            {
                ray.origin = source.transform.position;
            }
        }
        
        else
        {
            ray.origin = source.transform.position;
        }
        
        ray.direction = transform.up;
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, distance, checkLayer)){
            hitObj = hit.collider.gameObject;
            //Debug.Log("ray " + gameObject +"hit " + hitObj);
            rayBehavior.tmpDistance = Vector3.Distance(source.transform.position ,hitObj.transform.position) / 2;
            if (hitObj.GetComponent<MirrorBehavior>() != null && hitObj.GetComponent<MirrorBehavior>().isFirstPhone) {
                //Debug.Log("isfirstphone reflect !!!!");
                flashStat.hit = true;
                flashStat.pos = hit.point - markerTransform.position;
                //rayBehavior.intersectPos = hit.point;
                rayBehavior.HitTarget(true, hitObj, hit.point);
            }

        }
        else{
            hitObj = null;
            flashStat.hit = false;
            rayBehavior.tmpDistance = distance;
            rayBehavior.HitTarget(false, hitObj, new Vector3(0, 0, 0));
        }
    }

    public void SetFlash()
    {
        if (!flashStat.hit)
        {
            flash.transform.localPosition = new Vector3(0, -1, 0);
            SetGlobalScale(flash.transform, flashScale);
        }
        else
        {
            flash.transform.position = markerTransform.position + flashStat.pos;
            SetGlobalScale(flash.transform, flashScale);
        }
    }

    public void SetIndicator()
    {
        indicatorPS.transform.localPosition = new Vector3(0, -1.0f, 0);
        SetGlobalScale(indicatorPS.transform, indicatorScale);
    }

    public static void SetGlobalScale(Transform transform, Vector3 globalScale)
    {
        transform.localScale = Vector3.one;
        transform.localScale = new Vector3(globalScale.x / transform.lossyScale.x, globalScale.y / transform.lossyScale.y, globalScale.z / transform.lossyScale.z);
    }

    IEnumerator SetColor()
    {
        yield return new WaitUntil(() => isColorSet); 
        ParticleSystem[] particleSystems = lightBeamPS.GetComponentsInChildren<ParticleSystem>();
        foreach(ParticleSystem ps in particleSystems)
        {
            ParticleSystem.MainModule main = ps.main;
            main.startColor = colorsParticleSystem[colorIndex];
        }
        mrInternal.material = matsInternal[colorIndex];
        mrExtrernal.material = matsExternal[colorIndex];
        lineRenderer.startColor = colorsLineRenderer[colorIndex * 2];
        lineRenderer.endColor = colorsLineRenderer[colorIndex * 2 + 1];
    }

    IEnumerator CheckOwnerAndSetColorIndex()
    {
        yield return new WaitUntil(() => FindObjectsOfType<PlayerStatus>().Length == 2);
        PlayerStatus[] pss = FindObjectsOfType<PlayerStatus>();
        PlayerStatus ps = null;
        foreach (PlayerStatus playerStatus in pss)
        {
            if (playerStatus.isLocalPlayer)
            {
                ps = playerStatus;
                break;
            }
        }
        // for server
        if (ps == null)
        {
            colorIndex = 0;
        }
        else
        {
            // Self Color
            if (ps.index == rayBehavior.ownerIndex)
            {
                colorIndex = 1;
            }
            // neutral ray
            else if (rayBehavior.ownerIndex == 0)
            {
                colorIndex = 0;
            }
            // Enemy Color
            else
            {
                colorIndex = 2;
            }
        }
        isColorSet = true;
    }

}

public struct FlashStat{
    public bool hit;
    public Vector3 pos;
}