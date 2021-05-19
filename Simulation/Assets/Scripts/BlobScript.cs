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

    public float scale = 1;

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
        gameObject.GetComponent<Renderer>().material.color = new Color((speedBlob*4)/255, (100f + speedBlob)/255, (100f + speedBlob/2)/255);
        gameObject.transform.localScale = transform.localScale * scale;
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.localScale.y, gameObject.transform.position.z);
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

        GameObject[] allBlobs = GameObject.FindGameObjectsWithTag("Blob");
        List<GameObject> eatableBlobs = new List<GameObject>();
        foreach (GameObject lBlob in allBlobs)
        {
            if (transform.localScale.x >= lBlob.transform.localScale.x * 1.2f)
            {
                eatableBlobs.Add(lBlob);
            }
        }

        List<GameObject> allBlobsAndFood = new List<GameObject>();
        allBlobsAndFood.AddRange(allFood);
        allBlobsAndFood.AddRange(eatableBlobs);

        foreach (GameObject EatableGameobject in allBlobsAndFood)
        {
            
            if (minDistanceToBlob > (transform.position - EatableGameobject.transform.position).magnitude)
            {
                nearestFood = EatableGameobject;
                minDistanceToBlob = (transform.position - EatableGameobject.transform.position).magnitude;
            }
        }
    }

    void newRoundEvelutionMethod()
    {
        
        while (foodCollected >= foodToRepreduce)
        {
            foodCollected -= foodToRepreduce;
            
            simControll.BlobCount++;
            GameObject newBlob = (GameObject)Instantiate(blobPrefab, new Vector3(UnityEngine.Random.Range(-groundDimensions.x, groundDimensions.x), transform.position.y, UnityEngine.Random.Range(-groundDimensions.y, groundDimensions.y)), Quaternion.identity);
            if (Random.Range(0f, 1f) < mutationChance)
            {
                int ranDecider = Random.Range(0, 4);
                int mutationStrength = Random.Range(2, 5);
                BlobScript newBlobScript = newBlob.GetComponent<BlobScript>();
                if (ranDecider == 0)
                {
                    newBlobScript.foodToSurvive *= mutationStrength;
                    newBlobScript.speedBlob *= mutationStrength;
                    newBlobScript.foodToRepreduce *= mutationStrength;

                }
                if (ranDecider == 1 && foodToSurvive > 1)
                {
                    newBlobScript.foodToSurvive = Mathf.CeilToInt(newBlobScript.foodToSurvive / mutationStrength);
                    newBlobScript.speedBlob = (newBlobScript.speedBlob / mutationStrength);
                    newBlobScript.foodToRepreduce = Mathf.CeilToInt(newBlobScript.foodToRepreduce / mutationStrength);
                    
                }
                if (ranDecider == 2)
                {
                    newBlobScript.foodToSurvive *= 2;
                    newBlobScript.scale *= 1.1f;
                    newBlobScript.foodToRepreduce *= 2;
                }
                if (ranDecider == 3 && scale > 1)
                {
                    newBlobScript.foodToSurvive = Mathf.CeilToInt(newBlobScript.foodToSurvive / 2);
                    newBlobScript.scale = (newBlobScript.scale / 1.1f);
                    newBlobScript.foodToRepreduce = Mathf.CeilToInt(newBlobScript.foodToRepreduce / 2);
                }
                
            }
        }
        foodCollected--;
        if (foodCollected < foodToSurvive - 1)
        {

           
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

        if (destination != null && (destination.tag == "Food" && destination.GetComponent<FoodScript>().currentEater == gameObject || destination.tag == "Blob"))
        {
            foodCollected++;
            if (destination.tag == "Blob")
            {
                print(destination.tag);
            }
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
