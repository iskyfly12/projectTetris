using UnityEngine;
using DG.Tweening;

public class Block : MonoBehaviour
{
    [SerializeField]
    private int blockID;

    private Vector3 pointRotation = Vector3.zero;
    private Transform[] children;

    public int BlockID() { return blockID; }
    public Vector3 PointRotation() { return pointRotation; }
    public Transform[] Children() { return children; }

    void Awake()
    {
        children = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            children[i] = transform.GetChild(i);
            children[i].localScale = Vector3.zero;
            children[i].DOScale(Vector3.one, .25f);
        }
    }
}
