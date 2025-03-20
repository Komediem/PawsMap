using TMPro;
using UnityEngine;

public class InterestPoint : MonoBehaviour
{
    [Header("Rendering")]
    [SerializeField] private float spawnThreshold;
    [SerializeField] private GameObject pointName;

    [Header("Basic Characteristics")]
    private Sprite baseImage;
    private Vector3 baseScale;

    [SerializeField] public InterestPointDatas interestPointDatas;

    [Header("HoveredFeedbacks")]
    [SerializeField] private Sprite hoveredImage;
    [SerializeField] private Vector3 hoveredScale;
    [SerializeField] private float hoveredScalingSpeed;
    private bool isHovered;
    private bool isVisible;

    void Start()
    {
        baseImage = this.GetComponent<SpriteRenderer>().sprite;
        baseScale = this.gameObject.transform.localScale;

        pointName.GetComponent<TextMeshPro>().text = interestPointDatas.title;
        pointName.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.Instance.currentSize <= spawnThreshold)
        {
            isVisible = true;
            this.GetComponent<SpriteRenderer>().enabled = true;
        }
        else if(GameManager.Instance.currentSize > spawnThreshold)
        {
            isVisible = false;
            pointName.SetActive(false);
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
        pointName.SetActive(isVisible);
    }

    public void ResetImage()
    {
        //print("Reset");
        this.GetComponent<SpriteRenderer>().sprite = baseImage;
        isHovered = false;
        pointName.SetActive(false);
    }

    public void Click()
    {
        if(isVisible)
        {
            if (GameManager.Instance.infoPanelIsOpen)
            {
                GameManager.Instance.CloseInfoPanel();
            }

            else
            {
                GameManager.Instance.OpenInfoPanel(interestPointDatas);
            }

        }
    }
}
