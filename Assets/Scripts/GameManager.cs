using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private SpriteRenderer currentMap;

    private Camera cam;
    public float currentSize;
    public float mapBorder;
    public AnimationCurve SmoothBorder;

    [Header("Info Panel")]
    public GameObject infoPanel;
    public GameObject panelTitle;
    public GameObject panelDescription;
    public GameObject blackBackground;
    public GameObject imageHolder;
    public GameObject detailedImageBackground;
    public GameObject detailedImage;
    public List<GameObject> changeImageArrows = new();
    public Button placeImage;
    public Button quitButton;
    public bool infoPanelIsOpen;
    public bool isOnDetailedImage;
    private int currentImage;
    private InterestPointDatas currentInterestPoint;

    Vector3 oldMousePos = Vector3.zero;
    Vector3 newTargetPos = Vector3.zero;

    private float zoomVel = 0;
    private Vector3 moveVel = Vector3.zero;

    [Header("Camera Parameters")]
    [SerializeField] private float maxSize;
    [SerializeField] private float minSize;

    [Header("Sensibility And Controls")]
    [SerializeField] private float sensibilityMin;
    [SerializeField] private float sensibilityMax;
    private float currentSensibility;
    [SerializeField] private float sensibilityScrollWheel; 
    [SerializeField] private float zoomSmoothTime;
    [SerializeField] private float moveSmoothTime;

    private void Awake()
    {
        if(Instance == null)
        Instance = this;
    }

    void Start()
    {
        cam = Camera.main;
        currentSize = cam.orthographicSize;
        blackBackground.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = Input.mousePosition;

        float currentSizeLerp = Mathf.InverseLerp(minSize, maxSize, currentSize);

        currentSensibility = Mathf.Lerp(sensibilityMin, sensibilityMax, currentSizeLerp);

        Vector3 newScreenPos = cam.transform.position;

        if (Input.GetMouseButton(1))
        {
            if(infoPanelIsOpen)
            {
                CloseInfoPanel();
            }

            else
            {
                Vector3 deltaPos = mousePos - oldMousePos;

                Vector3 tempPos = cam.transform.position + new Vector3(-deltaPos.x * currentSensibility, -deltaPos.y * currentSensibility, 0f);

                float borderMoveAlphaX = 1;
                if (mapBorder != 0)
                {
                    //Get Size
                    borderMoveAlphaX = ((currentMap.bounds.max.x - cam.orthographicSize * cam.aspect) - (currentMap.bounds.min.x + cam.orthographicSize * cam.aspect));


                    borderMoveAlphaX /= 2;
                    //Get the max distance
                    float maxDist = Mathf.Abs(borderMoveAlphaX - mapBorder);


                    borderMoveAlphaX = Mathf.Abs(tempPos.x - borderMoveAlphaX);
                    //Debug.Log(borderMoveAlphaX);

                    borderMoveAlphaX = SmoothBorder.Evaluate(borderMoveAlphaX);

                    borderMoveAlphaX = 1;

                }

                newScreenPos = cam.transform.position + new Vector3(-deltaPos.x * currentSensibility * borderMoveAlphaX, -deltaPos.y * currentSensibility, 0f);
            }
        }

        var v = Input.GetAxis("Mouse ScrollWheel");

        currentSize = Mathf.Clamp(currentSize - v * sensibilityScrollWheel, minSize, maxSize);
        cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, currentSize, ref zoomVel, zoomSmoothTime);

        newScreenPos = new Vector3(
                Mathf.Clamp(newScreenPos.x, currentMap.bounds.min.x + cam.orthographicSize * cam.aspect, currentMap.bounds.max.x - cam.orthographicSize * cam.aspect),
                Mathf.Clamp(newScreenPos.y, currentMap.bounds.min.y + cam.orthographicSize, currentMap.bounds.max.y - cam.orthographicSize),
                newScreenPos.z);

        cam.transform.position = newScreenPos;

        oldMousePos = mousePos;
    }

    public void OpenInfoPanel(InterestPointDatas interestPointDatasValue)
    {
        infoPanel.GetComponent<Animator>().SetBool("IsOpen?", true);
        blackBackground.SetActive(true);
        //blackBackground.GetComponent<Animator>().SetBool("IsOpen?", true);
        infoPanelIsOpen = true;
        imageHolder.gameObject.SetActive(true);
        quitButton.gameObject.SetActive(true);
        AssignDatasInterestPoint(interestPointDatasValue);

        if(interestPointDatasValue.interestPointMultipleDatas.Count > 1)
        {
            foreach(var arrows in changeImageArrows)
            {
                arrows.SetActive(true);
                print(interestPointDatasValue.interestPointMultipleDatas.Count);
            }
        }
        else
        {
            foreach (var arrows in changeImageArrows)
            {
                arrows.SetActive(false);
                print(interestPointDatasValue.interestPointMultipleDatas.Count);
            }
        }

    }

    public void AssignDatasInterestPoint(InterestPointDatas interestPointDatasValue)
    {
        //Set Datas to interest point clicked
        currentInterestPoint = interestPointDatasValue;
        panelTitle.GetComponent<TextMeshProUGUI>().text = interestPointDatasValue.title;
        panelDescription.GetComponent<TextMeshProUGUI>().text = interestPointDatasValue.interestPointMultipleDatas[currentImage].imageDescription;
        placeImage.image.sprite = interestPointDatasValue.interestPointMultipleDatas[currentImage].image;


        float ratio = currentInterestPoint.interestPointMultipleDatas[currentImage].image.rect.height / currentInterestPoint.interestPointMultipleDatas[currentImage].image.rect.width;
        placeImage.transform.localScale = new Vector3(1, ratio, 1);
    }

    public void SlideNextRight()
    {
        currentImage = (currentImage + 1) % currentInterestPoint.interestPointMultipleDatas.Count;

        AssignDatasInterestPoint(currentInterestPoint);
    }

    public void SlideNextLeft()
    {
        if(currentImage > 0)
        {
            currentImage -= 1;
        }
        else
        {
            currentImage = currentInterestPoint.interestPointMultipleDatas.Count - 1;
        }

        AssignDatasInterestPoint(currentInterestPoint);
    }

    public void CloseInfoPanel()
    {
        if (isOnDetailedImage)
        {
            detailedImageBackground.SetActive(false);
            isOnDetailedImage = false;
        }
        else
        {
            currentInterestPoint = null;
            infoPanel.GetComponent<Animator>().SetBool("IsOpen?", false);
            blackBackground.SetActive(false);
            infoPanelIsOpen = false;
            imageHolder.gameObject.SetActive(false);
            quitButton.gameObject.SetActive(false);
            currentImage = 0;
        }
    }

    public void OpenDetailedImage()
    {
        isOnDetailedImage = true;
        detailedImageBackground.SetActive(true);
        detailedImage.GetComponent<Image>().sprite = currentInterestPoint.interestPointMultipleDatas[currentImage].image;
        float ratio = currentInterestPoint.interestPointMultipleDatas[currentImage].image.rect.width/ currentInterestPoint.interestPointMultipleDatas[currentImage].image.rect.height;
        detailedImage.transform.localScale = new Vector3(ratio,1, 1);
    }
}
