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
    [SerializeField] private Vector3 hoveredScale;
    [SerializeField] private float hoveredScalingSpeed;
    private bool isHovered;
    private bool isVisible;
    [HideInInspector] public bool isClicked;

    void Start()
    {
        //baseImage = this.GetComponent<SpriteRenderer>().sprite;
        baseScale = this.gameObject.transform.localScale;

        pointName.GetComponent<TextMeshPro>().text = interestPointDatas.title;
        pointName.SetActive(false);

        baseImage = interestPointDatas.iconBase;
        this.GetComponent<SpriteRenderer>().sprite = baseImage;

        if (GameManager.Instance.currentSize > spawnThreshold)
        {
            isVisible = false;
            pointName.SetActive(false);
            this.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.Instance.currentSize <= spawnThreshold)
        {
            this.GetComponent<Animator>().SetBool("IsVisible?", true);
            this.GetComponent<SpriteRenderer>().enabled = true;
        }
        else if(GameManager.Instance.currentSize > spawnThreshold)
        {
            this.GetComponent<Animator>().SetBool("IsVisible?", false);
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
        if(!isClicked)
        {
            //Feedback hovering
            this.GetComponent<SpriteRenderer>().sprite = interestPointDatas.iconHovered;

            //Utils
            isHovered = true;
            pointName.SetActive(isVisible);
        }
    }

    public void ResetImage()
    {
        //Feedback reseting image
        this.GetComponent<SpriteRenderer>().sprite = interestPointDatas.iconBase;

        //Utils
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
                GameManager.Instance.SmoothZoomInterestPoint(this.gameObject, interestPointDatas);
                this.GetComponent<Animator>().SetBool("IsClicked?", true);
                isClicked = true;

                ResetImage();
            }

        }
    }

    public void Activate()
    {
        isVisible = true;
        if(isHovered)
        pointName.SetActive(true);
    }

    public void Deactivate()
    {
        isVisible = false;
        pointName.SetActive(false);
        this.GetComponent<SpriteRenderer>().enabled = false;
    }
}
