using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AjustButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public ARMarkerAdjustController aRMarkerAdjustController;
    public bool isLeft;
    public bool isPressed = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnPointerDown(PointerEventData eventData){
        isPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData){
        isPressed = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(isPressed){
            aRMarkerAdjustController.AdjustRotation(isLeft);
        }
    }
}
