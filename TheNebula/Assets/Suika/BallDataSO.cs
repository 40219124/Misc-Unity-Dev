using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBallData", menuName = "SuikaGame/BallData")]
public class BallDataSO : ScriptableObject
{
	public Color Colour;
	public int Stage;
	public float Radius;
}
