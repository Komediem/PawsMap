using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InterestPoint : MonoBehaviour
{
    [Header("Rendering")]
    [SerializeField] private float spawnThreshold;

    [Header("Basic Caracteristics")]
    private Sprite baseImage;
    private Vector3 baseScale;

    [Header("DescriptionPanel")]
    [SerializeField] public string Title;
    [SerializeField] public Sprite PlaceImage;
    [SerializeField] public string Description;

    [Header("HoveredFeedbacks")]
    [SerializeField] private Sprite hoveredImage;
    [SerializeField] private Vector3 hoveredScale;
    [SerializeField] private float hoveredScalingSpeed;
    private bool isHovered;

    void Start()
    {
        baseImage = this.GetComponent<SpriteRenderer>().sprite;
        baseScale = this.gameObject.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.Instance.currentSize <= spawnThreshold)
        {
            this.GetComponent<SpriteRenderer>().enabled = true;
        }
        else if(GameManager.Instance.currentSize > spawnThreshold)
        {
            this.GetComponent<SpriteRenderer>().enabled = false;
        }

        if (isHovered)
        {
            this.transform.localScale = Vector3.Lerp(transform.localScale, hoveredScale, hoveredScalingSpeed * Time.deltaTime);
        }

        else
        {
            this.transform.localScale = Vector3.Lerp(transform.localScale, baseScale, hoveredScalingSpeed * Time.deltaTime);
        }
    }

    public void Hovered()
    {
        //print("Hovered");
        this.GetComponent<SpriteRenderer>().sprite = hoveredImage;
        isHovered = true;
    }

    public void ResetImage()
    {
        //print("Reset");
        this.GetComponent<SpriteRenderer>().sprite = baseImage;
        isHovered = false;
    }

    public void Click()
    {
        if(GameManager.Instance.infoPanelIsOpen)
        {
            GameManager.Instance.CloseInfoPanel();
        }

        else
        {
            GameManager.Instance.OpenInfoPanel(Title, Description, PlaceImage);
        }
    }
}
