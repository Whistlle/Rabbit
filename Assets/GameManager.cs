using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.Find("GameManager").GetComponent<GameManager>();
            }
            return _instance;
        }
    }

    void Awake()
    {
        _instance = this;
    }

	// Use this for initialization
	void Start ()
	{
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

    public Ball Ball_;
    public HoleTrigger Hole;

    public void OnStartGame()
    {
        Reset();
    }

    public void Reset()
    {
        ARCamera.CleanPhysicsEdge();
        ARCamera.OnPlayButton();
        ARCamera.IsCreateCollider = false;
        Ball_.Reset();
        Hole.Reset();
        StartCoroutine(倒计时());
    }
    
    public Text CountDown;

    public IEnumerator 倒计时()
    {
        int num = 5;
        while (num >= 0)
        {         
            CountDown.text = num.ToString();
            num--;
            yield return new WaitForSeconds(1);
        }
        CountDown.text = "";
        ARCamera.OnPauseButton();
        Ball_.Active();
        //ARCamera.IsCreateCollider = true;
        ARCamera.SetPhysicsEdge();
        IsGameStart = true;
    }

    public int ScoreCount;
    public Text ScoreText;

   public bool IsGameStart = false;
    public void Success()
    {
        Reset();
        ScoreCount++;
        ScoreText.text = ScoreCount.ToString();
        IsGameStart = false;
    }

    public void Fail()
    {
        Reset();
        ScoreCount--;
        ScoreText.text = ScoreCount.ToString();
        IsGameStart = false;
    }

//}
}
