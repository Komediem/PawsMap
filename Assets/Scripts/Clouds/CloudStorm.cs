using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.Port;

public class CloudStorm : MonoBehaviour
{

    [SerializeField] private GameObject CloudCenter;
    [SerializeField] private float Speed;
    [SerializeField] private float HidingThreshold;
    private float Opacity;

    // Start is called before the first frame update
    void Start()
    {
        Speed = Speed * Time.deltaTime;
        Opacity = CloudCenter.GetComponent<SpriteRenderer>().color.a;
    }

    // Update is called once per frame
    void Update()
    {
        CloudCenter.transform.Rotate(Vector3.forward, Speed);

        if (GameManager.Instance.currentSize < HidingThreshold)
        {
            float i = Mathf.Lerp(CloudCenter.GetComponent<SpriteRenderer>().color.a, 0, Time.deltaTime * 3);
            CloudCenter.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, i);
        }
        else if (GameManager.Instance.currentSize > HidingThreshold)
        {
            float i = Mathf.Lerp(CloudCenter.GetComponent<SpriteRenderer>().color.a, Opacity, Time.deltaTime * 3);
            CloudCenter.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, i);
        }
    }
}
