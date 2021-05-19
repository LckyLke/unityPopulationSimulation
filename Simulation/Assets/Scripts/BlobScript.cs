using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobScript : MonoBehaviour
{
    public GameObject blobPrefab;

    public GameObject Ground;
    
    public float speedBlob = 7.5f;
    public float mutationChance = .1f;
    public int foodToRepreduce = 2;
    public int foodToSurvive = 1;

    SimControllScript simControll;
    int foodCollected;
    GameObject nearestFood;
    GameObject[] allFood;

    Vector2 groundDimensions;

    Coroutine co;
    

    // Start is called before the first frame update
    void Awake()
    {
        Ground = GameObject.FindGameObjectWithTag("Ground");
        groundDimensions = new Vector2(Ground.transform.GetChild(0).transform.position.x, Ground.transform.GetChild(1).transform.position.z);
        simControll = FindObjectOfType<SimControllScript>();
        simControll.onRoundStart += newRoundEvelutionMethod;
        gameObject.GetComponent<Renderer>().material.color = new Color((speedBlob*5)/255, (100f + 2 * speedBlob)/255, (100f + speedBlob)/255);
    }

    private void Start()
    {
        MoveToNearestFood();
    }

    // Update is called once per frame

    void MoveToNearestFood()
    {
        FindClostestFood(ref nearestFood);
        co = StartCoroutine(Move(nearestFood, speedBlob));
    }

    void FindClostestFood(ref GameObject nearestFood)
    {
        float minDistanceToBlob = Mathf.Infinity;

        allFood = GameObject.FindGameObjectsWithTag("Food");
        
        foreach (GameObject Food in allFood)
        {
            
            if (minDistanceToBlob > (transform.position - Food.transform.position).magnitude)
            {
                nearestFood = Food;
                minDistanceToBlob = (transform.position - Food.transform.position).magnitude;
            }
        }
    }

    void newRoundEvelutionMethod()
    {
        
        while (foodCollected >= foodToRepreduce)
        {
            foodCollected -= foodToRepreduce;
            print("new Blob spawned");
            simControll.BlobCount++;
            GameObject newBlob = (GameObject)Instantiate(blobPrefab, new Vector3(UnityEngine.Random.Range(-groundDimensions.x, groundDimensions.x), transform.position.y, UnityEngine.Random.Range(-groundDimensions.y, groundDimensions.y)), Quaternion.identity);
            if (Random.Range(0f, 1f) < mutationChance)
            {
                int ranDecider = Random.Range(0, 2);
                BlobScript newBlobScript = newBlob.GetComponent<BlobScript>();
                if (ranDecider == 0)
                {
                    newBlobScript.foodToSurvive *= 2;
                    newBlobScript.speedBlob *= 2;
                    newBlobScript.foodToRepreduce *= 2;

                }
                if (ranDecider == 1 && foodToSurvive > 1)
                {
                    newBlobScript.foodToSurvive = newBlobScript.foodToSurvive / 2;
                    newBlobScript.speedBlob = newBlobScript.speedBlob / 2;
                    newBlobScript.foodToRepreduce = newBlobScript.foodToRepreduce / 2;
                }
            }
        }
        foodCollected--;
        if (foodCollected < foodToSurvive - 1)
        {

            print("Blob destroyed:(");
            simControll.BlobCount--;
            Destroy(gameObject);
            return;
        }
        
    }

    IEnumerator Move(GameObject destination, float speed)
    {

        while (destination != null && transform.position != new Vector3(destination.transform.position.x, transform.position.y, destination.transform.position.z))
        { 
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(destination.transform.position.x, transform.position.y, destination.transform.position.z), speed * Time.deltaTime);
            yield return null;
        }
        if (destination != null && destination.GetComponent<FoodScript>().currentEater == gameObject)
        {
            foodCollected++;
        }
        
        Destroy(destination);
        yield return null;
        MoveToNearestFood();
        
    }

    private void OnDestroy()
    {
        simControll.onRoundStart -= newRoundEvelutionMethod;
    }



}
