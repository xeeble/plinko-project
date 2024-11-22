using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BallTypes { Basic = 1, Stage1 = 2, Stage2 = 3, Stage3 = 4, none = 0 }
[CreateAssetMenu(fileName ="New Ball Data",menuName = "BallData")]
public class BallData : ScriptableObject
{
    public int BallID;
    public int BallValue;
    [SerializeField]private Color DefaultBallColour;
    public BallTypes Type;

    public void Awake()
    {
       
    }
}
