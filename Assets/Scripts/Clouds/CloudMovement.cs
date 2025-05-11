using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class CloudMovement : MonoBehaviour
{

    public float MinSpeed;
    public float MaxSpeed;
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
    private bool IsAppearing = true;
    private bool IsDestroying = false;

    void Start()
    {
        //Sets the Speed of the Cloud.
        Speed = Random.Range(MinSpeed, MaxSpeed);
        Speed = Speed * Time.deltaTime;

        //Sets a random opacity between two values.
        float CloudRandomOpacity = Random.Range(MinCloudOpacity, MaxCloudOpacity);
        CloudRandomOpacity = CloudRandomOpacity / 255;
        
        if(IsTopBottomCloud == true)
        {
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, CloudRandomOpacity);
        }
        else
        {
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        }

        Opacity = CloudRandomOpacity;

        //Sets the threshold at which the cloud fades aways when zooming in.
        HidingThreshold = Random.Range(MinHidingThreshold, MaxHidingThreshold);


    }

    void FixedUpdate()
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


        if(IsAppearing & IsTopBottomCloud == false)
        {
            SpawningOpacityChange();
        }

        if (IsTopBottomCloud == false)
        {
            transform.position = Vector2.MoveTowards(transform.position, Objective, Speed);

            if (transform.position == new Vector3(Objective.x, Objective.y, 0))
            {
                CloudSystem.Instance.NumberOfClouds -= 1;
                IsDestroying = true;
            }
        }
        if (IsTopBottomCloud == true)
        {
            transform.Translate(Vector2.right * Speed);
        }

        if(IsDestroying == true)
        {
            float i = Mathf.Lerp(this.GetComponent<SpriteRenderer>().color.a, 0, Time.deltaTime * 5);
            this.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, i);

            if(GetComponent<SpriteRenderer>().color.a <= i)
            {
                Destroy(gameObject);
            }
        }
    }

    void SpawningOpacityChange()
    {
        if(GetComponent<SpriteRenderer>().color.a != Opacity)
        {
            float i = Mathf.Lerp(this.GetComponent<SpriteRenderer>().color.a, Opacity, Time.deltaTime / 10);
            this.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, i);
        }
        else
        {
            IsAppearing = false;
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
