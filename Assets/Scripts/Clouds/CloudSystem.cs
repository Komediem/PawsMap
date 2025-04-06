using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;

public class CloudSystem : MonoBehaviour
{
    [Header("Cloud Spawn Colliders")]
    [SerializeField] private BoxCollider2D RightCollider;
    [SerializeField] private BoxCollider2D LeftCollider;
     private int CloudCount;

    [Space]
    [Space]

    [Header("Cloud Data")]
    [SerializeField] private GameObject[] CloudPrefabs;
    [Space]
    [SerializeField] private float MinCloudSpeed;
    [SerializeField] private float MaxCloudSpeed;
    //[Space]
    //[SerializeField] private List <GameObject> sqd ;

    void Start()
    {
        StartCoroutine(TimerForSpawn());
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

        int CloudPrefabNumber = Random.Range(0, CloudPrefabs.Length);
        Debug.Log(CloudPrefabNumber);

        //Right Collider
        if (ColliderNumber == 0)
        {
            //Get a random point in the collider, then get the world position of it.
            Collider = RightCollider;
            Vector2 RandomPosition = RandomPositionInCollider(Collider);

            //Spawn Cloud
            GameObject CloudObject = Instantiate(CloudPrefabs[CloudPrefabNumber]);
            CloudObject.transform.position = new Vector3(RandomPosition.x, RandomPosition.y, 0);
            CloudObject.GetComponent<CloudMovement>().Objective = new Vector2(-RandomPosition.x, RandomPosition.y);

            SetCloudRandomSpeed(CloudObject);

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
            GameObject CloudObject = Instantiate(CloudPrefabs[CloudPrefabNumber]);
            CloudObject.transform.position = new Vector3(RandomPosition.x, RandomPosition.y, 0);
            CloudObject.GetComponent<CloudMovement>().Objective = new Vector2(Mathf.Abs(RandomPosition.x), RandomPosition.y);

            SetCloudRandomSpeed(CloudObject);

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


    void SetCloudRandomSpeed(GameObject SpawnedCloud) 
    {
        SpawnedCloud.GetComponent<CloudMovement>().MinSpeed = MinCloudSpeed;
        SpawnedCloud.GetComponent<CloudMovement>().MaxSpeed = MaxCloudSpeed;
    }

}
