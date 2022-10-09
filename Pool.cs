using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Pool : Singleton<Pool>
{
    public List<PoolNode> objectPool;

    [System.Serializable]
    public class PoolNode
    {
        public string Name;
        public List<GameObject> objs;
        public int currentInstance;
        public int numInScene;
        public int numActive;

        public GameObject GetNextInstance()
        {
            currentInstance = (currentInstance + 1) % (numInScene);
            if (!objs[currentInstance].activeSelf)
            {
                numActive += 1;
            }
            return objs[currentInstance];
        }

        public void RemoveInstance()
        {
            numActive -= 1;
        }
    }
   
    //public int amount;

    public int HashSpawnables(GameObject[] g, int[] amount)
    {
        int ret = -1;
        for (int i = 0; i < g.Length; i++)
        {
            int loc = AddToObjectPool(g[i], amount[i]);
            if (i == 0)
            {
                ret = loc;
            }
        }
        return ret;
    }

    public void RemoveInstance(int hash)
    {
        objectPool[hash].RemoveInstance();
    }

    public int AddToObjectPool(GameObject g, int amount)
    {
        int hashLoc = 0;
        bool found = false;
        List<GameObject> intoPool = new();

        //Search through the current pool and see if there are two of the same items to be pooled. If there are, then add to the pool. Otherwise, make a new entry.
        for (hashLoc = 0; hashLoc < objectPool.Count; hashLoc++)
        {
            if (g.name == objectPool[hashLoc].Name)
            {
                found = true;
                break;
            }
        }

        for (int i = 0; i < amount; i++)
        {
            GameObject temp = Instantiate(g);
            temp.SetActive(false);
            intoPool.Add(temp);
            temp.GetComponent<PoolTracker>().poolHash = hashLoc;
            temp.name += i;
        }

        if (found)
        {
            PoolNode p = objectPool[hashLoc];
            p.objs.AddRange(intoPool);
            p.numInScene += amount;
            objectPool[hashLoc] = p;
        } 

        else
        {
            PoolNode add = new PoolNode()
            {
                Name = g.name,
                objs = intoPool,
                currentInstance = 0,
                numActive = 0,
                numInScene = amount
            };
            objectPool.Add(add);
        }





        return hashLoc;
    }

    public GameObject GetInactiveInstance(int location)
    {
        if (location < 0 || location >= objectPool.Count)
        {
            Debug.LogError("ERROR: Invalid pooling location: " + location);
            return null;
        }
        GameObject ret = objectPool[location].GetNextInstance();
        return ret;
        
    }

    public IEnumerator SpawnEnemies()
    {
        yield return new WaitForEndOfFrame();
        //Go through every single PoolNode and Hash All Enemies. Do this AFTER getting all enemies to behave properly.

    }

    private void Start()
    {
        //StartCoroutine(SpawnEnemies());
    }
    protected override void Awake()
    {
        base.Awake();
    }
}


