using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Interest Point", menuName = "Interest Point")]
public class InterestPointDatas : ScriptableObject
{
    public string title;
    public InterestPointTypes interestPointType;
    public List<InterestPointMultipleDatas> interestPointMultipleDatas;
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
