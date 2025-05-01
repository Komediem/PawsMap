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
    [SerializeField] private bool IsTopBottomCloud = false;
    [SerializeField] private int HidingThreshold;

    [Header("Private Data")]
    public float Speed;
    [SerializeField] public Vector2 Objective;
    [SerializeField] private int CloudLayer;
    [SerializeField] public int ID;
    private int Opacity;

    void Start()
    {
        Speed = Random.Range(MinSpeed, MaxSpeed);
        Speed = Speed * Time.deltaTime;

        int CloudRandomOpacity = Random.Range(MinCloudOpacity, MaxCloudOpacity);
        CloudRandomOpacity = CloudRandomOpacity / 255;
        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, CloudRandomOpacity);
        Opacity = CloudRandomOpacity;
    }

    void Update()
    {
        if (GameManager.Instance.currentSize > HidingThreshold)
        {
            float i = Mathf.Lerp(Opacity, 0, Time.deltaTime);
            this.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, i);
        }
        else if (GameManager.Instance.currentSize <= HidingThreshold)
        {
            //this.GetComponent<SpriteRenderer>().enabled = true;

        }


        if (IsTopBottomCloud == false)
        {
            transform.position = Vector2.MoveTowards(transform.position, Objective, Speed);

            if (transform.position == new Vector3(Objective.x, Objective.y, 0))
            {
                Destroy(gameObject);
                CloudSystem.Instance.NumberOfClouds -= 1;
            }
        }
        if (IsTopBottomCloud == true)
        {
            transform.Translate(Vector2.right * Speed);
        }
    }

    public GameObject TpObject;
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "LoopClouds")
        {
            Debug.Log("Tp Back");

            transform.position = TpObject.transform.position;
        }
    }
}
