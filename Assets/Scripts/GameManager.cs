using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private Camera cam;
    private float currentSize;

    [Header("Info Panel")]
    public GameObject infoPanel;
    public bool infoPanelIsOpen;

    [Header("Camera Parameters")]
    [SerializeField] private float maxSize;
    [SerializeField] private float minSize;

    [Header("Sensibility And Controls")]
    [SerializeField] private float sensibility;

    private void Awake()
    {
        if(Instance == null)
        Instance = this;
    }

    void Start()
    {
        cam = Camera.main;
        currentSize = cam.orthographicSize;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = Input.mousePosition;

        if (Input.GetMouseButton(1))
        {
            cam.transform.position = cam.transform.position + new Vector3(mousePos.x, mousePos.y, 0f);
        }

        var v = Input.GetAxis("Mouse ScrollWheel");

        if (v > 0 && currentSize >= minSize)
        {
            //print("Zoom");
            cam.orthographicSize -= v * sensibility;
            currentSize = cam.orthographicSize;
        }
        else if (v < 0 && currentSize <= maxSize)
        {
            //print("Dezoom");
            cam.orthographicSize -= v * sensibility;
            currentSize = cam.orthographicSize;
        }
    }
}
