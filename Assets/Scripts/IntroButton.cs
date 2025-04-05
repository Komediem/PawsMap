using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class IntroButton : MonoBehaviour
{
    public static IntroButton instance;

    public void Awake()
    {
        if(instance == null)
            instance = this;
    }

    public void Hovered()
    {
        if(GameManager.Instance.isIntro)
        this.GetComponentInParent<Animator>().SetBool("IsHovered?", true);
    }

    public void ResetImage()
    {
        this.GetComponentInParent<Animator>().SetBool("IsHovered?", false);
    }
}
