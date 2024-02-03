using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextBallUI : MonoBehaviour
{
    int LastBall = -1;
    public Dropper Dropper;
    Ball UIBall;
    // Start is called before the first frame update
    void Start()
    {
        UIBall = GetComponent<Ball>();

        GetComponent<Rigidbody2D>().gravityScale = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Dropper.ReadNextDrop != LastBall)
        {
            LastBall = Dropper.ReadNextDrop;
            UIBall.UpdateBallData(CollisionManager.Instance.BallList.BallDatas[LastBall]);
        }
    }
}
