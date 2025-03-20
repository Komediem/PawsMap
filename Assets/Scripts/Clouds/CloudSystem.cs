using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudSystem : MonoBehaviour
{
    [SerializeField] private GameObject CloudPrefab;
    [SerializeField] private BoxCollider2D RightCollider;
    [SerializeField] private BoxCollider2D LeftCollider;
    [SerializeField] private int CloudCount;


    void Start()
    {
        StartCoroutine(TimerForSpawn());
    }

    void Update()
    {

    }

    IEnumerator TimerForSpawn()
    {
        yield return new WaitForSeconds(2);
        SpawnCloud();
    }


    void SpawnCloud()
    {
        int ColliderNumber = Random.Range(0, 2);
        BoxCollider2D Collider;

        //Right Collider
        if(ColliderNumber == 0)
        {
            //Get a random point in the collider, then get the world position of it.
            Collider = RightCollider;
            Vector2 RandomPosition = RandomPositionInCollider(Collider);

            //Spawn Cloud
            GameObject CloudObject = Instantiate(CloudPrefab);
            CloudObject.transform.position = new Vector3(RandomPosition.x, RandomPosition.y, 0);
            CloudObject.GetComponent<CloudMovement>().test();


            //Restart Coroutine
            StartCoroutine(TimerForSpawn());
        }

        //Left Collider
        if (ColliderNumber == 1)
        {
            //Get a random point in the collider, then get the world position of it.
            Collider = LeftCollider;
            Vector2 RandomPosition = RandomPositionInCollider(Collider);

            //Spawn Cloud
            GameObject CloudObject = Instantiate(CloudPrefab);
            CloudObject.transform.position = new Vector3(RandomPosition.x, RandomPosition.y, 0);
            CloudObject.GetComponent<CloudMovement>().test();


            //Restart Coroutine
            StartCoroutine(TimerForSpawn());
        }
    }

    private Vector2 RandomPositionInCollider(BoxCollider2D SpawnCollider)
    {
        return new Vector2(
            Random.Range(SpawnCollider.bounds.min.x, SpawnCollider.bounds.max.x),
            Random.Range(SpawnCollider.bounds.min.y, SpawnCollider.bounds.max.y)
            );
    }

    void CloudMove()
    {
        
    }
}
