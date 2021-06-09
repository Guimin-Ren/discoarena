using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class ARSceneRespawner : MonoBehaviour
{
    // prefab of virtual scene
    public GameObject spacePrefab;
    
    // reference of respawned space
    public GameObject respawnedObj;
    
    // btn for confirm
    public GameObject btnConfirm;
    
    // btn for rotate marker
    public GameObject btnAdjust;

    public bool isSet = false;

    ARPlaneManager aRPlaneManager;

    ARTrackedImageManager imgTrackerManager;

    // value for setting ar marker adjustment
    public ARMarkerAdjustController arMarkerAdjustController;

    public Quaternion originRot;

    private void Awake()
    {
        imgTrackerManager = GetComponent<ARTrackedImageManager>();
        arMarkerAdjustController = GetComponent<ARMarkerAdjustController>();
    }

    private void OnEnable()
    {
        imgTrackerManager.trackedImagesChanged += OnTrackedImagesChanged;
    }
    void OnDisable()
    {
        imgTrackerManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }
    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            OnImagesChanged(trackedImage);
        }
        foreach (var trackedImage in eventArgs.updated)
        {
            OnImagesChanged(trackedImage);
        }
    }

    private void OnImagesChanged(ARTrackedImage referenceImage)
    {
        if (!respawnedObj){
            respawnedObj =  Instantiate(spacePrefab, referenceImage.transform.position, referenceImage.transform.rotation);
            originRot = respawnedObj.transform.rotation;
        }
        else{
            respawnedObj.transform.position = referenceImage.transform.position;
            respawnedObj.transform.rotation = originRot * Quaternion.Euler(arMarkerAdjustController.rotateOffset);
        }
        btnAdjust.SetActive(true);
        btnConfirm.SetActive(true);
    }

    void OnTrackedImagesChangedRuntime(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            OnImagesChangedRuntime(trackedImage);
        }
        foreach (var trackedImage in eventArgs.updated)
        {
            OnImagesChangedRuntime(trackedImage);
        }
    }

    private void OnImagesChangedRuntime(ARTrackedImage referenceImage)
    {
        respawnedObj.transform.position = referenceImage.transform.position;   
        respawnedObj.transform.rotation = originRot * Quaternion.Euler(arMarkerAdjustController.rotateOffset);
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
            imgTrackerManager.trackedImagesChanged -= OnTrackedImagesChanged;
            imgTrackerManager.trackedImagesChanged += OnTrackedImagesChangedRuntime;
            aRPlaneManager.enabled = false;
        }
    }
}
