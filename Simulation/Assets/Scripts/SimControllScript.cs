using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SimControllScript : MonoBehaviour
{
    public int BlobCount = 1;
    public Vector2 foodAmountMinMax;
    public event Action onRoundStart;
    int day;
    public GameObject FoodPrefab;
    public GameObject Ground;
    Vector2 groundDimensions;
    public float maxRoundTime = 10;
    private float rountTimeDelta;
    GameObject[] foodArr = {};

    // Start is called before the first frame update
    void Start()
    {
        groundDimensions = new Vector2(Ground.transform.GetChild(0).transform.position.x, Ground.transform.GetChild(1).transform.position.z);
        
        
    }

    private void Update()
    {
        BehavoirLoop();
    }

    void BehavoirLoop()
    {
        if (foodArr.Length == 0 || Time.time > maxRoundTime + rountTimeDelta)
        {
            rountTimeDelta = Time.time;
            day++;
            foodSpawn();
            if (onRoundStart != null && day > 1)
            {
                onRoundStart();
            }
            print(BlobCount);
            
        }
        foodArr = GameObject.FindGameObjectsWithTag("Food");

    }

    void foodSpawn()
    {
        
        for (int y = 0; y < UnityEngine.Random.Range(foodAmountMinMax.x, foodAmountMinMax.y); y++)
        {
            Instantiate(FoodPrefab, new Vector3(UnityEngine.Random.Range(-groundDimensions.x, groundDimensions.x), 0.5f, UnityEngine.Random.Range(-groundDimensions.y, groundDimensions.y)), Quaternion.identity);
        }
    }
}
