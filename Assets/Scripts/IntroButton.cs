using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class IntroButton : MonoBehaviour
{
    private bool isHovered;
    private Vector3 baseScale; 
    [SerializeField] private Vector3 hoveredScale;
    [SerializeField] private float hoveredScalingSpeed;

    public void Update()
    {
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
        //
    }

    public void ResetImage()
    {
        //
    }
}
