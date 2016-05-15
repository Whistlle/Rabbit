using System;
using System.Collections;
using System.Collections.Generic;
using OpenCVForUnity;
using OpenCVForUnitySample;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Web cam texture to mat helper.
/// </summary>
public class WebCamTextureHelper_ : MonoBehaviour
{
    /// <summary>
    /// The name of the device.
    /// </summary>
    public string requestDeviceName = null;

    /// <summary>
    /// The width.
    /// </summary>
    public int requestWidth = 640;

    /// <summary>
    /// The height.
    /// </summary>
    public int requestHeight = 480;

    /// <summary>
    /// Should use front facing.
    /// </summary>
    public bool requestIsFrontFacing = false;

    /// <summary>
    /// The flip vertical.
    /// </summary>
    public bool flipVertical = false;

    /// <summary>
    /// The flip horizontal.
    /// </summary>
    public bool flipHorizontal = false;

    /// <summary>
    /// The on inited event.
    /// </summary>
    public UnityEvent OnInitedEvent;

    /// <summary>
    /// The on disposed event.
    /// </summary>
    public UnityEvent OnDisposedEvent;

    /// <summary>
    /// The web cam texture.
    /// </summary>
    WebCamTexture webCamTexture;

    /// <summary>
    /// The web cam device.
    /// </summary>
    WebCamDevice webCamDevice;

    /// <summary>
    /// The init done.
    /// </summary>
    bool initDone = false;

