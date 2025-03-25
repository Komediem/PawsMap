using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class CloudMovement : MonoBehaviour
{

    [SerializeField] private float MinSpeed;
    [SerializeField] private float MaxSpeed;
    [SerializeField] private int CloudLayer;
    [SerializeField] public Vector2 Objective;

    private float Speed;

    void Start()
    {
        Speed = Random.Range(MinSpeed, MaxSpeed);
        Speed = Speed * Time.deltaTime;
    }

    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, Objective, Speed);

        if (transform.position == new Vector3(Objective.x, Objective.y, 0))
        {
            Destroy(gameObject);
        }
    }
}
