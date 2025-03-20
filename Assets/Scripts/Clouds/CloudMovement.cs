using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class CloudMovement : MonoBehaviour
{

    [SerializeField] private float MinSpeed;
    [SerializeField] private float MaxSpeed;

    private float Speed;
    private Position Objective;

    // Start is called before the first frame update
    void Start()
    {
        Speed = Random.Range(MinSpeed, MaxSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.Lerp(transform.position, transform.position * 2, Time.deltaTime);
    }

    public void test()
    {
        Debug.Log("Feur");
    }
}
