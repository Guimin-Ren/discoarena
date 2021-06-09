using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RayManager : NetworkBehaviour
{
    // count of ray clusters (last index + 1)
    [SyncVar]
    public int indexCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // create new ray cluster
    [ServerCallback]
    public int RequestNewRay()
    {
        int index = indexCount;
        indexCount++;
        return index;
    }
}
