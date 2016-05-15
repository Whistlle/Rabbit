using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class Ball : MonoBehaviour
{

    public Sprite[] CollorBalls;
    public Vector2 OriginVector;

    Rigidbody2D _body;
	// Use this for initialization
    void Awake()
    {
        _body = GetComponent<Rigidbody2D>();
    }
	void Start ()
	{
	    _body.isKinematic = true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Active()
    {
        _body.isKinematic = false;
    }

    public void Reset()
    {
        _body.isKinematic = true;
        transform.localPosition =new Vector3(OriginVector.x, OriginVector.y, transform.localPosition.z);
        RamdomColor();
    }

    public void RamdomColor()
    {
        var idx = UnityEngine.Random.Range(0, CollorBalls.Length);
        GetComponent<SpriteRenderer>().sprite = CollorBalls[idx];
    }
}
