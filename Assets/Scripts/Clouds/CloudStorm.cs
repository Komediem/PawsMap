using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudStorm : MonoBehaviour
{

    [SerializeField] private GameObject CloudCenter;
    [SerializeField] private float Speed;

    // Start is called before the first frame update
    void Start()
    {
        Speed = Speed * Time.deltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        //CloudCenter.transform.rotation = Quaternion.Slerp(CloudCenter.transform.rotation, CloudCenter.transform.rotation, Speed);
        CloudCenter.transform.Rotate(Vector3.forward, Speed);
    }
}
