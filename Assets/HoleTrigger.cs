using UnityEngine;
using System.Collections;

public class HoleTrigger : MonoBehaviour
{
    public Collider2D 碰撞器;
	//public Collider2D 触发器;
	// Use this for initialization
	void Start ()
	{
		碰撞器 = GameObject.Find ("back").GetComponent<Collider2D>();
		碰撞器.isTrigger = false;
		//触发器.isTrigger = true;
	}
	
	// Update is called once per frame
	void Update () {

	}
    
	void OnTriggerEnter2D()
    {
		碰撞器.isTrigger = true;
		StartCoroutine (Success ());
    }

    public void Reset()
    {
		碰撞器.isTrigger = false;
		RandomPosition ();
    }

	IEnumerator Success()
	{
		yield return new WaitForSeconds(0.1f);
		GameManager.Instance.Success();
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
