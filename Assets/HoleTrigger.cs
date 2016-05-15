using UnityEngine;
using System.Collections;

public class HoleTrigger : MonoBehaviour
{
    public Collider2D FrontCollider;
    public Collider2D EdgeCollider;
    public bool InHole = false;
	// Use this for initialization
	void Start ()
	{
	    EdgeCollider.isTrigger = true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    
    void OnTriggerEnter2D()
    {
        FrontCollider.isTrigger = true;
       // GetComponent<Collider2D>().isTrigger = true;
        EdgeCollider.isTrigger = false;
        InHole = true;
    }

    public void Reset()
    {
        FrontCollider.isTrigger = false;
      //  GetComponent<Collider2D>().isTrigger = false;
        EdgeCollider.isTrigger = true;
        RandomPosition();
        InHole = false;
    }

    public BoxCollider2D RandomArea;
    public void RandomPosition()
    {
        var a = RandomArea.bounds;
        var area = new Rect(a.center, a.size);
        var x = UnityEngine.Random.Range(area.xMin, area.xMax);
        var y = UnityEngine.Random.Range(area.yMin, area.yMax);
        var z = transform.position.z;
        transform.parent.position = new Vector3(x,y,z);
    }
}