    //public bool IsCreateEdge = false;
    /// <summary>
    /// The screenOrientation.
    /// </summary>
    ScreenOrientation screenOrientation = ScreenOrientation.Portrait;


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (initDone)
        {
            if (screenOrientation != Screen.orientation)
            {
                StartCoroutine(init());
            }
        }
    }

    public void Init()
    {
        if (OnInitedEvent == null)
            OnInitedEvent = new UnityEvent();
        if (OnDisposedEvent == null)
            OnDisposedEvent = new UnityEvent();

        StartCoroutine(init());
    }

    /// <summary>
    /// Init the specified deviceName, requestWidth, requestHeight, requestIsFrontFacing and OnInited.
    /// </summary>
    /// <param name="deviceName">Device name.</param>
    /// <param name="requestWidth">Request width.</param>
    /// <param name="requestHeight">Request height.</param>
    /// <param name="requestIsFrontFacing">If set to <c>true</c> request is front facing.</param>
    /// <param name="OnInited">On inited.</param>
    public void Init(string deviceName, int requestWidth, int requestHeight, bool requestIsFrontFacing)
    {
        this.requestDeviceName = deviceName;
        this.requestWidth = requestWidth;
        this.requestHeight = requestHeight;
        this.requestIsFrontFacing = requestIsFrontFacing;
        if (OnInitedEvent == null)
            OnInitedEvent = new UnityEvent();
        if (OnDisposedEvent == null)
            OnDisposedEvent = new UnityEvent();

        StartCoroutine(init());
    }

    /// <summary>
    /// Init this instance.
    /// </summary>
    private IEnumerator init()
    {
        if (initDone)
            Dispose();

        if (!String.IsNullOrEmpty(requestDeviceName))
        {
       //     Debug.Log ("deviceName is "+requestDeviceName);
            webCamTexture = new WebCamTexture(requestDeviceName, requestWidth, requestHeight);
        }
        else
        {
            //			Debug.Log ("deviceName is null");
            // Checks how many and which cameras are available on the device
            for (int cameraIndex = 0; cameraIndex < WebCamTexture.devices.Length; cameraIndex++)
            {

                if (WebCamTexture.devices[cameraIndex].isFrontFacing == requestIsFrontFacing)
                {

                   Debug.Log (cameraIndex + " name " + WebCamTexture.devices [cameraIndex].name + " isFrontFacing " + WebCamTexture.devices [cameraIndex].isFrontFacing);

                    webCamDevice = WebCamTexture.devices[cameraIndex];

                    webCamTexture = new WebCamTexture(webCamDevice.name, requestWidth, requestHeight);

                    break;
                }


            }
        }

        if (webCamTexture == null)
        {
  			Debug.Log ("webCamTexture is null");
            if (WebCamTexture.devices.Length > 0)
            {
                webCamDevice = WebCamTexture.devices[1];
                webCamTexture = new WebCamTexture(webCamDevice.name, requestWidth, requestHeight);
            }
            else
            {
                webCamTexture = new WebCamTexture(requestWidth, requestHeight);
            }
        }

       Debug.Log ("name " + webCamTexture.name + " width " + webCamTexture.width + " height " + webCamTexture.height + " fps " + webCamTexture.requestedFPS);


        screenOrientation = ScreenOrientation.Portrait;
        // Starts the camera
        //  webCamTexture.Play();
        /*

          while (true)
          {
              //If you want to use webcamTexture.width and webcamTexture.height on iOS, you have to wait until webcamTexture.didUpdateThisFrame == 1, otherwise these two values will be equal to 16. (http://forum.unity3d.com/threads/webcamtexture-and-error-0x0502.123922/)
  #if UNITY_IOS && !UNITY_EDITOR && (UNITY_4_6_3 || UNITY_4_6_4 || UNITY_5_0_0 || UNITY_5_0_1)
              if (webCamTexture.width > 16 && webCamTexture.height > 16) {
  #else
              if (webCamTexture.didUpdateThisFrame)
              {
  #if UNITY_IOS && !UNITY_EDITOR && UNITY_5_2
                      while (webCamTexture.width <= 16) {
                          webCamTexture.GetPixels32 ();
                          yield return new WaitForEndOfFrame ();
                      } 
  #endif
  #endif

                  Debug.Log("name " + webCamTexture.name + " width " + webCamTexture.width + " height " +
                            webCamTexture.height + " fps " + webCamTexture.requestedFPS);
                  Debug.Log("videoRotationAngle " + webCamTexture.videoRotationAngle + " videoVerticallyMirrored " +
                            webCamTexture.videoVerticallyMirrored + " isFrongFacing " + webCamDevice.isFrontFacing);

                  //				Debug.Log ("Screen.orientation " + Screen.orientation);
                  screenOrientation = Screen.orientation;
                  //					screenOrientation = ScreenOrientation.PortraitUpsideDown;



                  //				webCamTexture.Stop ();

                  initDone = true;

                  if (OnInitedEvent != null)
                      OnInitedEvent.Invoke();


                  break;
              }
              else
              {
                  yield return 0;
              }
          }*/
        yield return 0;
    }

    /// <summary>
    /// Ises the inited.
    /// </summary>
    /// <returns><c>true</c>, if inited was ised, <c>false</c> otherwise.</returns>
    public bool isInited()
    {

        return initDone;
    }

    /// <summary>
    /// Play this instance.
    /// </summary>
    public void Play()
    {
       //if (initDone)
        initDone = true;
        webCamTexture.Play();
    }

    /// <summary>
    /// Pause this instance.
    /// </summary>
    public void Pause()
    {
        //if (initDone)
            webCamTexture.Pause();
    }

    /// <summary>
    /// Stop this instance.
    /// </summary>
    public void Stop()
    {
        if (initDone)
            webCamTexture.Stop();
    }

    /// <summary>
    /// Ises the playing.
    /// </summary>
    /// <returns><c>true</c>, if playing was ised, <c>false</c> otherwise.</returns>
    public bool isPlaying()
    {
        if (!initDone)
            return false;
        return webCamTexture.isPlaying;
    }

    /// <summary>
    /// Gets the web cam texture.
    /// </summary>
    /// <returns>The web cam texture.</returns>
    public WebCamTexture GetWebCamTexture()
    {
        return webCamTexture;
    }

    Mat rgbaMat;

    /// <summary>
    /// Gets the web cam device.
    /// </summary>
    /// <returns>The web cam device.</returns>
    public WebCamDevice GetWebCamDevice()
    {
        return webCamDevice;
    }

    /// <summary>
    /// Dids the update this frame.
    /// </summary>
    /// <returns><c>true</c>, if update this frame was dided, <c>false</c> otherwise.</returns>
    public bool didUpdateThisFrame()
    {
        return true;
        if (!initDone)
            return false;

#if UNITY_IOS && !UNITY_EDITOR && (UNITY_4_6_3 || UNITY_4_6_4 || UNITY_5_0_0 || UNITY_5_0_1)
						if (webCamTexture.width > 16 && webCamTexture.height > 16) {
								return true;
						} else {
								return false;
						}
#else
        return webCamTexture.didUpdateThisFrame;
#endif

    }



    /// <summary>
    /// Releases all resource used by the <see cref="WebCamTextureToMatHelper"/> object.
    /// </summary>
    /// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="WebCamTextureToMatHelper"/>. The
    /// <see cref="Dispose"/> method leaves the <see cref="WebCamTextureToMatHelper"/> in an unusable state. After
    /// calling <see cref="Dispose"/>, you must release all references to the <see cref="WebCamTextureToMatHelper"/> so
    /// the garbage collector can reclaim the memory that the <see cref="WebCamTextureToMatHelper"/> was occupying.</remarks>
    public void Dispose()
    {
        initDone = false;

        if (webCamTexture != null)
        {
            webCamTexture.Stop();
            webCamTexture = null;
        }

        if (OnDisposedEvent != null)
            OnDisposedEvent.Invoke();
    }
}
