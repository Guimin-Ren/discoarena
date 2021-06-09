using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class ARTest : MonoBehaviour
{

    public GameObject spacePrefab;
    GameObject respawnedObj;
    ARRaycastManager aRRaycastManager;
    Vector2 touchPos;

    List<ARRaycastHit> hits = new List<ARRaycastHit>();

    public GameObject button;

    public bool isSet = false;

    ARPlaneManager aRPlaneManager;

    ARTrackedImageManager imgTrackerManager;

    private void Awake()
    {
        aRRaycastManager = GetComponent<ARRaycastManager>();
        imgTrackerManager = GetComponent<ARTrackedImageManager>();
    }

    bool TryGetTouchPos(out Vector2 touchPosition)
    {
        if(Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }
        touchPosition = default;
        return false;
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            OnImagesChanged(trackedImage);
        }
    }

    private void OnImagesChanged(ARTrackedImage referenceImage)
    {
        if (!respawnedObj){
            respawnedObj =  Instantiate(spacePrefab, referenceImage.transform.position, referenceImage.transform.rotation);
        }
        else{
            respawnedObj.transform.position = referenceImage.transform.position;
        }
        button.SetActive(true);
    }


    // Start is called before the first frame update
    void Start()
    {
        aRPlaneManager = GetComponent<ARPlaneManager>();
    }

    // Update is called once per frame
    void Update()
    {

        if(isSet){
            return;
        }
        if (!(TryGetTouchPos(out touchPos))) {
            return;
        }
        if(aRRaycastManager.Raycast(touchPos, hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPose = hits[0].pose;
            {
                if (!respawnedObj)
                {
                    respawnedObj = Instantiate(spacePrefab, hitPose.position, hitPose.rotation);
                    
                }
                else
                {
                    respawnedObj.transform.position = hitPose.position;
                    //respawnedObj.transform.rotation = hitPose.rotation;
                }
            }
            button.SetActive(true);
        }
    }

    public void SetVirtualSpace(){
        isSet = true;
        if(respawnedObj == null){
            return;
        }
        else{

            foreach(ARPlane plane in aRPlaneManager.trackables){
                plane.gameObject.SetActive(false);
            }

            aRPlaneManager.enabled = false;
            imgTrackerManager.enabled = false;

            respawnedObj.GetComponent<ARSpaceController>().SetLightSource();
        }
    }
}
