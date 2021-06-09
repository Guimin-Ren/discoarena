using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/*This script works as a lightsource manager. It will be called by the game manager to generate and destroy the light on server*/
public class LightSourceNetController : NetworkBehaviour
{
   public GameObject rayPrefab;

    Transform markerTransform;
    public bool isMarkerFound = false;
    [SerializeField] private float lightAngle = -1f;
    [SerializeField] private GameObject lanternPrefab;
    [SerializeField] private Vector3[] floatLightPosition;
    [SerializeField] private float inclination = 1;
    private bool odd = false;

    RayManager rayManager;
    
    int reservedIndex0 = 0;

    int reservedIndex1 = 1;
    private List<int> postionSet = new List<int>();
    [SerializeField] private GameObject[] Lanterns;
    [SerializeField] private List<GameObject> Lasers = new List<GameObject>();

    
    // Start is called before the first frame update
    [ServerCallback]
    void Start()
    {
        StartCoroutine(FindMarker());
        StartCoroutine(FindRayManager());
        GenerateLanterns();
    }

    
    //  call this to generate floating laser 
    // num is the total number of generated lasers
    public void CmdStartFloatLaser(int num){
        if (rayManager != null) {
            while (num > 0) {
               GenerateFloatLaser();
                num--;
            }
            postionSet.Clear();
        }
    }

  
    // random get direction of light
    public Vector3 GetLaserInitialDir(int laserNum){
        switch(laserNum){
            case 0:
                return new Vector3(1,lightAngle,1);
            case 1:
                return new Vector3(1,lightAngle,-1);
            case 2:
                return new Vector3(-1,lightAngle,1);
            case 3:         
                return new Vector3(-1,lightAngle,-1);
            default:
                return new Vector3(0,lightAngle,0);
        }
    }
    // random get direction of spotlight
    private Vector3 GetLanternInitialDir(Vector3 pos){
        float d = Random.Range(-0.6f, -0.2f);
        Vector3 dir = new Vector3();
        dir.x = -pos.x;
        dir.y = d;
        if (pos.y == 0) {
            dir.y = Random.Range(1.0f, 1.2f);
        }
       
        if (pos.z == 0) {
            dir.z = Random.Range(-1 * inclination, 1 * inclination);
        } else if (pos.z > 0) {
            dir.z = Random.Range(-1 * inclination, 0);
        } else {
            dir.z = Random.Range(0, 1 * inclination);
        }
        return dir;
    }
    //generate the spotlight at the beginning
    private void GenerateLanterns() {
        for (int i = 1; i < 13; i++) {
           
            Quaternion rot;
            if (i < 7) {
              rot = Quaternion.LookRotation(new Vector3(-1,0,0));
            } else {
              rot = Quaternion.LookRotation(new Vector3(1,0,0));
            }
            GameObject Lantern = Instantiate(lanternPrefab,floatLightPosition[i],rot);
            // Debug.Log("i = " + i + "Lantern" + Lantern);
            PosAndRot posAndRot = new PosAndRot();
       
            HelperFunction.GetRelativePosAndRot(FindObjectOfType<MarkerIdentity>().transform, Lantern.transform, out posAndRot);
            Lantern.transform.position = posAndRot.pos;
            Lantern.transform.rotation = posAndRot.rot;
            NetworkServer.Spawn(Lantern);
            Lantern.GetComponent<RelativeDiscoLightTranforms>().pos = Lantern.transform.position;
            Lantern.GetComponent<RelativeDiscoLightTranforms>().rot = Lantern.transform.rotation;
            Lanterns[i] = Lantern;
        }
       
    }

    // call this to generate the light on server
     private void GenerateFloatLaser(){
        int p;
        // odd is used to generate equally to each side
        if (odd) {
            p = Random.Range(1, 7);
        } else {
            p = Random.Range(7, 13);
        }
        
        // if position exsit, reroll until get a new one
        while (postionSet.Contains(p) && postionSet.Count < floatLightPosition.Length){
                if (odd) {
                p = Random.Range(1, 7);
            } else {
                p = Random.Range(7, 13);
            }
            
        }
        // add position to list
        postionSet.Add(p);
        if (odd) {
            odd = false;
        } else {
            odd = true;
        }
        Vector3 dir = GetLanternInitialDir(floatLightPosition[p]);
        // get rotation from the direction
        Quaternion rot = Quaternion.LookRotation(dir);
        // generate lantern prefab with position and rotation 

        GameObject laser = Instantiate(rayPrefab, floatLightPosition[p], rot);


        RayBehavior rayBehavior = laser.GetComponent<RayBehavior>();
        rayBehavior.source = Lanterns[p];
        
        rayBehavior.dir = markerTransform.InverseTransformDirection(dir);
        rayBehavior.isBulb = true;
        rayBehavior.index = rayManager.RequestNewRay();
        NetworkServer.Spawn(laser);
        Lasers.Add(laser);
        
        
    }
    // call this to destroy the light on server
    public void CmdDestroyFloatLaser() {
        if (Lasers.Count == 0) {
            return;
        }
        // Debug.Log("Laser count" +Lasers.Count);
        int index = Lasers.Count;
        for (int i = index - 1; i >= 0; i--){
            GameObject laser = Lasers[i];
            RayBehavior rayBehavior = laser.GetComponent<RayBehavior>();
            if (rayBehavior.nextMirror != null) {
                rayBehavior.nextMirror.CmdDestroyReflectRay(rayBehavior.index);
            }
            Lasers.Remove(Lasers[i]);
            NetworkServer.Destroy(laser);

        }
        
    }

    IEnumerator FindMarker(){
        yield return new WaitUntil(()=> FindObjectOfType<MarkerIdentity>() != null);
        markerTransform = FindObjectOfType<MarkerIdentity>().transform;
        isMarkerFound = true;
    }
    IEnumerator FindRayManager()
    {
        yield return new WaitUntil(() => FindObjectOfType<RayManager>() != null);
        rayManager = FindObjectOfType<RayManager>();

    }

  
}
