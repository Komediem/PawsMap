using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Interest Point", menuName = "Interest Point")]
public class InterestPointDatas : ScriptableObject
{
    public string title;
    public string subtitle;

    public string climaticCondition;
    public string faunaAndFlora;
    public string frequentRessources;
    public string dangerosity;

    public InterestPointTypes interestPointType;
    public List<InterestPointMultipleDatas> Researches;
    public List<InterestPointMultipleDatas> Illustrations;

    [HideInInspector] public List<InterestPointMultipleDatas> currentImages;
    [HideInInspector] public bool isResearches;
}

public enum InterestPointTypes
{
    place,
    creature,
    flora
}

[Serializable]
public struct InterestPointMultipleDatas
{
    public Sprite image;
    public string imageDescription; 
}
