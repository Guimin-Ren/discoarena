using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayReflectionManager : MonoBehaviour
{
   public bool isReceived;

    public Ray lightIn;

    //public Camera cam;
    
    //public LineRenderer lineRenderer;

    public GameObject cylinder;
    public Vector3 hitPoint;

    public LayerMask layer;
    public LayerMask enemyLayer;

    public GameObject circle;

    public GameObject detectedCircle;

    public GameObject Mirror;

    //Camera cam;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(isReceived){

            cylinder.SetActive(true);


            

            Vector3 start = hitPoint;
            
            Vector3 lo = Reflect(lightIn.origin, start, transform.forward);

            Vector3 end = start + lo;

            SetLS(start, end);

            //raycast
            Ray ray = new Ray();
            ray.origin = start;
            ray.direction = end - start;
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, Mathf.Infinity, enemyLayer)){
                Debug.Log("hit enemy");
              if (hit.transform.gameObject.GetComponent<RobotBehavior>() != null) {
                  hit.transform.gameObject.GetComponent<RobotBehavior>().onShot();
              }
              
            }



        }else{
            if (cylinder) {
                cylinder.SetActive(false);
            }
            
        }

        //Debug.Log("cam: " + transform.position);
    }

    Vector3 Reflect(Vector3 sourcePos, Vector3 mirPos, Vector3 mirForward){
            Vector3 li = (sourcePos - mirPos);

            Vector3 mid = mirForward;

            Vector3 lo = Vector3.zero;

            float dot = Vector3.Dot(li, mid);
            float scaler = (2 * dot / mid.sqrMagnitude );
            if(dot > 0){
                lo = mid * scaler -li;
            }
            else{
                lo = - mid * scaler -li;
            }
            return lo;
    }
    
    void SetLS(Vector3 startP, Vector3 finalP)
        {
           Vector3 rightPosition = (startP + finalP) / 2;
            Vector3 rightRotation = finalP - startP;
            float HalfLength = Vector3.Distance(startP, finalP) / 2;
            float LThickness = 0.01f;
    
            cylinder.transform.position = rightPosition;   
            cylinder.transform.rotation = Quaternion.FromToRotation(Vector3.up, rightRotation);
            cylinder.transform.localScale = new Vector3(LThickness, HalfLength * 1.0f, LThickness);
        }

}
