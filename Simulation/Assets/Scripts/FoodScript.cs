using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodScript : MonoBehaviour
{
    List<GameObject> blobsCloserThanRange;
    List<GameObject> prevBlobs;
    public GameObject currentEater;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        blobsCloserThanRange = CheckForBlobsInRange(.5f);
        
        if (blobsCloserThanRange.Count >= 1 && !CompareLists<GameObject>(blobsCloserThanRange, prevBlobs))
        {
            
            int ranInt = Random.Range(0, blobsCloserThanRange.Count);
            currentEater = blobsCloserThanRange[ranInt];
            prevBlobs = new List<GameObject>(blobsCloserThanRange);
        }
    }

    List<GameObject> CheckForBlobsInRange(float range)
    {
        GameObject[] allBlobs = GameObject.FindGameObjectsWithTag("Blob");
        
        List<GameObject> returnList = new List<GameObject>();

        foreach (GameObject blob in allBlobs)
        {
           
            
            if ((transform.position - new Vector3(blob.transform.position.x, transform.position.y, blob.transform.position.z)).magnitude < range)
            {
                returnList.Add(blob);
            }
        }
        return returnList;
    }

    public static bool CompareLists<T>(List<T> aListA, List<T> aListB)
    {
        if (aListA == null || aListB == null || aListA.Count != aListB.Count)
            return false;
        if (aListA.Count == 0)
            return true;
        Dictionary<T, int> lookUp = new Dictionary<T, int>();
        // create index for the first list
        for (int i = 0; i < aListA.Count; i++)
        {
            int count = 0;
            if (!lookUp.TryGetValue(aListA[i], out count))
            {
                lookUp.Add(aListA[i], 1);
                continue;
            }
            lookUp[aListA[i]] = count + 1;
        }
        for (int i = 0; i < aListB.Count; i++)
        {
            int count = 0;
            if (!lookUp.TryGetValue(aListB[i], out count))
            {
                // early exit as the current value in B doesn't exist in the lookUp (and not in ListA)
                return false;
            }
            count--;
            if (count <= 0)
                lookUp.Remove(aListB[i]);
            else
                lookUp[aListB[i]] = count;
        }
        // if there are remaining elements in the lookUp, that means ListA contains elements that do not exist in ListB
        return lookUp.Count == 0;
    }
}
