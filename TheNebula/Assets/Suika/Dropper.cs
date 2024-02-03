using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Dropper : MonoBehaviour
{
    SuikaControls InputAsset;

    public float Speed = 5f;
    public float Bounds = 5.5f;
    public float SpawnDelay = 0.5f;

    Rigidbody2D CurrentBall;
    float GravScale;
    bool Dropping = false;

    private void Start()
    {
        StartCoroutine(SpawnNewBall());
        InputAsset = new SuikaControls();
        InputAsset.Play.Enable();
        InputAsset.Play.Drop.started += DropBall;
    }

    // Update is called once per frame
    void Update()
    {
        Translate(Speed * Time.deltaTime * InputAsset.Play.Move.ReadValue<float>());
    }

    private void Translate(float distance)
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x + distance, -Bounds, Bounds);
        transform.position = pos;
    }

    private void DropBall(InputAction.CallbackContext context)
    {
        if (Dropping)
        {
            return;
        }
        CurrentBall.gravityScale = GravScale;
        CurrentBall.transform.parent = CollisionManager.Instance.transform;
        Dropping = true;
        StartCoroutine(SpawnNewBall());
    }

    private IEnumerator SpawnNewBall()
    {
        yield return new WaitForSeconds(SpawnDelay);
        CurrentBall = Instantiate(CollisionManager.Instance.BallPrefab, Vector3.up * 1000, Quaternion.identity, transform).GetComponent<Rigidbody2D>();
        GravScale = CurrentBall.gravityScale;
        CurrentBall.gravityScale = 0;
        yield return null;
        CurrentBall.transform.position = transform.position;
        CurrentBall.GetComponent<Ball>().UpdateBallData(CollisionManager.Instance.BallList.BallDatas[Random.Range(0,3)]);
        Dropping = false;
    }
}
