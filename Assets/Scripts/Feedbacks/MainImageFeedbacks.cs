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



        if (GameManager.Instance.currentImage == GameManager.Instance.currentInterestPoint.currentImages.Count - 1)
        {
            GameManager.Instance.nextImage.SetActive(false);
        }

        if (GameManager.Instance.currentImage == 0)
        {
            GameManager.Instance.previousImage.SetActive(false);
        }
    }

    public void UnlockSlide()
    {
        GameManager.Instance.canSlide = true; 
    }
}
