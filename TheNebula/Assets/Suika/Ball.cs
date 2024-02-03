using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    protected BallDataSO _data;
    protected CircleCollider2D Col;
    protected SpriteRenderer Sprite;

    public float GrowTime = 0.15f;

    public void UpdateBallData(BallDataSO newData)
    {
        _data = newData;
        Sprite.color = newData.Colour;
        StartCoroutine(GrowIn());
    }

    public BallDataSO Data { get { return _data; } }

    // Start is called before the first frame update
    void Start()
    {
        Col = GetComponent<CircleCollider2D>();
        Sprite = GetComponentInChildren<SpriteRenderer>();
    }

    public bool IsSameStage(Ball other)
    {
        return _data.Stage == other.Data.Stage;
    }

    private IEnumerator GrowIn()
    {
        float elapsed = 0;
        Col.radius = 0;
        Sprite.transform.localScale = Vector3.zero;
        while (elapsed < GrowTime)
        {
            yield return null;
            elapsed += Time.deltaTime;
            float t = elapsed / GrowTime;
            Col.radius = Mathf.Lerp(0, Data.Radius, t);
            Sprite.transform.localScale = Vector3.one * Mathf.Lerp(0, (Data.Radius / 0.5f), t);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ball"))
        {
            Ball other = collision.collider.GetComponent<Ball>();
            if (!IsSameStage(other))
            {
                return;
            }
            // ~~~ time out more collisions
            // ~~~ tell collision manager
            CollisionManager.Instance.ReportCollision(this, other);
        }
    }
}
