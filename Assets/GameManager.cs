using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public GameObject Ball;
    public Vector3 BallOriginPos;
	// Use this for initialization
	void Start ()
	{
	    Ball = GameObject.Find("ball");
	    BallOriginPos = Ball.transform.position;
	    ARCamera = GameObject.Find("GameCamera").GetComponent<WebCamTextureProcess>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public WebCamTextureProcess ARCamera;
    
    public void OnStartCamera()
    {
        ARCamera.OnPlayButton();
    }

    public void OnPauseCamera()
    {
        ARCamera.OnPauseButton();
    }

    public void OnStartGame()
    {
        ARCamera.IsCreateCollider = true;
        Ball.GetComponent<Rigidbody2D>().isKinematic = false;
    }

    public void Reset()
    {
        Ball.transform.position = BallOriginPos;
        Ball.GetComponent<Rigidbody2D>().isKinematic = true;
    }
}
