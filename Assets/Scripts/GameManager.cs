using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

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
    public GameObject infoTextPanel;
    public GameObject blackBackground;
    public GameObject imageHolder;
    public GameObject detailedImageBackground;
    public GameObject detailedImage;
    public Button placeImage;
    public GameObject previousImage;
    public GameObject nextImage;
    public Button quitButton;
    [HideInInspector] public bool infoPanelIsOpen;
    [HideInInspector] public bool isOnDetailedImage;
    [HideInInspector] public bool canSlide;
    private int currentImage;
    [HideInInspector] public InterestPointDatas currentInterestPoint;
    [SerializeField] private Button researchButton;
    [SerializeField] private Button illustrationButton;
    #endregion

    #region Dots
    [Header("Dots")]
    [SerializeField] private GameObject dot;
    [SerializeField] private GameObject dotHolder;
    private GameObject currentLightedDot;
    [HideInInspector] public List<GameObject> currentDots = new();
    [SerializeField] private Sprite baseDot;
    [SerializeField] private Sprite activeDot;

    #endregion

    #region Info Panel Texts

    [Header("Info Panel Texts")]
    [SerializeField] private GameObject Title;
    [SerializeField] private GameObject RegionName;
    [SerializeField] private GameObject PlaceIcon;
    [SerializeField] private GameObject ClimaticCondition;
    [SerializeField] private GameObject FaunaAndFlora;
    [SerializeField] private GameObject FrequentRessources;
    [SerializeField] private GameObject Dangerosity;
    [SerializeField] private GameObject panelDescription;
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
                //Can't Move !
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
            if(Input.GetAxis("Mouse ScrollWheel") > 0 && currentInterestPoint.currentImages.Count > 1 && canSlide)
            {
                SlideNextLeft();
            }

            else if (Input.GetAxis("Mouse ScrollWheel") < 0 && currentInterestPoint.currentImages.Count > 1 && canSlide)
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

    #region Info Panel Utils

    public void OpenInfoPanel(InterestPointDatas interestPointDatasValue)
    {
        infoPanel.GetComponent<Animator>().SetBool("IsOpen?", true);
        blackBackground.SetActive(true);
        //blackBackground.GetComponent<Animator>().SetBool("IsOpen?", true);
        infoPanelIsOpen = true;
        imageHolder.gameObject.SetActive(true);
        quitButton.gameObject.SetActive(true);
        AssignDatasInterestPoint(interestPointDatasValue);

        SpawnCurrentDot();
        SpawnTypeOfImage();
    }

    public void AssignDatasInterestPoint(InterestPointDatas interestPointDatasValue)
    {
        //Set Datas to interest point clicked
        currentInterestPoint = interestPointDatasValue;

        //Set All Texts
        Title.GetComponent<TextMeshProUGUI>().text = interestPointDatasValue.title;
        RegionName.GetComponent<TextMeshProUGUI>().text = interestPointDatasValue.subtitle;

        ClimaticCondition.GetComponent<TextMeshProUGUI>().text = interestPointDatasValue.climaticCondition;
        FaunaAndFlora.GetComponent<TextMeshProUGUI>().text = interestPointDatasValue.faunaAndFlora;
        FrequentRessources.GetComponent<TextMeshProUGUI>().text = interestPointDatasValue.frequentRessources;
        Dangerosity.GetComponent<TextMeshProUGUI>().text = interestPointDatasValue.dangerosity;

        panelDescription.GetComponent<TextMeshProUGUI>().text = interestPointDatasValue.currentImages[currentImage].imageDescription;
        PlaceIcon.GetComponent<Image>().sprite = interestPointDatasValue.iconHovered;

        //Assign Image
        placeImage.image.sprite = interestPointDatasValue.currentImages[currentImage].image;


        if (currentInterestPoint.currentImages.Count > 2)
        {
            previousImage.SetActive(true);
            nextImage.SetActive(true);

            //IMAGE 0
            if (currentImage == 0)
            {
                previousImage.GetComponent<Image>().sprite = interestPointDatasValue.currentImages[currentInterestPoint.currentImages.Count - 1].image;

                nextImage.GetComponent<Image>().sprite = interestPointDatasValue.currentImages[currentImage + 1].image;
            }

            //IMAGE MAX
            else if (currentImage >= currentInterestPoint.currentImages.Count - 1)
            {
                previousImage.GetComponent<Image>().sprite = interestPointDatasValue.currentImages[currentImage - 1].image;

                nextImage.GetComponent<Image>().sprite = interestPointDatasValue.currentImages[0].image;
            }

            //IMAGE INTERMEDIATE
            else if (currentImage > 0)
            {
                previousImage.GetComponent<Image>().sprite = interestPointDatasValue.currentImages[currentImage - 1].image;

                nextImage.GetComponent<Image>().sprite = interestPointDatasValue.currentImages[currentImage + 1].image;
            }
        }

        else if (currentInterestPoint.currentImages.Count == 2)
        {
            //IMAGE 0
            if (currentImage == 0)
            {
                nextImage.GetComponent<Image>().sprite = interestPointDatasValue.currentImages[1].image;
                previousImage.SetActive(false);
                nextImage.SetActive(true);
            }

            if (currentImage == 1)
            {
                previousImage.GetComponent<Image>().sprite = interestPointDatasValue.currentImages[0].image;
                previousImage.SetActive(true);
                nextImage.SetActive(false);
            }
        }

        else
        {
            previousImage.SetActive(false);
            nextImage.SetActive(false);
        }

        float ratio = currentInterestPoint.currentImages[currentImage].image.rect.height / currentInterestPoint.currentImages[currentImage].image.rect.width;
        placeImage.transform.localScale = new Vector3(1, ratio * 2.5f, 1);

        /*
        float ratioPrev = currentInterestPoint.currentImages[currentInterestPoint.currentImages.Count - 1].image.rect.height / currentInterestPoint.currentImages[currentInterestPoint.currentImages.Count - 1].image.rect.width;
        previousImage.transform.localScale = new Vector3(1, ratioPrev * 2, 1);

        float ratioNext = currentInterestPoint.currentImages[currentImage + 1].image.rect.height / currentInterestPoint.currentImages[currentImage + 1].image.rect.width;
        nextImage.transform.localScale = new Vector3(1, ratioNext * 2, 1);
        */
    }

    public void SlideNextRight()
    {
        currentImage = (currentImage + 1) % currentInterestPoint.currentImages.Count;

        placeImage.GetComponent<Animator>().SetBool("IsSwitching", true);

        canSlide = false;

        UpdateCurrentDot();
    }

    public void SlideNextLeft()
    {
        if (currentImage > 0)
        {
            currentImage -= 1;
        }
        else
        {
            currentImage = currentInterestPoint.currentImages.Count - 1;
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
        detailedImage.GetComponent<Image>().sprite = currentInterestPoint.currentImages[currentImage].image;
        float ratio = currentInterestPoint.currentImages[currentImage].image.rect.height / currentInterestPoint.currentImages[currentImage].image.rect.width;
        detailedImage.transform.localScale = new Vector3(1, ratio, 1);
        canSlide = false;
    }

    public void SpawnCurrentDot()
    {
        if(currentDots.Count > 0)
        {
            foreach(GameObject dot in currentDots)
            {
                Destroy(dot);
            }

            for(var i = 0; i < currentDots.Count; i++)
            {
                currentDots.RemoveAt(i);
            }
        }

        //Create white dots depending on the number of images
        if (currentInterestPoint.currentImages.Count > 1)
        {
            for (int i = 0; i < currentInterestPoint.currentImages.Count; i++)
            {
                var dotClone = Instantiate(dot, dotHolder.transform);
                currentDots.Add(dotClone);
            }
        }

        print("SpawnDot");

        UpdateCurrentDot();
    }

    public void UpdateCurrentDot()
    {
        if (currentInterestPoint.currentImages.Count > 1)
        {
            if (currentLightedDot != null)
            {
                currentLightedDot.GetComponent<Animator>().SetBool("IsLightedUp", false);
                currentLightedDot.GetComponent<Image>().sprite = baseDot;
            }

            currentLightedDot = currentDots[currentImage];
            currentLightedDot.GetComponent<Animator>().SetBool("IsLightedUp", true);
            currentLightedDot.GetComponent<Image>().sprite = activeDot;
        }
    }

    public void SmoothZoomInterestPoint(GameObject interestPointClicked, InterestPointDatas interestPointDatasValue)
    {
        StartCoroutine(LerpFunction(interestPointClicked, interestPointDatasValue, 6, 1));
    }

    IEnumerator LerpFunction(GameObject interestPointClicked, InterestPointDatas interestPointDatasValue, float targetValue, float duration)
    {
        float time = 0;
        float startValue = currentSize;

        while (time < duration)
        {
            currentSize = Mathf.Lerp(currentSize, targetValue, time / duration);

            Vector3 target = interestPointClicked.transform.position;
            target.Set(interestPointClicked.transform.position.x, interestPointClicked.transform.position.y, cam.transform.position.z);

            cam.transform.position = Vector3.Lerp(cam.transform.position, target, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        OpenInfoPanel(interestPointDatasValue);
        currentSize = targetValue;
    }

    #endregion

    #region Intro

    public void IntroStart()
    {
        introPanel.GetComponent<Animator>().SetBool("IsStarted?", true);
        currentSize = 20;
        cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, currentSize, ref zoomVel, 5);
        StartCoroutine(IntroEnd());
    }

    IEnumerator IntroEnd()
    {
        yield return new WaitForSeconds(0.5f);

        introPanel.SetActive(false);
        infoTextPanel.SetActive(true);
    }

    public void StartPlaying()
    {
        infoTextPanel.SetActive(false);
        isIntro = false;
    }

    #endregion

    #region ResearchToIllustration

    private void SpawnTypeOfImage()
    {
        //Set base images to illustrations
        currentInterestPoint.currentImages = currentInterestPoint.Illustrations;
        currentInterestPoint.isResearches = false;

        illustrationButton.Select();
        AssignDatasInterestPoint(currentInterestPoint);
    }

    public void SwitchImagesToResearches()
    {
        //Switch to Researches
        if (!currentInterestPoint.isResearches)
        {
            currentImage = 0;

            currentInterestPoint.currentImages = currentInterestPoint.Researches;
            currentInterestPoint.isResearches = true;

            AssignDatasInterestPoint(currentInterestPoint);
            SpawnCurrentDot();
        }
    }

    public void SwitchImagesToIllustrations()
    {
        //Switch to Illustrations
        if (currentInterestPoint.isResearches)
        {
            currentImage = 0;

            currentInterestPoint.currentImages = currentInterestPoint.Illustrations;
            currentInterestPoint.isResearches = false;

            AssignDatasInterestPoint(currentInterestPoint);
            SpawnCurrentDot();
        }
    }

    #endregion
}
