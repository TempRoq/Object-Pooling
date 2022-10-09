using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolTracker : MonoBehaviour
{
    public int poolHash = -1;


    public void StopTracking()
    {
        Pool.instance.RemoveInstance(poolHash);
    }

}