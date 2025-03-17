using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Interest Point", menuName = "Interest Point")]
public class InterestPointDatas : ScriptableObject
{
    public string title;
    public List<InterestPointMultipleDatas> interestPointDatas;
}

public struct InterestPointMultipleDatas
{
    public Sprite image;
    public string imageDescription; 
}
