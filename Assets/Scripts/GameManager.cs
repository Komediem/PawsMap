using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

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
    [HideInInspector] public float zoomPourcentage;
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
    public Button endGameButton;
    [HideInInspector] public bool infoPanelIsOpen;
    [HideInInspector] public bool isOnDetailedImage;
    [HideInInspector] public bool canSlide;
    [HideInInspector] public int currentImage;
    [HideInInspector] public InterestPointDatas currentInterestPoint;
    [HideInInspector] public InterestPoint currentInterestPointScript;
    [SerializeField] private Button researchButton;
    [SerializeField] private Button illustrationButton;
    [SerializeField] private Sprite backgroundPanelMajor;
    [SerializeField] private Sprite backgroundPanelMedium;
    private bool isOnButton;
    private float currentAlpha;
    private float alphaRef;
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

    #region Info Panel Datas

    [Header("Info Panel Texts")]
    [SerializeField] private GameObject Title;
    [SerializeField] private GameObject RegionName;
    [SerializeField] private GameObject PlaceIcon;
    [SerializeField] private GameObject ClimaticCondition;
    [SerializeField] private GameObject FaunaAndFlora;
    [SerializeField] private GameObject FrequentRessources;
    [SerializeField] private GameObject Dangerosity;
    [SerializeField] private GameObject panelDescription;
    [SerializeField] private GameObject panelDescriptionMediumPoint;
    [SerializeField] private GameObject interestPointImportance;
    [SerializeField] private Sprite interestPointMinor;
    [SerializeField] private Sprite interestPointMedium;
    [SerializeField] private Sprite interestPointMajor;
    #endregion

    #region movement and velocities
    Vector3 oldMousePos = Vector3.zero;
    private float zoomVel = 0;
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
    [SerializeField] private float zoomMovementSmoothTime;
    [SerializeField] private float moveSmoothTime;
    [SerializeField] private Vector3 smoothEffect;
    public AnimationCurve SmoothBorder;
    #endregion

    #region Cursors
    [Header("Cursors")]
    [SerializeField] private Texture2D cursorTextureDefault;
    [SerializeField] private Texture2D cursorTextureLoop;
    [SerializeField] private Vector2 clickPosition = Vector2.zero;
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
        endGameButton.gameObject.SetActive(false);

        SetToDefaultCursor();
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
            if(infoPanelIsOpen && Input.GetMouseButtonDown(1))
            {
                CloseInfoPanel();
                SetToDefaultCursor();
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
                    borderMoveAlphaX = Mathf.Abs(tempPos.x - borderMoveAlphaX);

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

            if(Input.GetMouseButton(0) && !isOnButton)
            {
                if(currentInterestPoint.isResearches)
                {
                    researchButton.Select();
                }
                else
                {
                    illustrationButton.Select();
                }
            }
        }
        
        else if(!isIntro)
        {
            var v = Input.GetAxis("Mouse ScrollWheel");
            //Vector3 zoomMovementVel = Vector3.zero;

            currentSize = Mathf.Clamp(currentSize - v * sensibilityScrollWheel, minSize, maxSize);
            cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, currentSize, ref zoomVel, zoomSmoothTime);

            newScreenPos = new Vector3(
                    Mathf.Clamp(newScreenPos.x, currentMap.bounds.min.x + cam.orthographicSize * cam.aspect, currentMap.bounds.max.x - cam.orthographicSize * cam.aspect),
                    Mathf.Clamp(newScreenPos.y, currentMap.bounds.min.y + cam.orthographicSize, currentMap.bounds.max.y - cam.orthographicSize),
                    newScreenPos.z);

            /*if(Input.GetAxis("Mouse ScrollWheel") != 0 || zoomMovementVel != Vector3.zero)
            {
                newScreenPos = Vector3.SmoothDamp(cam.transform.position, worldMousePos, ref zoomMovementVel, zoomMovementSmoothTime);
            }*/

            //print(zoomMovementVel);

            cam.transform.position = newScreenPos;

            oldMousePos = mousePos;
        }
    }

    public void SetToLoopCursor()
    {
        Cursor.SetCursor(cursorTextureLoop, clickPosition, CursorMode.ForceSoftware);
    }

    public void SetToDefaultCursor()
    {
        Cursor.SetCursor(cursorTextureDefault, clickPosition, CursorMode.ForceSoftware);
    }

    public void Reload()
    {
        SceneManager.LoadScene("Map");
    }

    #region Info Panel Utils

    public void OpenInfoPanel(InterestPointDatas interestPointDatasValue, InterestPoint interestPointScript)
    {
        infoPanel.GetComponent<Animator>().SetBool("IsOpen?", true);
        blackBackground.SetActive(true);
        //blackBackground.GetComponent<Animator>().SetBool("IsOpen?", true);
        infoPanelIsOpen = true;
        imageHolder.gameObject.SetActive(true);
        quitButton.gameObject.SetActive(true);
        currentInterestPointScript = interestPointScript;

        if (interestPointDatasValue.type == InterestPointType.major)
        {
            researchButton.gameObject.SetActive(true);
            illustrationButton.gameObject.SetActive(true);

            infoPanel.GetComponent<Image>().sprite = backgroundPanelMajor;

            //panelDescription.transform.localPosition = new Vector3(335, -680, 0);
            interestPointImportance.GetComponent<Image>().sprite = interestPointMajor;
        }
        else if (interestPointDatasValue.type == InterestPointType.medium)
        {
            researchButton.gameObject.SetActive(false);
            illustrationButton.gameObject.SetActive(false);

            infoPanel.GetComponent<Image>().sprite = backgroundPanelMedium;

            //panelDescription.transform.localPosition = new Vector3(335, -500, 0);
            interestPointImportance.GetComponent<Image>().sprite = interestPointMedium;
        }
        else if (interestPointDatasValue.type == InterestPointType.minor)
        {
            researchButton.gameObject.SetActive(false);
            illustrationButton.gameObject.SetActive(false);

            infoPanel.GetComponent<Image>().sprite = backgroundPanelMedium;

            //panelDescription.transform.localPosition = new Vector3(335, -500, 0);
            interestPointImportance.GetComponent<Image>().sprite = interestPointMinor;
        }

        SpawnTypeOfImage(interestPointDatasValue);
        AssignDatasInterestPoint(interestPointDatasValue);

        SpawnCurrentDot();
        if(currentImage == 0)
        {
            previousImage.SetActive(false);
        }
    }

    public void AssignDatasInterestPoint(InterestPointDatas interestPointDatasValue)
    {
        //Set All Texts
        Title.GetComponent<TextMeshProUGUI>().text = interestPointDatasValue.title;
        RegionName.GetComponent<TextMeshProUGUI>().text = interestPointDatasValue.subtitle;

        ClimaticCondition.GetComponent<TextMeshProUGUI>().text = interestPointDatasValue.climaticCondition;
        FaunaAndFlora.GetComponent<TextMeshProUGUI>().text = interestPointDatasValue.faunaAndFlora;
        FrequentRessources.GetComponent<TextMeshProUGUI>().text = interestPointDatasValue.frequentRessources;
        Dangerosity.GetComponent<TextMeshProUGUI>().text = interestPointDatasValue.dangerosity;

        if(interestPointDatasValue.type == InterestPointType.major)
        {
            panelDescriptionMediumPoint.SetActive(false);
            panelDescription.SetActive(true);
            panelDescription.GetComponent<TextMeshProUGUI>().text = interestPointDatasValue.currentImages[currentImage].imageDescription;

        }

        else
        {
            panelDescriptionMediumPoint.SetActive(true);
            panelDescription.SetActive(false);  
            panelDescriptionMediumPoint.GetComponent<TextMeshProUGUI>().text = interestPointDatasValue.currentImages[currentImage].imageDescription;
        }



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

        float ratio = placeImage.gameObject.GetComponent<RectTransform>().rect.width * currentInterestPoint.currentImages[currentImage].image.rect.height / currentInterestPoint.currentImages[currentImage].image.rect.width;
        placeImage.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(placeImage.gameObject.GetComponent<RectTransform>().rect.width, ratio);
        /*
        float ratioPrev = previousImage.gameObject.GetComponent<RectTransform>().rect.width * currentInterestPoint.currentImages[currentInterestPoint.currentImages.Count - 1].image.rect.height / currentInterestPoint.currentImages[currentInterestPoint.currentImages.Count - 1].image.rect.width;
        previousImage.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(previousImage.gameObject.GetComponent<RectTransform>().rect.width, ratioPrev);

        float ratioNext = nextImage.gameObject.GetComponent<RectTransform>().rect.width * currentInterestPoint.currentImages[currentImage + 1].image.rect.height / currentInterestPoint.currentImages[currentImage + 1].image.rect.width;
        nextImage.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(nextImage.gameObject.GetComponent<RectTransform>().rect.width, ratioNext);*/
    }

    public void IsOnAButton()
    {
        isOnButton = true;  
    }

    public void IsNotOnButton()
    {
        isOnButton = false;
    }

    public void SlideNextRight()
    {
        if(currentImage < currentInterestPoint.currentImages.Count - 1)
        {
            currentImage += 1;
            placeImage.GetComponent<Animator>().SetBool("IsSwitching", true);
            canSlide = false;
            UpdateCurrentDot();
        }
    }

    public void SlideNextLeft()
    {
        if (currentImage > 0)
        {
            currentImage -= 1;
            canSlide = false;
            placeImage.GetComponent<Animator>().SetBool("IsSwitchingBackward", true);
            UpdateCurrentDot();
        }
    }

    public void CloseInfoPanel()
    {
        if (isOnDetailedImage)
        {
            detailedImageBackground.SetActive(false);
            canSlide = true;
            isOnDetailedImage = false;

            if(currentInterestPoint.isResearches == false)
            {
                illustrationButton.Select();
            }
            else
            {
                researchButton.Select();
            }
        }

        else
        {
            currentInterestPointScript.GetComponent<Animator>().SetBool("IsClicked?", false);
            currentInterestPointScript.isClicked = false;

            currentInterestPoint = null;
            currentInterestPointScript = null;
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

            currentDots.Clear();
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

        StartCoroutine(LerpFunction(interestPointClicked, interestPointDatasValue, 6, 0.4f));
    }

    IEnumerator LerpFunction(GameObject interestPointClicked, InterestPointDatas interestPointDatasValue, float targetValue, float duration)
    {
        float time = 0;
        float speedZoom = 0;
        currentAlpha = interestPointClicked.GetComponent<SpriteRenderer>().color.a;
        Vector3 speed = Vector3.zero;
        Vector3 target = interestPointClicked.transform.position;
        target.z = cam.transform.position.z;
        var interestPointScript = interestPointClicked.GetComponent<InterestPoint>();

        while (Vector3.Distance(cam.transform.position,target) > 0.03f)
        {
            cam.transform.position = Vector3.SmoothDamp(cam.transform.position, target, ref speed, duration);
            currentSize = Mathf.SmoothDamp(currentSize, targetValue, ref speedZoom, duration);

            time += Time.deltaTime;

            yield return null;
        }

        OpenInfoPanel(interestPointDatasValue, interestPointScript);
        currentSize = targetValue;
    }

    #endregion

    #region Intro

    public void PassToIntroPanel()
    {
        SetToDefaultCursor();
        introPanel.GetComponent<Animator>().SetBool("IsStarted?", true);
        introPanel.SetActive(false);
        infoTextPanel.SetActive(true);

        this.gameObject.GetComponent<AudioSource>().Play();
    }

    public void StartPlaying()
    {
        infoTextPanel.SetActive(false);

        currentSize = 20;

        StartCoroutine(IntroEnd());
    }

    IEnumerator IntroEnd()
    {
        infoTextPanel.SetActive(false);
        float zoomVel = 0;
        float zoomSpeed = 0.4f;

        while (cam.orthographicSize > currentSize + 0.03)
        {
            cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, currentSize, ref zoomVel, zoomSpeed);

            yield return null;
        }

        isIntro = false;
        endGameButton.gameObject.SetActive(true);
    }

    #endregion

    #region ResearchToIllustration

    private void SpawnTypeOfImage(InterestPointDatas interestPointDatasValue)
    {
        //Set Datas to interest point clicked
        currentInterestPoint = interestPointDatasValue;

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

            previousImage.SetActive(false);
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

            previousImage.SetActive(false);
        }
    }

    #endregion
}
