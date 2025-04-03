using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainImageFeedbacks : MonoBehaviour
{
    public void ChangeImage()
    {
        GameManager.Instance.AssignDatasInterestPoint(GameManager.Instance.currentInterestPoint);

        this.GetComponent<Animator>().SetBool("IsSwitching", false);
        this.GetComponent<Animator>().SetBool("IsSwitchingBackward", false);
    }

    public void UnlockSlide()
    {
        GameManager.Instance.canSlide = true; 
    }
}
