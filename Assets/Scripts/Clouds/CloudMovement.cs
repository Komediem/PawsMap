using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class CloudMovement : MonoBehaviour
{

    [HideInInspector] public float MinSpeed;
    [HideInInspector] public float MaxSpeed;
    [Space]
    [SerializeField] private int MinCloudOpacity;
    [SerializeField] private int MaxCloudOpacity = 255;
    [SerializeField] private bool IsTopBottomCloud = false;
    
    private float HidingThreshold;
    [Space]
    [SerializeField] private float MinHidingThreshold;
    [SerializeField] private float MaxHidingThreshold;


    [Header("Private Data")]
    public float Speed;
    [HideInInspector] public Vector2 Objective;
    [HideInInspector] public int ID;
    private float Opacity;

    void Start()
    {
        Speed = Random.Range(MinSpeed, MaxSpeed);
        Speed = Speed * Time.deltaTime;

        float CloudRandomOpacity = Random.Range(MinCloudOpacity, MaxCloudOpacity);
        CloudRandomOpacity = CloudRandomOpacity / 255;
        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, CloudRandomOpacity);
        Opacity = CloudRandomOpacity;

        HidingThreshold = Random.Range(MinHidingThreshold, MaxHidingThreshold);
    }

    void Update()
    {
        if (GameManager.Instance.currentSize < HidingThreshold)
        {
            float i = Mathf.Lerp(this.GetComponent<SpriteRenderer>().color.a, 0, Time.deltaTime * 5);
            this.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, i);
        }
        else if (GameManager.Instance.currentSize > HidingThreshold)
        {
            float i = Mathf.Lerp(this.GetComponent<SpriteRenderer>().color.a, Opacity, Time.deltaTime * 5);
            this.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, i);
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
