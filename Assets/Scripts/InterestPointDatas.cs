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

    public Sprite iconBase;
    public Sprite iconHovered;

    public List<InterestPointMultipleDatas> Researches;
    public List<InterestPointMultipleDatas> Illustrations;

    [HideInInspector] public List<InterestPointMultipleDatas> currentImages;
    [HideInInspector] public bool isResearches;
}

[Serializable]
public struct InterestPointMultipleDatas
{
    public Sprite image;
    public string imageDescription; 
}
