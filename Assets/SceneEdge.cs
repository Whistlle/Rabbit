using UnityEngine;
using System.Collections;

public class SceneEdge : MonoBehaviour {

    void OnTriggerExit2D()
    {
        if(GameManager.Instance.IsGameStart)
            GameManager.Instance.Fail();
    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
