using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBallList", menuName = "SuikaGame/BallList")]
public class BallListSO : ScriptableObject
{
	public List<BallDataSO> BallDatas;
	
	public BallDataSO GetNextBall(BallDataSO current){
		foreach(BallDataSO bd in BallDatas){
			if(bd.Stage == (current.Stage + 1)){
				return bd;
			}
		}
		return null;
	}
}
