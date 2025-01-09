using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private SpriteRenderer currentMap;

    private Camera cam;
    private float currentSize;

    [Header("Info Panel")]
    public GameObject infoPanel;
    public GameObject blackBackground;
    public bool infoPanelIsOpen;

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
            Vector3 deltaPos = mousePos - oldMousePos;

            newScreenPos = cam.transform.position + new Vector3(-deltaPos.x * currentSensibility, -deltaPos.y * currentSensibility, 0f);
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

    public void OpenInfoPanel()
    {
        infoPanel.GetComponent<Animator>().SetBool("IsOpen?", true);
        blackBackground.SetActive(true);
        blackBackground.GetComponent<Animator>().SetBool("IsOpen?", true);
        infoPanelIsOpen = true;
    }

    public void CloseInfoPanel()
    {
        infoPanel.GetComponent<Animator>().SetBool("IsOpen?", false);
        blackBackground.SetActive(false);
        infoPanelIsOpen = false;
    }
}
