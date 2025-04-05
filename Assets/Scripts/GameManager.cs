using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    #region Intro

    [Header("Intro")]
    [SerializeField] private GameObject introPanel;
    [SerializeField] public bool isIntro;

    #endregion

    #region Map
    [Header("Map")]
    [SerializeField] private SpriteRenderer currentMap;
    [HideInInspector] public float currentSize;
    [HideInInspector] public float mapBorder;
    #endregion

    #region Info Panel
    [Header("Info Panel")]
    public GameObject infoPanel;
    public GameObject panelTitle;
    public GameObject panelDescription;
    public GameObject blackBackground;
    public GameObject imageHolder;
    public GameObject detailedImageBackground;
    public GameObject detailedImage;
    [SerializeField] private GameObject dot;
    [SerializeField] private GameObject dotHolder;
    private GameObject currentLightedDot;
    [HideInInspector] public List<GameObject> currentDots = new();
    public Button placeImage;
    public Button quitButton;
    [HideInInspector] public bool infoPanelIsOpen;
    [HideInInspector] public bool isOnDetailedImage;
    [HideInInspector] public bool canSlide;
    private int currentImage;
    [HideInInspector] public InterestPointDatas currentInterestPoint;
    #endregion

    #region movement and velocities
    Vector3 oldMousePos = Vector3.zero;
    Vector3 newTargetPos = Vector3.zero;
    private float zoomVel = 0;
    private Vector3 moveVel = Vector3.zero;
    #endregion

    #region Camera Parameters
    [Header("Camera Parameters")]
    [SerializeField] private float maxSize;
    [SerializeField] private float minSize;
    private Camera cam;
    #endregion

    #region Sensibility and Controls
    [Header("Sensibility And Controls")]
    [SerializeField] private float sensibilityMin;
    [SerializeField] private float sensibilityMax;
    private float currentSensibility;
    [SerializeField] private float sensibilityScrollWheel; 
    [SerializeField] private float zoomSmoothTime;
    [SerializeField] private float moveSmoothTime;
    public AnimationCurve SmoothBorder;
    #endregion

    private void Awake()
    {
        if(Instance == null)
        Instance = this;


        cam = Camera.main;
        currentSize = cam.orthographicSize;
        blackBackground.SetActive(false);
        canSlide = true;
        isIntro = true;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = Input.mousePosition;

        float currentSizeLerp = Mathf.InverseLerp(minSize, maxSize, currentSize);

        currentSensibility = Mathf.Lerp(sensibilityMin, sensibilityMax, currentSizeLerp);

        Vector3 newScreenPos = cam.transform.position;

        if (Input.GetMouseButton(1) || Input.GetMouseButton(0) && !isIntro)
        {
            if(infoPanelIsOpen && Input.GetMouseButton(1))
            {
                CloseInfoPanel();
            }

            if(infoPanelIsOpen && Input.GetMouseButton(0))
            {
                Debug.Log("Can't Move for now");
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

        if(infoPanelIsOpen)
        {
            if(Input.GetAxis("Mouse ScrollWheel") > 0 && currentInterestPoint.interestPointMultipleDatas.Count > 1 && canSlide)
            {
                SlideNextLeft();
            }

            else if (Input.GetAxis("Mouse ScrollWheel") < 0 && currentInterestPoint.interestPointMultipleDatas.Count > 1 && canSlide)
            {
                SlideNextRight();
            }
        }
        
        else if(!isIntro)
        {
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

        //Create white dots depending on the number of images
        if (currentInterestPoint.interestPointMultipleDatas.Count > 1)
        {
            for (int i = 0; i < currentInterestPoint.interestPointMultipleDatas.Count; i++)
            {
                var dotClone = Instantiate(dot, dotHolder.transform);
                currentDots.Add(dotClone);
            }
        }

        UpdateCurrentDot();
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

        placeImage.GetComponent<Animator>().SetBool("IsSwitching", true);
        canSlide = false;

        UpdateCurrentDot();
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

        canSlide = false;
        placeImage.GetComponent<Animator>().SetBool("IsSwitchingBackward", true);

        UpdateCurrentDot();
    }

    public void CloseInfoPanel()
    {
        if (isOnDetailedImage)
        {
            detailedImageBackground.SetActive(false);
            canSlide = true;
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

            //Destroy all the current white dots if there are already
            if (currentDots.Count > 0)
            {
                foreach (GameObject currentDot in currentDots)
                {
                    Destroy(currentDot);
                }

                currentDots.Clear();
            }
        }
    }

    public void OpenDetailedImage()
    {
        isOnDetailedImage = true;
        detailedImageBackground.SetActive(true);
        detailedImage.GetComponent<Image>().sprite = currentInterestPoint.interestPointMultipleDatas[currentImage].image;
        float ratio = currentInterestPoint.interestPointMultipleDatas[currentImage].image.rect.height / currentInterestPoint.interestPointMultipleDatas[currentImage].image.rect.width;
        detailedImage.transform.localScale = new Vector3(1, ratio, 1);
        canSlide = false;
    }

    public void IntroStart()
    {
        IntroButton.instance.ResetImage();
        introPanel.GetComponent<Animator>().SetBool("IsStarted?", true);
        currentSize = 20;
        cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, currentSize, ref zoomVel, zoomSmoothTime);
        StartCoroutine(IntroEnd());
    }

    IEnumerator IntroEnd()
    {
        yield return new WaitForSeconds(0.5f);

        introPanel.SetActive(false);
        isIntro = false;
    }

    public void UpdateCurrentDot()
    {
        if (currentLightedDot != null)
            currentLightedDot.GetComponent<Animator>().SetBool("IsLightedUp", false);

        currentLightedDot = currentDots[currentImage];
        currentLightedDot.GetComponent<Animator>().SetBool("IsLightedUp", true);
    }
}
