using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class CloudMovement : MonoBehaviour
{

    [SerializeField] public float MinSpeed;
    [SerializeField] public float MaxSpeed;
    [Space]
    [SerializeField] private int MinCloudOpacity;
    [SerializeField] private int MaxCloudOpacity = 255;

    [Header("Private Data")]
    public float Speed;
    [SerializeField] public Vector2 Objective;
    [SerializeField] private int CloudLayer;
    [SerializeField] public int ID;

    void Start()
    {
        Speed = Random.Range(MinSpeed, MaxSpeed);
        Speed = Speed * Time.deltaTime;

        float CloudRandomOpacity = Random.Range(MinCloudOpacity, MaxCloudOpacity);
        CloudRandomOpacity = CloudRandomOpacity / 255;
        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, CloudRandomOpacity);
    }

    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, Objective, Speed);

        if (transform.position == new Vector3(Objective.x, Objective.y, 0))
        {
            Destroy(gameObject);
            CloudSystem.Instance.NumberOfClouds -= 1;
        }
    }
}
