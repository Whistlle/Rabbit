using UnityEngine;
using System.Collections;

public class DropHole : MonoBehaviour
{

  //  public HoleTrigger Hole;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter2D()
    {
        GameManager.Instance.Success();
    }
}
