using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconInterestPoint : MonoBehaviour
{
    public static IconInterestPoint Instance;

    public Sprite iconPlace;
    public Sprite iconCreature;
    public Sprite iconFlora;

    [HideInInspector] public Sprite currentIcon;

    public void Awake()
    {
        if(Instance == null)
            Instance = this;
    }

    public void SetBaseImage(InterestPointTypes interestPointType)
    {
        switch(interestPointType)
        {
            case InterestPointTypes.place:

                currentIcon = iconPlace;
                break;  

            case InterestPointTypes.creature:

                currentIcon = iconCreature;
                break;

            case InterestPointTypes.flora:

                currentIcon = iconFlora;
                break;
        }
    }
}
