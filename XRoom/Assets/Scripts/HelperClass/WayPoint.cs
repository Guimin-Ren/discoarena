using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WayPoint : MonoBehaviour
{
    public Color color = Color.yellow;
    public float size = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawSphere(transform.position, size);
    }

    // private void OnDrawGizmosSelected() {

    //     if (GetComponentInParent<WayPointRoot>() != null)
    //     {
    //         WayPointRoot wayPointRoot = GetComponentInParent<WayPointRoot>();
    //         wayPointRoot.DrawLines();
    //     }
        
    // }
 
}
