using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    private static CollisionManager _this;
    public static CollisionManager Instance
    {
        get { return _this; }
    }

    List<int> CollidingIDs = new List<int>();
    public GameObject BallPrefab;
    public BallListSO BallList;

    // Start is called before the first frame update
    void Start()
    {
        if (_this == null)
        {
            _this = this;
        }
    }

    public void ReportCollision(Ball source, Ball other)
    {
        bool ordered = source.GetInstanceID() < other.GetInstanceID();
        if (!ordered)
        {
            Ball temp = source;
            source = other;
            other = temp;
        }
        int sID = source.GetInstanceID();
        int oID = other.GetInstanceID();
        Debug.Log($"1: {sID}, 2: {oID}");
        if (!CollidingIDs.Contains(sID) && !CollidingIDs.Contains(oID))
        {
            CollidingIDs.Add(sID);
            CollidingIDs.Add(oID);

            StartCoroutine(ManageCollision(source, other));
        }
    }

    private IEnumerator ManageCollision(Ball source, Ball other)
    {
        yield return null;
        List<int> ids = new List<int> { source.GetInstanceID(), other.GetInstanceID() };
        BallDataSO next = BallList.GetNextBall(source.Data);
        Vector3 newPos = (source.transform.position + other.transform.position) / 2f;
        Destroy(source.gameObject);
        Destroy(other.gameObject);
        while (source != null)
        {
            yield return null;
            Debug.Log("waiting");
        }
        foreach (int id in ids)
        {
            CollidingIDs.Remove(id);
        }
        if (next != null)
        {
            Ball newBall = Instantiate(BallPrefab, Vector3.up * 1000, Quaternion.identity, transform).GetComponent<Ball>();
            yield return null;
            newBall.UpdateBallData(next);
            newBall.transform.position = newPos;
        }
    }
}
