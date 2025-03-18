using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Interest Point", menuName = "Interest Point")]
public class InterestPointDatas : ScriptableObject
{
    public string title;
    public List<InterestPointMultipleDatas> interestPointMultipleDatas;
}

[Serializable]
public struct InterestPointMultipleDatas
{
    public Sprite image;
    public string imageDescription; 
}
